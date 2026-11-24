const couponCopyStatus = document.querySelector("[data-coupon-copy-status]");

document.querySelectorAll("[data-copy-coupon]").forEach(button => {
    button.addEventListener("click", async () => {
        const code = button.dataset.copyCoupon;
        try {
            await navigator.clipboard.writeText(code);
            couponCopyStatus.textContent = `คัดลอกโค้ด ${code} แล้ว`;
        } catch {
            couponCopyStatus.textContent = `คัดลอกอัตโนมัติไม่ได้ พิมพ์โค้ด ${code} ในตะกร้าได้เลย`;
        }
    });
});
