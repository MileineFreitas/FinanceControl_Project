(function () {

    var grid = document.getElementById('payment-icon-grid');

    var hidden = document.getElementById('payment-icon-value');



    function selectIcon(icon) {

        if (!hidden || !grid) return;

        hidden.value = icon;

        grid.querySelectorAll('[data-payment-icon]').forEach(function (opt) {

            var active = opt.getAttribute('data-payment-icon') === icon;

            opt.classList.toggle('wt-icone-opt--selected', active);

            opt.setAttribute('aria-selected', active ? 'true' : 'false');

        });

    }



    if (grid && hidden) {

        if (!hidden.value) {

            var first = grid.querySelector('[data-payment-icon]');

            if (first) selectIcon(first.getAttribute('data-payment-icon'));

        }



        grid.addEventListener('click', function (e) {

            var opt = e.target.closest('[data-payment-icon]');

            if (!opt) return;

            e.preventDefault();

            selectIcon(opt.getAttribute('data-payment-icon'));

        });



        grid.addEventListener('keydown', function (e) {

            if (e.key !== 'Enter' && e.key !== ' ') return;

            var opt = e.target.closest('[data-payment-icon]');

            if (!opt) return;

            e.preventDefault();

            selectIcon(opt.getAttribute('data-payment-icon'));

        });

    }



    document.querySelectorAll('.js-pay-delete').forEach(function (form) {

        form.addEventListener('submit', function (e) {

            if (!confirm('Excluir este meio de pagamento?')) {

                e.preventDefault();

            }

        });

    });

})();

