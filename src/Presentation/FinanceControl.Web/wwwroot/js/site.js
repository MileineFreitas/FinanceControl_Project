(function () {
    function qs(sel, root) {
        return (root || document).querySelector(sel);
    }

    var sidebar = document.getElementById('fc-sidebar');
    var pageRoot = document.getElementById('fc-page-root');

    function setMobileSidebar(open) {
        if (!sidebar) return;
        sidebar.classList.toggle('active', open);
        pageRoot?.classList.toggle('sidebar-mobile-open', open);
    }

    document.querySelectorAll('[data-fc-mobile-nav-toggle]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var open = !sidebar?.classList.contains('active');
            setMobileSidebar(open);
        });
    });

    document.querySelectorAll('[data-fc-sidebar-backdrop]').forEach(function (el) {
        el.addEventListener('click', function () {
            setMobileSidebar(false);
        });
    });

    /** Dropdown do perfil no header */
    var profile = document.getElementById('fc-header-profile');
    if (profile) {
        var toggleBtn = qs('[data-fc-profile-toggle]', profile);
        var backdrop = qs('[data-fc-profile-close]', profile);

        function setOpen(open) {
            profile.classList.toggle('is-open', open);
            if (toggleBtn) toggleBtn.setAttribute('aria-expanded', open ? 'true' : 'false');
        }

        toggleBtn?.addEventListener('click', function (e) {
            e.stopPropagation();
            setOpen(!profile.classList.contains('is-open'));
        });

        backdrop?.addEventListener('click', function () {
            setOpen(false);
        });

        profile.querySelectorAll('.header-dropdown-item').forEach(function (link) {
            link.addEventListener('click', function () {
                setOpen(false);
            });
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                setOpen(false);
                setMobileSidebar(false);
            }
        });
    }
})();
