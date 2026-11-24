const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");

test("the warehouse header only opens the add-stock dialog", () => {
    const view = readFileSync(join(__dirname, "../Views/WarehouseWorker/Index.cshtml"), "utf8");
    const header = view.match(/<header class="dash-header">([\s\S]*?)<\/header>/)?.[1] ?? "";

    assert.deepEqual(Array.from(header.matchAll(/data-dash-open="([^"]+)"/g), match => match[1]), ["stock-create"]);
    assert.match(view, /<dialog\b[^>]*\bid="stock-create"/);
});
