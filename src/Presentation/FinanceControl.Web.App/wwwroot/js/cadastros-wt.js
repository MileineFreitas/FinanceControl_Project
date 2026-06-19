(function () {
    function openModal(id) {
        var el = document.getElementById(id);
        if (!el) return;
        el.classList.remove('is-hidden');
        el.setAttribute('aria-hidden', 'false');
        var focusTarget = el.querySelector('[data-wt-modal-focus]') || el.querySelector('input:not([type="hidden"]):not([disabled])');
        if (focusTarget) {
            window.setTimeout(function () {
                try {
                    focusTarget.focus();
                } catch (e) { /* ignore */ }
            }, 0);
        }
    }

    function closeModal(id) {
        var el = document.getElementById(id);
        if (el) {
            el.classList.add('is-hidden');
            el.setAttribute('aria-hidden', 'true');
        }
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
            if (overlay) {
                overlay.classList.add('is-hidden');
                overlay.setAttribute('aria-hidden', 'true');
            }
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
                o.setAttribute('aria-hidden', 'true');
            });
        }
    });
})();
