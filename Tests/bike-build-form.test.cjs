const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");

test("the quote form keeps a right-aligned submit button without adjacent instructions", () => {
    const view = readFileSync(join(__dirname, "../Views/Cart/ConfigureBike.cshtml"), "utf8");
    const css = readFileSync(join(__dirname, "../wwwroot/css/dashboard.css"), "utf8");

    const form = view.match(/<form asp-controller="Cart" asp-action="ConfigureBike" method="post">([\s\S]*?)<\/form>/)?.[1];
    const actions = form?.match(/<div class="dash-actions bike-builder-actions">([\s\S]*?)<\/div>/)?.[1];

    assert.ok(actions, "the form should have a dedicated action row");
    assert.match(actions.trim(), /^<button\b[^>]*type="submit"[^>]*disabled="@unavailable"[^>]*>ส่งคำขอประเมินราคา<\/button>$/);
    assert.match(css, /\.bike-builder-actions\s*\{[^}]*justify-content:\s*flex-end\s*;/);
});

test("each picker puts its MVC-bound choices inside an in-flow disclosure", () => {
    const view = readFileSync(join(__dirname, "../Views/Cart/ConfigureBike.cshtml"), "utf8");
    const css = readFileSync(join(__dirname, "../wwwroot/css/dashboard.css"), "utf8");

    const picker = view.match(/<details class="bike-builder-picker">([\s\S]*?)<\/details>/)?.[1];

    assert.ok(picker);
    assert.match(picker, /<summary[\s\S]*data-part-summary/);
    assert.match(picker, /<fieldset[\s\S]*asp-for="Form.Lines\[index\].StockItemId" type="radio"/);
    assert.doesNotMatch(css.match(/\.bike-builder-options\s*\{[^}]*\}/)?.[0] ?? "", /position:\s*(absolute|fixed)/);
});
