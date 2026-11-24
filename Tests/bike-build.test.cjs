const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");
const { runInNewContext } = require("node:vm");

function setup(minimum) {
    const events = new Map();
    const quantity = { value: String(minimum), addEventListener: (name, fn) => events.set(name, fn) };
    const preview = { hidden: minimum === 0, removeAttribute(name) { delete this[name]; } };
    const sku = { textContent: "" };
    const summary = { textContent: "" };
    const price = { textContent: "" };
    const none = { value: "", dataset: { name: "ไม่เลือก" } };
    const standard = { value: "13", dataset: { sku: "STD-13", image: "/standard.jpg", name: "Standard", price: "฿100.00" } };
    const alternative = { value: "14", dataset: { sku: "ALT-14", image: "/alternative.jpg", name: "Alternative", price: "฿200.00" } };
    const choices = minimum === 0 ? [none, standard, alternative] : [standard, alternative];
    const fields = {
        "[data-part-quantity]": quantity,
        "[data-part-image]": preview,
        "[data-part-sku]": sku,
        "[data-part-summary]": summary,
        "[data-part-summary-price]": price
    };
    const row = {
        querySelector: selector => fields[selector],
        querySelectorAll: () => choices,
        addEventListener: (name, fn) => events.set("row-" + name, fn)
    };
    const file = join(__dirname, "../wwwroot/js/bike-build.js");
    runInNewContext(readFileSync(file, "utf8"), { document: { querySelectorAll: () => [row] } }, { filename: file });

    return {
        quantity, preview, sku, summary, price, none, standard, alternative,
        choose(choice) {
            choices.forEach(item => { item.checked = item === choice; });
            events.get("row-change")({ target: choice });
        },
        clearQuantity() {
            quantity.value = "0";
            events.get("change")();
        }
    };
}

test("changing a required part updates its summary and preview while retaining quantity", () => {
    const picker = setup(1);

    picker.choose(picker.standard);
    assert.equal(picker.summary.textContent, "Standard");
    picker.choose(picker.alternative);

    assert.equal(picker.quantity.value, "1");
    assert.equal(picker.summary.textContent, "Alternative");
    assert.equal(picker.price.textContent, "฿200.00");
    assert.equal(picker.preview.src, "/alternative.jpg");
    assert.equal(picker.preview.alt, "Alternative");
    assert.equal(picker.sku.textContent, "ALT-14");
});

test("selecting and clearing an optional part keeps its radio, quantity, and summary in sync", () => {
    const picker = setup(0);

    picker.choose(picker.alternative);
    assert.equal(picker.quantity.value, "1");
    picker.choose(picker.none);
    assert.equal(picker.quantity.value, "0");
    assert.equal(picker.price.textContent, "");
    picker.choose(picker.alternative);
    picker.clearQuantity();

    assert.equal(picker.none.checked, true);
    assert.equal(picker.summary.textContent, "ไม่เลือก");
    assert.equal(picker.preview.hidden, true);
    assert.equal(picker.preview.src, undefined);
    assert.equal(picker.sku.textContent, "");
});
