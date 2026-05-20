(function () {
    var grid = document.getElementById('category-icon-grid');
    var hidden = document.getElementById('category-icon-value');

    function selectIcon(icon) {
        if (!hidden || !grid) return;
        hidden.value = icon;
        grid.querySelectorAll('[data-category-icon]').forEach(function (opt) {
            var active = opt.getAttribute('data-category-icon') === icon;
            opt.classList.toggle('wt-icone-opt--selected', active);
            opt.setAttribute('aria-selected', active ? 'true' : 'false');
        });
    }

    if (grid && hidden) {
        if (!hidden.value) {
            var first = grid.querySelector('[data-category-icon]');
            if (first) selectIcon(first.getAttribute('data-category-icon'));
        }

        grid.addEventListener('click', function (e) {
            var opt = e.target.closest('[data-category-icon]');
            if (!opt) return;
            e.preventDefault();
            selectIcon(opt.getAttribute('data-category-icon'));
        });

        grid.addEventListener('keydown', function (e) {
            if (e.key !== 'Enter' && e.key !== ' ') return;
            var opt = e.target.closest('[data-category-icon]');
            if (!opt) return;
            e.preventDefault();
            selectIcon(opt.getAttribute('data-category-icon'));
        });
    }

    document.querySelectorAll('.js-category-delete').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (!confirm('Excluir esta categoria? Transações vinculadas impedem a exclusão.')) {
                e.preventDefault();
            }
        });
    });
})();
