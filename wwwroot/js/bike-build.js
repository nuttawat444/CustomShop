document.querySelectorAll("[data-build-row]").forEach(row => {
    const select = row.querySelector("[data-part-select]");
    const quantity = row.querySelector("[data-part-quantity]");
    const preview = row.querySelector("[data-part-image]");
    const sku = row.querySelector("[data-part-sku]");

    select.addEventListener("change", () => {
        const option = select.selectedOptions[0];
        const selected = Boolean(option?.value);
        if (Number(row.dataset.min) === 0) quantity.value = selected ? "1" : "0";
        sku.textContent = option?.dataset.sku ?? "";
        preview.hidden = !selected;
        if (selected) {
            preview.src = option.dataset.image;
            preview.alt = option.dataset.name;
        } else {
            preview.removeAttribute("src");
            preview.alt = "";
        }
    });

    quantity.addEventListener("change", () => {
        if (Number(row.dataset.min) === 0 && quantity.value === "0" && select.value) {
            select.value = "";
            select.dispatchEvent(new Event("change"));
        }
    });
});
