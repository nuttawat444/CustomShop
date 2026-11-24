document.querySelectorAll("[data-build-row]").forEach(row => {
    const choices = Array.from(row.querySelectorAll("[data-part-choice]"));
    const emptyChoice = choices.find(choice => choice.value === "");
    const quantity = row.querySelector("[data-part-quantity]");
    const preview = row.querySelector("[data-part-image]");
    const sku = row.querySelector("[data-part-sku]");
    const summary = row.querySelector("[data-part-summary]");
    const price = row.querySelector("[data-part-summary-price]");

    function updateSelection(choice) {
        const selected = Boolean(choice.value);
        if (emptyChoice) quantity.value = selected ? "1" : "0";
        summary.textContent = choice.dataset.name;
        price.textContent = choice.dataset.price ?? "";
        sku.textContent = choice.dataset.sku ?? "";
        preview.hidden = !selected;
        if (selected) {
            preview.src = choice.dataset.image;
            preview.alt = choice.dataset.name;
        } else {
            preview.removeAttribute("src");
            preview.alt = "";
        }
    }

    row.addEventListener("change", event => {
        if (choices.includes(event.target)) updateSelection(event.target);
    });

    quantity.addEventListener("change", () => {
        if (emptyChoice && quantity.value === "0" && !emptyChoice.checked) {
            emptyChoice.checked = true;
            updateSelection(emptyChoice);
        }
    });
});
