(function () {
    document.querySelectorAll("[data-coupon-slider]").forEach(function (track) {
        var section = track.closest(".promo-coupons-section");
        if (!section) return;

        var buttons = section.querySelectorAll("[data-coupon-direction]");
        if (!buttons.length) return;

        function updateButtons() {
            var maxScroll = Math.max(0, track.scrollWidth - track.clientWidth);
            buttons.forEach(function (button) {
                var direction = Number(button.dataset.couponDirection);
                button.disabled = direction < 0
                    ? track.scrollLeft <= 1
                    : track.scrollLeft >= maxScroll - 1;
            });
        }

        buttons.forEach(function (button) {
            button.addEventListener("click", function () {
                var direction = Number(button.dataset.couponDirection);
                track.scrollBy({ left: direction * Math.max(260, Math.round(track.clientWidth * 0.82)), behavior: "smooth" });
            });
        });

        track.addEventListener("scroll", updateButtons, { passive: true });
        window.addEventListener("resize", updateButtons);
        updateButtons();
    });
})();
