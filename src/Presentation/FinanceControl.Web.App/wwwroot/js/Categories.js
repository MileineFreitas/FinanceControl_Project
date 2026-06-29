(function () {
    document.querySelectorAll('.js-category-delete').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (!confirm('Excluir esta categoria? Transações vinculadas impedem a exclusão.')) {
                e.preventDefault();
            }
        });
    });
})();
