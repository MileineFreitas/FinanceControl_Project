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

    var tbody = table.querySelector('tbody');
    if (!tbody) return;

    var rows = Array.prototype.slice.call(tbody.querySelectorAll('tr[data-dash-filter]'));
    var sortButtons = Array.prototype.slice.call(table.querySelectorAll('[data-sort-key]'));
    var sortState = { key: 'date', direction: 'desc' };
    var listTotal = countEl ? (parseInt(countEl.getAttribute('data-list-total'), 10) || rows.length) : rows.length;
    var appTotal = countEl ? (parseInt(countEl.getAttribute('data-app-total'), 10) || rows.length) : rows.length;
    var elShown = countEl ? countEl.querySelector('.js-dash-shown') : null;
    var elApp = countEl ? countEl.querySelector('.js-dash-app') : null;

    rows.forEach(function (tr, index) {
        tr.setAttribute('data-sort-index', String(index));
    });

    function getSortValue(tr, key) {
        switch (key) {
            case 'date':
                return parseInt(tr.getAttribute('data-sort-date') || '0', 10);
            case 'value':
                return parseFloat(tr.getAttribute('data-sort-value') || '0');
            case 'category':
                return tr.getAttribute('data-sort-category') || '';
            case 'title':
                return tr.getAttribute('data-sort-title') || '';
            default:
                return tr.getAttribute('data-sort-index') || '0';
        }
    }

    function compareRows(a, b) {
        var aValue = getSortValue(a, sortState.key);
        var bValue = getSortValue(b, sortState.key);
        var result = 0;

        if (typeof aValue === 'string' || typeof bValue === 'string') {
            result = String(aValue).localeCompare(String(bValue), 'pt-BR');
        } else if (aValue !== bValue) {
            result = aValue > bValue ? 1 : -1;
        }

        if (result === 0) {
            var aIndex = parseInt(a.getAttribute('data-sort-index') || '0', 10);
            var bIndex = parseInt(b.getAttribute('data-sort-index') || '0', 10);
            if (aIndex !== bIndex) {
                result = aIndex > bIndex ? 1 : -1;
            }
        }

        return sortState.direction === 'asc' ? result : -result;
    }

    function sortRows() {
        rows.sort(compareRows);
        rows.forEach(function (tr) {
            tbody.appendChild(tr);
        });
    }

    function updateSortUi() {
        sortButtons.forEach(function (btn) {
            var key = btn.getAttribute('data-sort-key');
            var isActive = key === sortState.key;
            var th = btn.closest('th');
            var icon = btn.querySelector('.dash-th-sort__icon');

            btn.classList.toggle('is-active', isActive);
            if (th) {
                th.setAttribute('aria-sort', isActive ? (sortState.direction === 'asc' ? 'ascending' : 'descending') : 'none');
            }

            if (icon) {
                icon.classList.remove('is-none', 'is-asc', 'is-desc');
                icon.classList.add(isActive ? (sortState.direction === 'asc' ? 'is-asc' : 'is-desc') : 'is-none');
            }
        });
    }

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
        if (elApp) {
            elApp.textContent = String(appTotal);
        }
    }

    sortButtons.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var key = btn.getAttribute('data-sort-key');
            var defaultDirection = btn.getAttribute('data-sort-default') || 'asc';

            if (sortState.key === key) {
                sortState.direction = sortState.direction === 'asc' ? 'desc' : 'asc';
            } else {
                sortState.key = key;
                sortState.direction = defaultDirection;
            }

            sortRows();
            updateSortUi();
        });
    });

    input.addEventListener('input', applyFilter);
    sortRows();
    updateSortUi();
    applyFilter();
})();
