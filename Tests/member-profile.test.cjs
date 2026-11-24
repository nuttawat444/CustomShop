const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");
const { runInNewContext } = require("node:vm");

const read = path => readFileSync(join(__dirname, "..", path), "utf8");

test("the profile exposes buyer shortcuts, coupon availability, and the saved-code form", () => {
    const view = read("Views/Account/index.cshtml");
    for (const action of ["History", "Quotes", "Product", "SaveOffer"])
        assert.ok(view.includes(`asp-action="${action}"`));
    assert.match(view, /โค้ดส่วนลดของฉัน/);
    assert.match(view, /coupon\.Used/);
    assert.match(view, /data-copy-coupon/);
});

test("copying an available coupon writes the code and reports the result", async () => {
    let click;
    let copied;
    const status = { textContent: "" };
    const code = { textContent: "SHOP10" };
    const button = {
        dataset: { copyCoupon: "SHOP10" },
        addEventListener: (event, handler) => { if (event === "click") click = handler; },
        closest: () => ({ querySelector: () => code })
    };
    const document = { querySelectorAll: () => [button], querySelector: () => status };
    const navigator = { clipboard: { writeText: async value => { copied = value; } } };

    runInNewContext(read("wwwroot/js/member-profile.js"), { document, navigator });
    await click();

    assert.equal(copied, "SHOP10");
    assert.match(status.textContent, /SHOP10/);
});
