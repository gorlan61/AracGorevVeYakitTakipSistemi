// Theme toggle: the initial theme is already applied by an inline script in <head>
// (before first paint) to avoid a flash of the wrong theme. This script only keeps
// the toggle button's icon in sync and handles clicks.
(function () {
    const root = document.documentElement;
    const toggle = document.getElementById('theme-toggle');
    const icon = document.getElementById('theme-icon');
    const STORAGE_KEY = 'site-theme';

    function setIcon(theme) {
        if (!icon) return;
        icon.className = 'bi ' + (theme === 'dark' ? 'bi-moon-stars-fill' : 'bi-sun-fill');
    }

    function applyTheme(theme) {
        if (theme !== 'dark' && theme !== 'light') return;
        root.setAttribute('data-bs-theme', theme);
        setIcon(theme);
        if (toggle) {
            toggle.setAttribute('aria-label', theme === 'dark' ? 'Açık temaya geç' : 'Koyu temaya geç');
            toggle.setAttribute('title', theme === 'dark' ? 'Açık temaya geç' : 'Koyu temaya geç');
        }
    }

    // Sync icon with the theme the inline <head> script already applied.
    setIcon(root.getAttribute('data-bs-theme') || 'light');

    if (toggle) {
        toggle.addEventListener('click', function () {
            const current = root.getAttribute('data-bs-theme') === 'dark' ? 'dark' : 'light';
            const next = current === 'dark' ? 'light' : 'dark';
            localStorage.setItem(STORAGE_KEY, next);
            applyTheme(next);
        });
    }

    // Keep in sync with OS theme changes, unless the user has explicitly chosen one.
    if (window.matchMedia) {
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function (e) {
            if (localStorage.getItem(STORAGE_KEY)) return;
            applyTheme(e.matches ? 'dark' : 'light');
        });
    }
})();

// =============================================================================
// SweetAlert2 — Silme Onay Helper
// Kullanım: <a href="..." class="btn-sil-onayla" data-isim="Plaka veya Ad">
//           veya <form class="form-sil-onayla"> içinde submit butonu
// =============================================================================
document.addEventListener('DOMContentLoaded', function () {

    // Tüm "Sil" linklerini SweetAlert2 onay pop-up'ına bağla
    document.querySelectorAll('.btn-sil-onayla').forEach(function (link) {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            var href = this.getAttribute('href');
            var isim = this.getAttribute('data-isim') || 'bu kaydı';

            Swal.fire({
                title: 'Silmek istediğinizden emin misiniz?',
                html: '<strong>' + isim + '</strong> kalıcı olarak silinecektir.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc2626',
                cancelButtonColor: '#6b7280',
                confirmButtonText: '<i class="bi bi-trash me-1"></i>Evet, Sil',
                cancelButtonText: 'Vazgeç',
                reverseButtons: true,
                customClass: {
                    confirmButton: 'btn btn-danger btn-sm',
                    cancelButton: 'btn btn-secondary btn-sm me-2'
                },
                buttonsStyling: false
            }).then(function (result) {
                if (result.isConfirmed) {
                    window.location.href = href;
                }
            });
        });
    });

    // TempData mesajlarını SweetAlert2 toast olarak göster
    var basariMesaji = document.getElementById('tempdata-basari');
    var hataMesaji = document.getElementById('tempdata-hata');
    var uyariMesaji = document.getElementById('tempdata-uyari');

    if (basariMesaji) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'success',
            title: basariMesaji.textContent.trim(),
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
    }

    if (hataMesaji) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'error',
            title: hataMesaji.textContent.trim(),
            showConfirmButton: false,
            timer: 4000,
            timerProgressBar: true
        });
    }

    if (uyariMesaji) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'warning',
            title: uyariMesaji.textContent.trim(),
            showConfirmButton: false,
            timer: 5000,
            timerProgressBar: true
        });
    }

    // Silme formları için genel SweetAlert2 onay yakalayıcısı
    // Kullanım: <form class="form-sil-onayla">
    document.querySelectorAll('.form-sil-onayla').forEach(function(form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            Swal.fire({
                title: 'Kalıcı olarak silinecek',
                text: 'Bu işlemi geri alamazsınız. Onaylıyor musunuz?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc2626',
                cancelButtonColor: '#6b7280',
                confirmButtonText: '<i class="bi bi-trash me-1"></i>Evet, Sil',
                cancelButtonText: 'Vazgeç',
                reverseButtons: true,
                customClass: {
                    confirmButton: 'btn btn-danger',
                    cancelButton: 'btn btn-secondary me-2'
                },
                buttonsStyling: false
            }).then((result) => {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
        });
    });
});

// =============================================================================
// DataTables — Türkçe Dil Yapılandırması
// Her sayfada class="datatable-auto" olan tablolara otomatik uygulanır
// =============================================================================
var datatablesTurkce = {
    decimal: ',',
    thousands: '.',
    processing: 'Yükleniyor...',
    search: 'Ara:',
    lengthMenu: '_MENU_ kayıt göster',
    info: '_TOTAL_ kayıttan _START_ ile _END_ arası gösteriliyor',
    infoEmpty: 'Kayıt bulunamadı',
    infoFiltered: '(_MAX_ toplam kayıttan filtrelendi)',
    loadingRecords: 'Yükleniyor...',
    zeroRecords: 'Eşleşen kayıt bulunamadı',
    emptyTable: 'Tabloda veri yok',
    paginate: {
        first: '«',
        previous: '‹',
        next: '›',
        last: '»'
    },
    aria: {
        orderable: 'Sıralamak için tıklayın',
        orderableReverse: 'Ters sıralamak için tıklayın'
    }
};

document.addEventListener('DOMContentLoaded', function () {
    if (typeof DataTable !== 'undefined') {
        document.querySelectorAll('table.datatable-auto').forEach(function (tablo) {
            new DataTable(tablo, {
                language: datatablesTurkce,
                pageLength: 10,
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, 'Tümü']],
                order: [],
                columnDefs: [
                    { orderable: false, targets: -1 }  // Son sütun (İşlemler) sıralanamaz
                ],
                responsive: true
            });
        });
    }
});
