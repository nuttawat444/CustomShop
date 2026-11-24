const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");
const { runInNewContext } = require("node:vm");

const themePath = join(__dirname, "../wwwroot/js/theme.js");
const script = readFileSync(themePath, "utf8");

function page(storage, blocked = false, lateToggle = false) {
    const listeners = new Map();
    const element = { dataset: {} };
    let toggleAvailable = !lateToggle;
    const toggle = { setAttribute: (_, value) => { toggle.pressed = value; } };
    const checkStorage = () => { if (blocked) throw Error("Storage blocked"); };
    const document = {
        documentElement: element,
        querySelectorAll: () => toggleAvailable ? [toggle] : [],
        addEventListener: (name, action) => listeners.set(name, action)
    };
    const window = {
        localStorage: {
            getItem() { checkStorage(); return storage.get("customshop_theme"); },
            setItem(key, value) { checkStorage(); storage.set(key, value); }
        },
        addEventListener: document.addEventListener
    };
    runInNewContext(script, { document, window }, { filename: themePath });
    return {
        theme: () => element.dataset.theme,
        pressed: () => toggle.pressed,
        ready: () => { toggleAvailable = true; listeners.get("DOMContentLoaded")(); },
        click: (isToggle = true) => listeners.get("click")({ target: { closest: () => isToggle ? toggle : null } }),
        storageEvent: (event) => listeners.get("storage")(event)
    };
}

test("saves each selection and initializes the next page with it", () => {
    const storage = new Map();
    const firstPage = page(storage);

    firstPage.click();
    const nextPage = page(storage);

    assert.equal(firstPage.pressed(), "true");
    assert.equal(nextPage.theme(), "dark");
    nextPage.click();
    assert.equal(page(storage).theme(), "light");
});

test("invalid storage and clicks away from the switch keep the default", () => {
    const initial = page(new Map([["customshop_theme", "random"]]));

    initial.click(false);

    assert.equal(initial.theme(), "light");
    assert.equal(initial.pressed(), "false");
});

test("switch changes the page if storage is blocked", () => {
    const initial = page(new Map(), true);

    initial.click();

    assert.equal(initial.theme(), "dark");
    assert.equal(initial.pressed(), "true");
});

test("updates the switch when the theme loads before the page body", () => {
    const initial = page(new Map([["customshop_theme", "dark"]]), false, true);
    assert.equal(initial.pressed(), undefined);

    initial.ready();

    assert.equal(initial.theme(), "dark");
    assert.equal(initial.pressed(), "true");
});

test("theme changes in another tab are synchronized", () => {
    const initial = page(new Map());

    initial.storageEvent({ key: "customshop_theme", newValue: "dark" });

    assert.equal(initial.theme(), "dark");
    initial.storageEvent({ key: null, newValue: null });
    assert.equal(initial.theme(), "light");
});



function luminance(hex) {
    const channel = index => {
        const value = Number.parseInt(hex.slice(index, index + 2), 16) / 255;
        return value <= .04045 ? value / 12.92 : ((value + .055) / 1.055) ** 2.4;
    };
    return .2126 * channel(0) + .7152 * channel(2) + .0722 * channel(4);
}

for (const variant of ["light", "dark"]) {
    test(variant + " theme keeps readable foreground and background pairs", () => {
        const css = readFileSync(join(__dirname, "../wwwroot/css/theme.css"), "utf8");
        const parts = [...css.matchAll(/:root[^{}]*\{([^{}]+)\}/g)];
        const vars = Object.fromEntries([...parts.slice(0, variant === "light" ? 1 : 2).map(match => match[1]).join("\n").matchAll(/--theme-([\w-]+): #([0-9a-f]{3,6});/g)]
            .map(([, key, hex]) => [key, hex.length === 3 ? [...hex].map(letter => letter + letter).join("") : hex]));

        for (const [foreground, background] of [
            ["ink", "canvas"], ["ink", "surface"], ["muted", "surface"], ["muted", "raised"],
            ["accent", "surface"], ["on-accent", "accent-bg"], ["on-accent", "accent-hover"],
            ["green", "green-bg"], ["amber", "amber-bg"], ["error", "error-bg"], ["info", "info-bg"]
        ]) {
            const tones = [luminance(vars[foreground]), luminance(vars[background])].sort((a, b) => b - a);
            const contrast = (tones[0] + .05) / (tones[1] + .05);
            assert.ok(contrast >= 4.5, foreground + "/" + background + ": " + contrast);
        }
    });
}
