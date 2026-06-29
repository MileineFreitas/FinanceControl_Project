(function () {
    var overlay = null;
    var visible = false;

    function ensureOverlay() {
        if (overlay) return overlay;

        overlay = document.createElement('div');
        overlay.id = 'fc-loading-overlay';
        overlay.className = 'fc-loading-overlay';
        overlay.setAttribute('role', 'status');
        overlay.setAttribute('aria-live', 'polite');
        overlay.setAttribute('aria-label', 'Aguarde');
        overlay.innerHTML =
            '<div class="fc-loading-overlay__panel">' +
            '<div class="fc-loading-overlay__spinner" aria-hidden="true"></div>' +
            '<span class="fc-loading-overlay__text">Aguarde</span>' +
            '</div>';

        document.body.appendChild(overlay);
        return overlay;
    }

    function show() {
        if (visible) return;
        visible = true;
        ensureOverlay();
        overlay.classList.add('is-visible');
        overlay.setAttribute('aria-busy', 'true');
        document.body.classList.add('fc-loading-active');
    }

    function hide() {
        if (!visible) return;
        visible = false;
        if (overlay) {
            overlay.classList.remove('is-visible');
            overlay.setAttribute('aria-busy', 'false');
        }
        document.body.classList.remove('fc-loading-active');
    }

    function shouldSkipElement(el) {
        return el && el.closest('[data-fc-no-loading]') !== null;
    }

    function isNavigableLink(link) {
        var href = (link.getAttribute('href') || '').trim();
        if (!href || href === '#') return false;
        if (href.indexOf('javascript:') === 0) return false;
        if (link.hasAttribute('download')) return false;
        if (link.getAttribute('target') === '_blank') return false;
        if (link.hasAttribute('data-bs-toggle')) return false;
        if (link.hasAttribute('data-fc-mobile-nav-toggle')) return false;
        if (link.hasAttribute('data-fc-profile-toggle')) return false;
        if (link.hasAttribute('data-fc-notifications-toggle')) return false;
        if (link.hasAttribute('data-fc-sidebar-backdrop')) return false;
        if (link.hasAttribute('data-wt-modal-open')) return false;
        if (link.hasAttribute('data-wt-modal-close')) return false;
        return true;
    }

    window.FcLoading = {
        show: show,
        hide: hide
    };

    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!(form instanceof HTMLFormElement)) return;
        if (shouldSkipElement(form)) return;
        if ((form.method || 'get').toLowerCase() === 'dialog') return;
        show();
    }, true);

    document.addEventListener('click', function (e) {
        if (e.defaultPrevented) return;
        if (e.button !== 0) return;
        if (e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;

        var submitButton = e.target.closest('button[type="submit"], input[type="submit"]');
        if (submitButton && !shouldSkipElement(submitButton)) {
            var form = submitButton.form;
            if (form && !shouldSkipElement(form)) {
                show();
                return;
            }
        }

        var link = e.target.closest('a[href]');
        if (!link || shouldSkipElement(link) || !isNavigableLink(link)) return;
        show();
    }, true);

    window.addEventListener('pageshow', hide);
    document.addEventListener('DOMContentLoaded', hide);
})();
