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
    var notifications = document.getElementById('fc-header-notifications');

    function initHeaderDropdown(container, toggleSel, closeSel, onOpen) {
        if (!container) return;
        var toggleBtn = qs(toggleSel, container);
        var backdrop = qs(closeSel, container);

        function setOpen(open) {
            container.classList.toggle('is-open', open);
            if (toggleBtn) toggleBtn.setAttribute('aria-expanded', open ? 'true' : 'false');
            if (open && typeof onOpen === 'function') onOpen();
        }

        toggleBtn?.addEventListener('click', function (e) {
            e.stopPropagation();
            setOpen(!container.classList.contains('is-open'));
        });

        backdrop?.addEventListener('click', function () {
            setOpen(false);
        });

        container.querySelectorAll('a[href], button:not([data-fc-notifications-toggle]):not([data-fc-profile-toggle])').forEach(function (el) {
            if (el.matches('[data-fc-notifications-close], [data-fc-profile-close], [data-fc-notifications-mark-read]')) return;
            el.addEventListener('click', function () {
                setOpen(false);
            });
        });

        return setOpen;
    }

    var setProfileOpen = initHeaderDropdown(
        profile,
        '[data-fc-profile-toggle]',
        '[data-fc-profile-close]',
        function () {
            notifications?.classList.remove('is-open');
            var nToggle = qs('[data-fc-notifications-toggle]', notifications);
            if (nToggle) nToggle.setAttribute('aria-expanded', 'false');
        }
    );

    var setNotificationsOpen = initHeaderDropdown(
        notifications,
        '[data-fc-notifications-toggle]',
        '[data-fc-notifications-close]',
        function () {
            profile?.classList.remove('is-open');
            var pToggle = qs('[data-fc-profile-toggle]', profile);
            if (pToggle) pToggle.setAttribute('aria-expanded', 'false');
        }
    );

    if (notifications) {
        var badge = qs('[data-fc-notifications-badge]', notifications);
        var markReadBtn = qs('[data-fc-notifications-mark-read]', notifications);

        function updateBadge() {
            if (!badge) return;
            var unread = notifications.querySelectorAll('.header-notifications__item--unread').length;
            if (unread > 0) {
                badge.textContent = String(unread);
                badge.classList.remove('is-hidden');
            } else {
                badge.classList.add('is-hidden');
            }
        }

        markReadBtn?.addEventListener('click', function (e) {
            e.preventDefault();
            notifications.querySelectorAll('.header-notifications__item--unread').forEach(function (item) {
                item.classList.remove('header-notifications__item--unread');
                var dot = item.querySelector('.header-notifications__dot');
                if (dot) dot.remove();
            });
            updateBadge();
        });

        notifications.querySelectorAll('.header-notifications__item').forEach(function (item) {
            item.addEventListener('click', function () {
                item.classList.remove('header-notifications__item--unread');
                var dot = item.querySelector('.header-notifications__dot');
                if (dot) dot.remove();
                updateBadge();
            });
        });

        updateBadge();
    }

    if (profile || notifications) {
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                setProfileOpen?.(false);
                setNotificationsOpen?.(false);
                setMobileSidebar(false);
                closeGlobalSearch();
            }
        });
    }

    /** Menu lateral recolhível (desktop): só ícones quando recolhido */
    var collapseBtn = document.querySelector('[data-fc-sidebar-collapse]');
    var mqDesktop = window.matchMedia('(min-width: 769px)');

    function readCollapsed() {
        return localStorage.getItem('fc-sidebar-collapsed') === '1';
    }

    function setCollapsedUi(collapsed) {
        if (!pageRoot) return;
        if (!mqDesktop.matches) {
            pageRoot.classList.remove('sidebar-collapsed');
            return;
        }
        pageRoot.classList.toggle('sidebar-collapsed', collapsed);
        localStorage.setItem('fc-sidebar-collapsed', collapsed ? '1' : '0');
        if (collapseBtn) {
            collapseBtn.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
            collapseBtn.setAttribute('aria-label', collapsed ? 'Expandir menu lateral' : 'Recolher menu lateral');
            collapseBtn.setAttribute('title', collapsed ? 'Expandir menu' : 'Recolher menu');
        }
    }

    function syncSidebarCollapse() {
        if (!pageRoot) return;
        if (!mqDesktop.matches) {
            pageRoot.classList.remove('sidebar-collapsed');
            return;
        }
        setCollapsedUi(readCollapsed());
    }

    collapseBtn?.addEventListener('click', function () {
        if (!mqDesktop.matches || !pageRoot) return;
        setCollapsedUi(!pageRoot.classList.contains('sidebar-collapsed'));
    });

    syncSidebarCollapse();
    mqDesktop.addEventListener('change', syncSidebarCollapse);

    /** Busca global no header: encontra páginas do menu */
    var FC_GLOBAL_PAGES = [
        { t: 'Dashboard', u: '/home', k: 'dashboard início home painel principal' },
        { t: 'Transações', u: '/transacoes', k: 'transações lançamentos movimentações extrato' },
        { t: 'Categorias', u: '/categorias', k: 'categorias classificação etiquetas' },
        { t: 'Meios de pagamento', u: '/meios-pagamento', k: 'pagamento pix débito crédito dinheiro meios' },
        { t: 'Investimentos', u: '/dashboards/geral', k: 'investimentos gráficos portfolio ações' },
        { t: 'Relatórios', u: '/relatorios', k: 'relatórios exportar resumo' }
    ];

    var searchInput = document.getElementById('fc-global-search-input');
    var searchResults = document.getElementById('fc-global-search-results');
    var searchWrap = document.getElementById('fc-global-search-wrap');

    function closeGlobalSearch() {
        if (!searchResults) return;
        searchResults.classList.add('is-hidden');
        searchResults.innerHTML = '';
        if (searchInput) {
            searchInput.setAttribute('aria-expanded', 'false');
        }
    }

    function renderGlobalSearch(q) {
        if (!searchResults || !searchInput) return;
        var term = (q || '').trim().toLowerCase();
        if (!term) {
            closeGlobalSearch();
            return;
        }
        var matches = FC_GLOBAL_PAGES.filter(function (p) {
            return p.t.toLowerCase().indexOf(term) !== -1 || p.k.indexOf(term) !== -1;
        });
        searchResults.innerHTML = '';
        if (matches.length === 0) {
            var li0 = document.createElement('li');
            li0.className = 'fc-global-search-empty';
            li0.textContent = 'Nenhuma página encontrada.';
            searchResults.appendChild(li0);
        } else {
            matches.forEach(function (p) {
                var li = document.createElement('li');
                li.setAttribute('role', 'option');
                var a = document.createElement('a');
                a.href = p.u;
                a.textContent = p.t;
                a.addEventListener('click', function () {
                    closeGlobalSearch();
                });
                li.appendChild(a);
                searchResults.appendChild(li);
            });
        }
        searchResults.classList.remove('is-hidden');
        searchInput.setAttribute('aria-expanded', 'true');
    }

    if (searchInput && searchResults) {
        searchInput.addEventListener('input', function () {
            renderGlobalSearch(searchInput.value);
        });
        searchInput.addEventListener('focus', function () {
            if (searchInput.value.trim()) renderGlobalSearch(searchInput.value);
        });
        searchInput.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                var first = searchResults.querySelector('a[href]');
                if (first) window.location.href = first.getAttribute('href');
            }
        });
        document.addEventListener('click', function (e) {
            if (searchWrap && !searchWrap.contains(e.target)) closeGlobalSearch();
        });
    }
})();
