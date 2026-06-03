(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {

        // ===== SIDEBAR OVERLAY =====
        var overlay = document.createElement('div');
        overlay.className = 'sidebar-overlay';
        document.body.appendChild(overlay);

        // ===== SIDEBAR TOGGLE =====
        var sidebar = document.getElementById('sidebar');
        var sidebarToggle = document.getElementById('sidebarToggle');
        var hamburgerBtn = document.getElementById('hamburgerBtn');

        if (sidebarToggle) {
            sidebarToggle.addEventListener('click', function () {
                sidebar.classList.toggle('collapsed');
            });
        }

        if (hamburgerBtn) {
            hamburgerBtn.addEventListener('click', function () {
                sidebar.classList.toggle('mobile-open');
                overlay.classList.toggle('active');
                document.body.style.overflow = sidebar.classList.contains('mobile-open') ? 'hidden' : '';
            });
        }

        overlay.addEventListener('click', function () {
            sidebar.classList.remove('mobile-open');
            overlay.classList.remove('active');
            document.body.style.overflow = '';
        });

        document.addEventListener('click', function (e) {
            if (sidebar && sidebar.classList.contains('mobile-open')) {
                var isSidebar = sidebar.contains(e.target);
                var isHamburger = hamburgerBtn && hamburgerBtn.contains(e.target);
                if (!isSidebar && !isHamburger) {
                    sidebar.classList.remove('mobile-open');
                    overlay.classList.remove('active');
                    document.body.style.overflow = '';
                }
            }
        });

        // ===== ACTIVE NAV LINK =====
        var currentPath = window.location.pathname.toLowerCase();
        document.querySelectorAll('.nav-item').forEach(function (link) {
            var href = link.getAttribute('href');
            if (href && currentPath.includes(href.toLowerCase()) && href !== '/') {
                link.classList.add('active');
            } else if (href === '/' && currentPath === '/') {
                link.classList.add('active');
            }
        });

        // ===== SCROLL REVEAL ANIMATION =====
        var revealElements = document.querySelectorAll('.card, .stat-card, .form-section, .detail-group, .profile-header, .delete-card');
        if (revealElements.length > 0 && 'IntersectionObserver' in window) {
            var observer = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        entry.target.style.opacity = '1';
                        entry.target.style.transform = 'translateY(0)';
                        observer.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

            revealElements.forEach(function (el) {
                if (!el.classList.contains('profile-header') && !el.classList.contains('delete-card')) {
                    el.style.opacity = '0';
                    el.style.transform = 'translateY(20px)';
                    el.style.transition = 'opacity 0.6s ease-out, transform 0.6s ease-out';
                    observer.observe(el);
                }
            });
        }

        // ===== STAT COUNTER ANIMATION =====
        var statValues = document.querySelectorAll('.stat-value');
        statValues.forEach(function (el) {
            var target = parseInt(el.textContent.trim()) || 0;
            if (target === 0) return;
            var duration = 1000;
            var startTime = null;

            function animate(timestamp) {
                if (!startTime) startTime = timestamp;
                var progress = Math.min((timestamp - startTime) / duration, 1);
                var eased = 1 - Math.pow(1 - progress, 3);
                el.textContent = Math.floor(eased * target);
                if (progress < 1) {
                    requestAnimationFrame(animate);
                } else {
                    el.textContent = target;
                }
            }

            if ('IntersectionObserver' in window) {
                var counterObserver = new IntersectionObserver(function (entries) {
                    entries.forEach(function (entry) {
                        if (entry.isIntersecting) {
                            requestAnimationFrame(animate);
                            counterObserver.unobserve(entry.target);
                        }
                    });
                }, { threshold: 0.5 });
                counterObserver.observe(el);
            } else {
                requestAnimationFrame(animate);
            }
        });

        // ===== FORM STYLING =====
        document.querySelectorAll('.form-control, .form-select').forEach(function (input) {
            input.addEventListener('focus', function () {
                this.closest('.input-group')?.classList.add('focused');
            });
            input.addEventListener('blur', function () {
                this.closest('.input-group')?.classList.remove('focused');
            });
        });

        // ===== TOAST AUTO-HIDE =====
        var statusToast = document.getElementById('statusToast');
        if (statusToast) {
            setTimeout(function () {
                var toast = bootstrap.Toast.getInstance(statusToast);
                if (toast) toast.hide();
            }, 4000);
        }

        // ===== TABLE ROW HOVER EFFECT =====
        document.querySelectorAll('.table tbody tr').forEach(function (row) {
            row.addEventListener('mouseenter', function () {
                var actions = this.querySelector('.action-btns');
                if (actions) {
                    actions.style.opacity = '1';
                }
            });
            row.addEventListener('mouseleave', function () {
                var actions = this.querySelector('.action-btns');
                if (actions) {
                    actions.style.opacity = '';
                }
            });
        });

        console.log('%c GymSystem %c v1.0 ', 'background:#059669;color:white;padding:4px 8px;border-radius:4px 0 0 4px;font-weight:bold;', 'background:#0F172A;color:white;padding:4px 8px;border-radius:0 4px 4px 0;');
    });
})();
