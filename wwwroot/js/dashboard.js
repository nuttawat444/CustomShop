document.addEventListener("click", (event) => {
    if (event.target.closest("[data-receipt-print]")) {
        window.print();
        return;
    }

    const trigger = event.target.closest("[data-dash-open]");
    if (!trigger) return;

    const dialog = document.getElementById(trigger.dataset.dashOpen);
    if (dialog instanceof HTMLDialogElement && !dialog.open) {
        dialog.showModal();
    }
});
