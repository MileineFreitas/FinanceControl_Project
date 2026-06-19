(function () {
    function openModal(id) {
        var el = document.getElementById(id);
        if (el) el.classList.remove('is-hidden');
    }

    function closeModal(id) {
        var el = document.getElementById(id);
        if (el) el.classList.add('is-hidden');
    }

    document.querySelectorAll('[data-wt-modal-open]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var id = btn.getAttribute('data-wt-modal-open');
            if (id) openModal(id);
        });
    });

    document.querySelectorAll('[data-wt-close-modal]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var overlay = btn.closest('.wt-modal-overlay');
            if (overlay) overlay.classList.add('is-hidden');
        });
    });

    document.querySelectorAll('.wt-modal-overlay').forEach(function (overlay) {
        overlay.addEventListener('click', function (e) {
            if (e.target === overlay) overlay.classList.add('is-hidden');
        });
    });

    document.querySelectorAll('[data-wt-stop-propagation]').forEach(function (box) {
        box.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            document.querySelectorAll('.wt-modal-overlay:not(.is-hidden)').forEach(function (o) {
                o.classList.add('is-hidden');
            });
        }
    });
})();
