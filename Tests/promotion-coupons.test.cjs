const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");
const { runInNewContext } = require("node:vm");

const source = readFileSync(join(__dirname, "../wwwroot/js/promotion-coupons.js"), "utf8");
const view = readFileSync(join(__dirname, "../Views/Home/Promotion.cshtml"), "utf8");

function fixture({ scrollWidth = 1200, clientWidth = 600, scrollLeft = 0 } = {}) {
    const handlers = new Map();
    const clickHandlers = new Map();
    const buttons = [-1, 1].map(direction => ({
        dataset: { couponDirection: String(direction) },
        addEventListener: (name, handler) => clickHandlers.set(direction, handler)
    }));
    const track = {
        scrollWidth,
        clientWidth,
        scrollLeft,
        closest: () => section,
        addEventListener: (name, handler) => handlers.set(name, handler),
        scrollBy(options) { this.lastScroll = options; }
    };
    const section = { querySelectorAll: () => buttons };
    runInNewContext(source, {
        document: { querySelectorAll: () => [track] },
        window: { addEventListener() {} }
    });
    return { buttons, clickHandlers, handlers, track };
}

test("promotion coupons render as a single horizontal, snap-scrolling row", () => {
    assert.match(view, /class="promo-coupons-track"[^>]*data-coupon-slider/);
    assert.match(view, /scroll-snap-type:\s*x mandatory/);
    assert.match(view, /promo-coupon-slide/);
    assert.match(view, /\.promo-coupons-carousel\s*\{[^}]*grid-template-columns:\s*40px minmax\(0, 1fr\) 40px/s);
    assert.match(view, /data-coupon-direction="-1"[\s\S]*?data-coupon-slider[\s\S]*?data-coupon-direction="1"/);
    assert.doesNotMatch(view, /\.promo-coupons-controls\s*\{[^}]*position:\s*absolute/s);
});

test("slider arrows reflect the current scroll position", () => {
    const slider = fixture();

    assert.equal(slider.buttons[0].disabled, true);
    assert.equal(slider.buttons[1].disabled, false);

    slider.track.scrollLeft = 600;
    slider.handlers.get("scroll")();

    assert.equal(slider.buttons[0].disabled, false);
    assert.equal(slider.buttons[1].disabled, true);
});

test("each arrow scrolls by a useful portion of the visible track", () => {
    const slider = fixture({ clientWidth: 600 });

    slider.clickHandlers.get(1)();

    assert.equal(slider.track.lastScroll.left, 492);
    assert.equal(slider.track.lastScroll.behavior, "smooth");
});
