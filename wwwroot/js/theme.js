(() => {
    const key = "customshop_theme";
    const root = document.documentElement;

    function applyTheme(value) {
        root.dataset.theme = value === "dark" ? "dark" : "light";
        document.querySelectorAll("[data-theme-toggle]").forEach(button => {
            button.setAttribute("aria-pressed", String(root.dataset.theme === "dark"));
        });
    }

    try {
        applyTheme(window.localStorage.getItem(key));
    } catch {
        applyTheme(null);
    }

    document.addEventListener("DOMContentLoaded", () => applyTheme(root.dataset.theme));
    document.addEventListener("click", event => {
        if (!event.target.closest("[data-theme-toggle]")) return;
        const next = root.dataset.theme === "dark" ? "light" : "dark";
        applyTheme(next);
        try {
            window.localStorage.setItem(key, next);
        } catch {
            // The current page still changes when browser storage is unavailable.
        }
    });
    window.addEventListener("storage", event => {
        if (event.key === key || event.key === null) {
            applyTheme(event.key === null ? null : event.newValue);
        }
    });
})();
