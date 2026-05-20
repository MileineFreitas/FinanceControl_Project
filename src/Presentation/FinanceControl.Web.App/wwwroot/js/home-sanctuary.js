(function () {
    document.querySelectorAll('[data-fc-filter]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var filter = btn.getAttribute('data-fc-filter');
            document.querySelectorAll('.hs-trans[data-type]').forEach(function (row) {
                var t = row.getAttribute('data-type');
                var show = filter === 'Todas' || filter === t;
                row.style.display = show ? '' : 'none';
            });
            document.querySelectorAll('[data-fc-filter]').forEach(function (b) {
                b.classList.toggle('hs-filter--active', b === btn);
            });
        });
    });
})();
