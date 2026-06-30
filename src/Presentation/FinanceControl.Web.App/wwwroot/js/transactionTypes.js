(function () {
    function deleteMessage() {
        var loc = window.fcLocale && window.fcLocale.paymentMethods;
        return (loc && loc.deleteConfirm) || 'Delete this payment method?';
    }

    document.querySelectorAll('.js-type-delete').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (form.getAttribute('data-fc-confirm') || (window.FcDialog && window.FcDialog.confirm)) return;
            if (!confirm(deleteMessage())) {
                e.preventDefault();
            }
        });
    });

    var modal = document.getElementById('wt-type-modal');
    if (modal && !modal.classList.contains('is-hidden')) {
        modal.setAttribute('aria-hidden', 'false');
    }
})();
