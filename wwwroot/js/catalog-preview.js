(() => {
    const modal = document.getElementById("catalog-preview");
    if (!modal) return;

    const title = modal.querySelector(".modal-title");
    const body = modal.querySelector(".modal-body");

    modal.addEventListener("show.bs.modal", event => {
        const itemId = event.relatedTarget?.dataset.catalogPreview;
        const template = document.getElementById("catalog-preview-item-" + itemId);
        if (!template?.content) {
            event.preventDefault();
            return;
        }

        title.textContent = template.dataset.title;
        body.replaceChildren(template.content.cloneNode(true));
        body.scrollTop = 0;
    });

    modal.addEventListener("hidden.bs.modal", () => {
        title.textContent = "";
        body.replaceChildren();
    });
})();
