(function () {
    function deleteMessage() {
        var loc = window.fcLocale && window.fcLocale.paymentMethods;
        return (loc && loc.deleteConfirm) || 'Delete this payment method?';
    }

    document.querySelectorAll('.js-pay-delete').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (form.getAttribute('data-fc-confirm') || (window.FcDialog && window.FcDialog.confirm)) return;
            if (!confirm(deleteMessage())) {
                e.preventDefault();
            }
        });
    });
})();
