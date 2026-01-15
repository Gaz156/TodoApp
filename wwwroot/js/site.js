// Notes App JavaScript

document.addEventListener('DOMContentLoaded', function () {
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });

    // Form validation enhancement
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...';
            }
        });
    });

    // Auto-resize textareas
    const textareas = document.querySelectorAll('textarea[data-autoresize]');
    textareas.forEach(textarea => {
        textarea.addEventListener('input', function () {
            this.style.height = 'auto';
            this.style.height = (this.scrollHeight) + 'px';
        });
        // Trigger initial resize
        textarea.dispatchEvent(new Event('input'));
    });

    // Confirm before delete
    const deleteForms = document.querySelectorAll('form[data-confirm]');
    deleteForms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const message = this.getAttribute('data-confirm') || 'Are you sure?';
            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    });

    // Copy to clipboard functionality
    const copyButtons = document.querySelectorAll('[data-copy]');
    copyButtons.forEach(button => {
        button.addEventListener('click', function () {
            const targetId = this.getAttribute('data-copy');
            const target = document.getElementById(targetId);
            if (target) {
                navigator.clipboard.writeText(target.textContent)
                    .then(() => {
                        const originalText = this.innerHTML;
                        this.innerHTML = '<i class="bi bi-check"></i> Copied!';
                        setTimeout(() => {
                            this.innerHTML = originalText;
                        }, 2000);
                    });
            }
        });
    });

    // Character counter for textareas
    const charCountTextareas = document.querySelectorAll('[data-char-count]');
    charCountTextareas.forEach(textarea => {
        const counterId = textarea.getAttribute('data-char-count');
        const counter = document.getElementById(counterId);
        if (counter) {
            const updateCounter = () => {
                const remaining = textarea.maxLength - textarea.value.length;
                counter.textContent = `${textarea.value.length} / ${textarea.maxLength} characters`;
                counter.className = remaining < 50 ? 'text-danger' : 'text-muted';
            };
            textarea.addEventListener('input', updateCounter);
            updateCounter();
        }
    });
});

// Utility functions
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

function showToast(message, type = 'info') {
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type} border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');

    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;

    document.body.appendChild(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();

    toast.addEventListener('hidden.bs.toast', function () {
        document.body.removeChild(toast);
    });
}