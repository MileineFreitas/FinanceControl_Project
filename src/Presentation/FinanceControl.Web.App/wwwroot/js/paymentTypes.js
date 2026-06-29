(function () {
    document.querySelectorAll('.js-pay-delete').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (!confirm('Excluir este meio de pagamento?')) {
                e.preventDefault();
            }
        });
    });
})();
