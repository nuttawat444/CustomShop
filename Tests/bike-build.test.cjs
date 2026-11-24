const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");
const { runInNewContext } = require("node:vm");

test("selecting and removing an accessory updates the preview and requested quantity", () => {
    const events = new Map();
    const quantity = { value: "0", addEventListener: (name, fn) => events.set("quantity-" + name, fn) };
    const preview = { hidden: true, removeAttribute(name) { delete this[name]; } };
    const sku = { textContent: "" };
    const select = {
        value: "14",
        get selectedOptions() { return this.value ? [{ value: this.value, dataset: { sku: "RACK-14", image: "/rack.jpg", name: "Rack" } }] : [{ dataset: {} }]; },
        addEventListener: (name, fn) => events.set("select-" + name, fn),
        dispatchEvent: event => events.get("select-" + event.type)()
    };
    const fields = { "[data-part-select]": select, "[data-part-quantity]": quantity, "[data-part-image]": preview, "[data-part-sku]": sku };
    const row = { dataset: { min: "0" }, querySelector: selector => fields[selector] };
    const file = join(__dirname, "../wwwroot/js/bike-build.js");
    runInNewContext(readFileSync(file, "utf8"), { document: { querySelectorAll: () => [row] }, Event }, { filename: file });

    events.get("select-change")();
    assert.equal(quantity.value, "1");
    assert.equal(preview.src, "/rack.jpg");
    assert.equal(sku.textContent, "RACK-14");

    quantity.value = "0";
    events.get("quantity-change")();
    assert.equal(select.value, "");
    assert.equal(preview.hidden, true);
    assert.equal(sku.textContent, "");
});
