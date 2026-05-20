(function () {
    var fileInput = document.querySelector('.register-form input[type=file]');
    var img = document.getElementById('fc-avatar-img');
    var placeholder = document.getElementById('fc-avatar-placeholder');
    var pwd = document.getElementById('Register_Password');
    var eye = document.getElementById('fc-toggle-pwd');
    var bar = document.getElementById('fc-strength-bar');
    var label = document.getElementById('fc-strength-label');

    fileInput?.addEventListener('change', function () {
        var f = fileInput.files && fileInput.files[0];
        if (!f || !img || !placeholder) return;
        var url = URL.createObjectURL(f);
        img.src = url;
        img.style.display = 'block';
        placeholder.style.display = 'none';
    });

    eye?.addEventListener('click', function () {
        if (!pwd) return;
        pwd.type = pwd.type === 'password' ? 'text' : 'password';
    });

    function strengthScore(val) {
        var s = 0;
        if (val.length >= 8) s++;
        if (/[A-Z]/.test(val)) s++;
        if (/\d/.test(val)) s++;
        if (/[^A-Za-z0-9]/.test(val)) s++;
        return s;
    }

    function colors(barIndex, score) {
        if (barIndex > score) return 'var(--color-border-tertiary)';
        switch (score) {
            case 1: return '#E24B4A';
            case 2: return '#EF9F27';
            case 3: return '#639922';
            case 4: return '#1D9E75';
            default: return 'var(--color-border-tertiary)';
        }
    }

    pwd?.addEventListener('input', function () {
        var val = pwd.value || '';
        var score = strengthScore(val);
        if (!bar || !label) return;
        if (!val) {
            bar.hidden = true;
            label.textContent = '';
            return;
        }
        bar.hidden = false;
        bar.querySelectorAll('span[data-bar]').forEach(function (el) {
            var i = parseInt(el.getAttribute('data-bar'), 10);
            el.style.background = colors(i, score);
        });
        label.textContent = score === 1 ? 'Fraca' : score === 2 ? 'Média' : score === 3 ? 'Boa' : score === 4 ? 'Forte' : '';
    });
})();
