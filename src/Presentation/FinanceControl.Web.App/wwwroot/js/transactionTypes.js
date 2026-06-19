(function () {
    document.querySelectorAll('.js-type-delete').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (!confirm('Excluir este tipo de pagamento?')) {
                e.preventDefault();
            }
        });
    });

    var modal = document.getElementById('wt-type-modal');
    if (modal && !modal.classList.contains('is-hidden')) {
        modal.setAttribute('aria-hidden', 'false');
    }
})();
