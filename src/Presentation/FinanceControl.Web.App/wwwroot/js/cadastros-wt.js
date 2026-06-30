(function () {
    function openModal(id) {
        var el = document.getElementById(id);
        if (el) el.classList.remove('is-hidden');
    }

    function closeModal(id) {
        var el = document.getElementById(id);
        if (el) el.classList.add('is-hidden');
    }

    function initIconGrid(config) {
        var grid = document.getElementById(config.gridId);
        var hidden = document.getElementById(config.hiddenId);
        var dataAttr = config.dataAttr;

        if (!grid || !hidden) return;

        function selectIcon(icon) {
            if (!icon) return;

            hidden.value = icon;
            grid.querySelectorAll('[' + dataAttr + ']').forEach(function (opt) {
                var active = opt.getAttribute(dataAttr) === icon;
                opt.classList.toggle('wt-icone-opt--selected', active);
                opt.setAttribute('aria-selected', active ? 'true' : 'false');
                opt.setAttribute('aria-pressed', active ? 'true' : 'false');
            });
        }

        if (hidden.value) {
            selectIcon(hidden.value);
        } else {
            var first = grid.querySelector('[' + dataAttr + ']');
            if (first) selectIcon(first.getAttribute(dataAttr));
        }

        grid.addEventListener('click', function (e) {
            var opt = e.target.closest('[' + dataAttr + ']');
            if (!opt) return;
            e.preventDefault();
            selectIcon(opt.getAttribute(dataAttr));
        });

        grid.addEventListener('keydown', function (e) {
            if (e.key !== 'Enter' && e.key !== ' ') return;
            var opt = e.target.closest('[' + dataAttr + ']');
            if (!opt) return;
            e.preventDefault();
            selectIcon(opt.getAttribute(dataAttr));
        });
    }

    function resizeAutoTextarea(el) {
        if (!el || !el.classList.contains('wt-textarea--auto')) return;
        el.style.height = 'auto';
        var maxHeight = 128;
        el.style.height = Math.min(el.scrollHeight, maxHeight) + 'px';
        el.style.overflowY = el.scrollHeight > maxHeight ? 'auto' : 'hidden';
    }

    function initAutoTextareas(root) {
        (root || document).querySelectorAll('.wt-textarea--auto').forEach(function (ta) {
            resizeAutoTextarea(ta);
            if (ta.dataset.autoResizeBound === 'true') return;
            ta.dataset.autoResizeBound = 'true';
            ta.addEventListener('input', function () {
                resizeAutoTextarea(ta);
            });
        });
    }

    initIconGrid({
        gridId: 'payment-icon-grid',
        hiddenId: 'payment-icon-value',
        dataAttr: 'data-payment-icon'
    });

    initIconGrid({
        gridId: 'category-icon-grid',
        hiddenId: 'category-icon-value',
        dataAttr: 'data-category-icon'
    });

    initAutoTextareas(document);

    document.querySelectorAll('[data-wt-modal-open]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var id = btn.getAttribute('data-wt-modal-open');
            if (id) {
                openModal(id);
                var modal = document.getElementById(id);
                if (modal) initAutoTextareas(modal);
            }
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
