(function () {
    var periodBtns = document.querySelectorAll('[data-dash-period]');
    periodBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            periodBtns.forEach(function (b) { b.classList.remove('is-active'); });
            btn.classList.add('is-active');
        });
    });

    var input = document.getElementById('dash-tx-filter');
    var table = document.getElementById('dash-tx-table');
    var countEl = document.getElementById('dash-tx-count');
    if (!input || !table) return;

    var rows = table.querySelectorAll('tbody tr[data-dash-filter]');
    var listTotal = parseInt(countEl.getAttribute('data-list-total'), 10) || rows.length;
    var appTotal = parseInt(countEl.getAttribute('data-app-total'), 10) || rows.length;
    var elShown = countEl.querySelector('.js-dash-shown');

    function applyFilter() {
        var q = (input.value || '').trim().toLowerCase();
        var visible = 0;
        rows.forEach(function (tr) {
            var blob = tr.getAttribute('data-dash-filter') || '';
            var show = !q || blob.indexOf(q) !== -1;
            tr.classList.toggle('is-hidden', !show);
            if (show) visible++;
        });
        if (elShown) {
            elShown.textContent = q ? String(visible) : String(listTotal);
        }
    }

    input.addEventListener('input', applyFilter);
})();
