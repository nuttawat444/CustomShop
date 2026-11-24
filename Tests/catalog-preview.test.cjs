const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");
const { runInNewContext } = require("node:vm");

const source = readFileSync(join(__dirname, "../wwwroot/js/catalog-preview.js"), "utf8");

function fixture() {
    const events = new Map();
    const title = { textContent: "" };
    const body = {
        children: [],
        scrollTop: 60,
        replaceChildren(...children) { this.children = children; }
    };
    const modal = {
        querySelector: selector => selector === ".modal-title" ? title : body,
        addEventListener: (event, handler) => events.set(event, handler)
    };
    const item = (name, asset) => ({
        dataset: { title: name },
        content: { cloneNode: deep => ({ asset, deep }) }
    });
    const elements = new Map([
        ["catalog-preview", modal],
        ["catalog-preview-item-5", item("Motorcycle", "bike.jpg")],
        ["catalog-preview-item-9", item("<b>Helmet</b>", "helmet.jpg")]
    ]);
    runInNewContext(source, { document: { getElementById: id => elements.get(id) } });

    function open(id) {
        const event = { relatedTarget: id === undefined ? null : { dataset: { catalogPreview: id } },
            preventDefault() { this.cancelled = true; } };
        events.get("show.bs.modal")(event);
        return event;
    }

    return { open, hide: () => events.get("hidden.bs.modal")(), title, body };
}

test("selecting a second item replaces the title and content and resets scroll", () => {
    const preview = fixture();

    preview.open("5");
    assert.equal(preview.title.textContent, "Motorcycle");
    preview.body.scrollTop = 90;
    preview.open("9");

    assert.equal(preview.title.textContent, "<b>Helmet</b>");
    assert.deepEqual(preview.body.children, [{ asset: "helmet.jpg", deep: true }]);
    assert.equal(preview.body.scrollTop, 0);
});

test("closing clears the preview and reopening with an unknown trigger is cancelled", () => {
    const preview = fixture();
    preview.open("5");

    preview.hide();
    const invalidItem = preview.open("99");
    const noTrigger = preview.open();

    assert.equal(preview.title.textContent, "");
    assert.deepEqual(preview.body.children, []);
    assert.equal(invalidItem.cancelled, true);
    assert.equal(noTrigger.cancelled, true);
});

test("the shared script can load on a page without a preview modal", () => {
    const document = { getElementById: () => null };

    assert.doesNotThrow(() => runInNewContext(source, { document }));
});
