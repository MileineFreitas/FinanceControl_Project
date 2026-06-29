(function () {
    var overlay = null;
    var dialogEl = null;
    var activeResolve = null;
    var queue = [];

    function dlg() {
        return (window.fcLocale && window.fcLocale.dialog) || {};
    }

    var icons = {
        info: 'i',
        success: '✓',
        error: '!',
        confirm: '?'
    };

    function defaultTitles() {
        var d = dlg();
        return {
            info: d.info || 'Information',
            success: d.success || 'Success',
            error: d.error || 'Attention',
            confirm: d.confirm || 'Confirm'
        };
    }

    function ensureOverlay() {
        if (overlay) return;

        overlay = document.createElement('div');
        overlay.id = 'fc-dialog-overlay';
        overlay.className = 'fc-dialog-overlay';
        overlay.setAttribute('role', 'presentation');
        overlay.setAttribute('aria-hidden', 'true');
        overlay.innerHTML =
            '<div class="fc-dialog" role="dialog" aria-modal="true" aria-labelledby="fc-dialog-title" aria-describedby="fc-dialog-message">' +
            '  <div class="fc-dialog__icon" id="fc-dialog-icon" aria-hidden="true"></div>' +
            '  <h2 class="fc-dialog__title" id="fc-dialog-title"></h2>' +
            '  <p class="fc-dialog__message" id="fc-dialog-message"></p>' +
            '  <div class="fc-dialog__footer" id="fc-dialog-footer"></div>' +
            '</div>';

        document.body.appendChild(overlay);
        dialogEl = overlay.querySelector('.fc-dialog');

        overlay.addEventListener('click', function (e) {
            if (e.target === overlay && dialogEl.dataset.allowBackdropClose === 'true') {
                finish(false);
            }
        });

        document.addEventListener('keydown', function (e) {
            if (!overlay.classList.contains('is-visible')) return;
            if (e.key === 'Escape') finish(false);
        });
    }

    function finish(result) {
        if (!activeResolve) return;

        overlay.classList.remove('is-visible');
        overlay.setAttribute('aria-hidden', 'true');
        document.body.classList.remove('fc-dialog-open');

        var resolve = activeResolve;
        activeResolve = null;
        resolve(result);
        processQueue();
    }

    function processQueue() {
        if (activeResolve || queue.length === 0) return;
        show(queue.shift());
    }

    function enqueue(options) {
        return new Promise(function (resolve) {
            queue.push({ options: options, resolve: resolve });
            processQueue();
        });
    }

    function show(item) {
        ensureOverlay();
        activeResolve = item.resolve;

        var opts = item.options;
        var variant = opts.variant || 'info';
        var titles = defaultTitles();
        var d = dlg();
        var titleEl = overlay.querySelector('#fc-dialog-title');
        var messageEl = overlay.querySelector('#fc-dialog-message');
        var iconEl = overlay.querySelector('#fc-dialog-icon');
        var footerEl = overlay.querySelector('#fc-dialog-footer');

        dialogEl.className = 'fc-dialog fc-dialog--' + variant;
        dialogEl.dataset.allowBackdropClose = opts.showCancel ? 'true' : 'false';

        titleEl.textContent = opts.title || titles[variant] || titles.info;
        messageEl.textContent = opts.message || '';
        iconEl.textContent = opts.icon || icons[variant] || icons.info;

        footerEl.innerHTML = '';

        if (opts.showCancel) {
            var cancelBtn = document.createElement('button');
            cancelBtn.type = 'button';
            cancelBtn.className = 'fc-dialog__btn fc-dialog__btn--outline';
            cancelBtn.textContent = opts.cancelText || d.cancel || 'Cancel';
            cancelBtn.addEventListener('click', function () { finish(false); });
            footerEl.appendChild(cancelBtn);
        }

        var confirmBtn = document.createElement('button');
        confirmBtn.type = 'button';
        confirmBtn.className = 'fc-dialog__btn ' + (variant === 'error' || opts.danger ? 'fc-dialog__btn--danger' : 'fc-dialog__btn--primary');
        confirmBtn.textContent = opts.confirmText || (opts.showCancel ? (d.confirmBtn || 'Confirm') : (d.ok || 'OK'));
        confirmBtn.addEventListener('click', function () { finish(true); });
        footerEl.appendChild(confirmBtn);

        overlay.classList.add('is-visible');
        overlay.setAttribute('aria-hidden', 'false');
        document.body.classList.add('fc-dialog-open');
        window.setTimeout(function () { confirmBtn.focus(); }, 0);
    }

    function alert(message, options) {
        options = options || {};
        var d = dlg();
        return enqueue({
            message: message,
            title: options.title,
            variant: options.variant || 'info',
            icon: options.icon,
            confirmText: options.confirmText || d.ok || 'OK',
            showCancel: false
        });
    }

    function confirm(message, options) {
        options = options || {};
        var d = dlg();
        return enqueue({
            message: message,
            title: options.title,
            variant: options.variant || 'confirm',
            icon: options.icon,
            confirmText: options.confirmText || d.confirmBtn || 'Confirm',
            cancelText: options.cancelText || d.cancel || 'Cancel',
            danger: !!options.danger,
            showCancel: true
        });
    }

    function bindConfirmForms() {
        document.addEventListener('submit', function (e) {
            var form = e.target;
            if (!(form instanceof HTMLFormElement)) return;

            var msg = form.getAttribute('data-fc-confirm');
            if (!msg) return;
            if (form.dataset.fcConfirmOk === '1') {
                delete form.dataset.fcConfirmOk;
                return;
            }

            e.preventDefault();
            e.stopPropagation();

            var danger = form.getAttribute('data-fc-confirm-danger') === 'true';
            confirm(msg, { danger: danger }).then(function (ok) {
                if (!ok) return;
                form.dataset.fcConfirmOk = '1';
                if (typeof form.requestSubmit === 'function') {
                    form.requestSubmit();
                } else {
                    form.submit();
                }
            });
        }, true);

        document.addEventListener('click', function (e) {
            var btn = e.target.closest('button[data-fc-confirm], input[type="submit"][data-fc-confirm]');
            if (!btn) return;
            if (btn.dataset.fcConfirmOk === '1') {
                delete btn.dataset.fcConfirmOk;
                return;
            }

            e.preventDefault();
            e.stopPropagation();

            var msg = btn.getAttribute('data-fc-confirm');
            var danger = btn.getAttribute('data-fc-confirm-danger') === 'true';
            confirm(msg, { danger: danger }).then(function (ok) {
                if (!ok) return;
                btn.dataset.fcConfirmOk = '1';
                if (typeof btn.form?.requestSubmit === 'function') {
                    btn.form.requestSubmit(btn);
                } else {
                    btn.click();
                }
            });
        }, true);
    }

    function promotePageAlerts() {
        var selectors = [
            '.wt-alert--success',
            '.wt-alert--error',
            '.alert-box'
        ];
        var d = dlg();

        selectors.forEach(function (selector) {
            document.querySelectorAll(selector).forEach(function (el) {
                if (el.closest('.wt-modal-overlay')) return;
                if (el.classList.contains('fc-dialog-source-hidden')) return;

                var message = (el.textContent || '').trim();
                if (!message) return;

                el.classList.add('fc-dialog-source-hidden');

                var isError = el.classList.contains('wt-alert--error') || el.classList.contains('alert-box');
                alert(message, {
                    title: isError ? (d.error || 'Attention') : (d.success || 'Success'),
                    variant: isError ? 'error' : 'success'
                });
            });
        });
    }

    window.FcDialog = {
        alert: alert,
        confirm: confirm
    };

    bindConfirmForms();

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', promotePageAlerts);
    } else {
        promotePageAlerts();
    }
})();
