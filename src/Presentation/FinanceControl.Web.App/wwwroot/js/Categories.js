(function () {
    var overlay = document.getElementById('fc-cat-modal');
    if (!overlay) return;

    function open() {
        overlay.classList.remove('is-hidden');
    }

    function close() {
        overlay.classList.add('is-hidden');
    }

    document.querySelectorAll('[data-fc-open-modal]').forEach(function (el) {
        el.addEventListener('click', open);
    });

    document.querySelectorAll('[data-fc-close-modal]').forEach(function (el) {
        el.addEventListener('click', close);
    });

    overlay.addEventListener('click', function (e) {
        if (e.target === overlay) close();
    });

    document.querySelector('[data-fc-stop]')?.addEventListener('click', function (e) {
        e.stopPropagation();
    });
})();
