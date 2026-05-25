(function () {
    var fileInput = document.getElementById('profile-photo-file');
    var preview = document.getElementById('profile-avatar-preview');
    var nameInput = document.querySelector('[name="Input.UserName"]');
    var emailInput = document.querySelector('[name="Input.Email"]');
    var displayName = document.getElementById('profile-display-name');
    var displayEmail = document.getElementById('profile-display-email');

    if (fileInput && preview) {
        fileInput.addEventListener('change', function () {
            var file = fileInput.files && fileInput.files[0];
            if (!file) return;
            if (file.size > 2 * 1024 * 1024) {
                alert('A imagem deve ter no máximo 2 MB.');
                fileInput.value = '';
                return;
            }
            var reader = new FileReader();
            reader.onload = function (e) {
                var dataUrl = e.target && e.target.result;
                if (typeof dataUrl === 'string') {
                    preview.src = dataUrl;
                }
            };
            reader.readAsDataURL(file);
        });
    }

    function syncDisplay() {
        if (displayName && nameInput) {
            displayName.textContent = nameInput.value.trim() || 'Usuário';
        }
        if (displayEmail && emailInput) {
            displayEmail.textContent = emailInput.value.trim() || '—';
        }
    }

    if (nameInput) nameInput.addEventListener('input', syncDisplay);
    if (emailInput) emailInput.addEventListener('input', syncDisplay);
})();
