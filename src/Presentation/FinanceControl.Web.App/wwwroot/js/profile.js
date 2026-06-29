(function () {
    var form = document.getElementById('profile-form');
    var fileInput = document.getElementById('profile-photo-file');
    var preview = document.getElementById('profile-avatar-preview');
    var nameInput = document.querySelector('[name="Input.UserName"]');
    var emailInput = document.querySelector('[name="Input.Email"]');
    var newPasswordInput = document.querySelector('[name="Input.Password"]');
    var currentPasswordInput = document.querySelector('[name="Input.CurrentPassword"]');
    var displayName = document.getElementById('profile-display-name');
    var displayEmail = document.getElementById('profile-display-email');
    var passwordModal = document.getElementById('profile-password-modal');
    var passwordModalClose = document.getElementById('profile-password-modal-close');
    var passwordModalCancel = document.getElementById('profile-password-modal-cancel');
    var passwordModalConfirm = document.getElementById('profile-password-modal-confirm');
    var passwordModalError = document.getElementById('profile-password-modal-error');
    var passwordConfirmed = false;

    if (fileInput && preview) {
        fileInput.addEventListener('change', function () {
            var file = fileInput.files && fileInput.files[0];
            if (!file) return;
            if (file.size > 2 * 1024 * 1024) {
                if (window.FcDialog) {
                    window.FcDialog.alert('A imagem deve ter no máximo 2 MB.', { variant: 'error', title: 'Arquivo inválido' });
                }
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

    function openPasswordModal() {
        if (!passwordModal) return;
        passwordModal.classList.remove('is-hidden');
        passwordModal.setAttribute('aria-hidden', 'false');
        hidePasswordError();
        window.setTimeout(function () {
            if (currentPasswordInput) currentPasswordInput.focus();
        }, 0);
    }

    function closePasswordModal() {
        if (!passwordModal) return;
        passwordModal.classList.add('is-hidden');
        passwordModal.setAttribute('aria-hidden', 'true');
        hidePasswordError();
    }

    function showPasswordError(message) {
        if (!passwordModalError) return;
        passwordModalError.textContent = message;
        passwordModalError.classList.remove('is-hidden');
    }

    function hidePasswordError() {
        if (!passwordModalError) return;
        passwordModalError.classList.add('is-hidden');
    }

    function passwordChangeRequested() {
        return !!(newPasswordInput && newPasswordInput.value.trim());
    }

    function resetPasswordConfirmation(clearCurrentPassword) {
        passwordConfirmed = false;
        hidePasswordError();
        if (clearCurrentPassword && currentPasswordInput) {
            currentPasswordInput.value = '';
        }
    }

    if (newPasswordInput) {
        newPasswordInput.addEventListener('input', function () {
            resetPasswordConfirmation(!newPasswordInput.value.trim());
        });
    }

    if (currentPasswordInput) {
        currentPasswordInput.addEventListener('input', hidePasswordError);
    }

    [passwordModalClose, passwordModalCancel].forEach(function (button) {
        if (!button) return;
        button.addEventListener('click', function () {
            closePasswordModal();
            resetPasswordConfirmation(false);
        });
    });

    if (passwordModal) {
        passwordModal.addEventListener('click', function (event) {
            if (event.target === passwordModal) {
                closePasswordModal();
                resetPasswordConfirmation(false);
            }
        });
    }

    if (passwordModalConfirm) {
        passwordModalConfirm.addEventListener('click', function () {
            if (!currentPasswordInput || !currentPasswordInput.value.trim()) {
                showPasswordError('Informe sua senha atual para continuar.');
                return;
            }

            passwordConfirmed = true;
            closePasswordModal();

            if (form) {
                if (typeof form.requestSubmit === 'function') {
                    form.requestSubmit();
                    return;
                }

                form.submit();
            }
        });
    }

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' && passwordModal && !passwordModal.classList.contains('is-hidden')) {
            closePasswordModal();
            resetPasswordConfirmation(false);
        }
    });

    if (form) {
        form.addEventListener('submit', function (event) {
            if (!passwordChangeRequested()) {
                if (currentPasswordInput) currentPasswordInput.value = '';
                passwordConfirmed = true;
                return;
            }

            if (!passwordConfirmed) {
                event.preventDefault();
                openPasswordModal();
                return;
            }

            if (!currentPasswordInput || !currentPasswordInput.value.trim()) {
                event.preventDefault();
                openPasswordModal();
                showPasswordError('Informe sua senha atual para continuar.');
                return;
            }
        });
    }
})();
