(function () {
    function deleteMessage() {
        var loc = window.fcLocale && window.fcLocale.categories;
        return (loc && loc.deleteConfirm) || 'Delete this category?';
    }

    document.querySelectorAll('.js-category-delete').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (form.getAttribute('data-fc-confirm') || (window.FcDialog && window.FcDialog.confirm)) return;
            if (!confirm(deleteMessage())) {
                e.preventDefault();
            }
        });
    });
})();
