(function () {
    function qs(sel, root) {
        return (root || document).querySelector(sel);
    }

    /** Menu lateral */
    var sidebar = document.getElementById('fc-sidebar');
    var navRoot = document.getElementById('fc-nav-root');
    document.querySelectorAll('[data-fc-sidebar-toggle]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            if (sidebar) sidebar.classList.toggle('sidebar-collapsed');
            if (navRoot) navRoot.classList.toggle('nav-root--collapsed');
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
            if (e.key === 'Escape') setOpen(false);
        });
    }
})();
