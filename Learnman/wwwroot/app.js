// Scroll to bottom helper
window.scrollToBottom = (elementId) => {
    var element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// THEME SYSTEM
// ═══════════════════════════════════════════════════════════════════════════

const THEMES = ['light', 'dark', 'sunset', 'ocean', 'forest'];
const THEME_KEY = 'learnman-theme';

// Initialize theme on page load
(function initTheme() {
    const savedTheme = localStorage.getItem(THEME_KEY) || 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);
})();

// Set theme and save to localStorage
window.setTheme = function (theme) {
    if (THEMES.includes(theme)) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(THEME_KEY, theme);
        updateThemePickerUI(theme);
    }
}

// Get current theme
window.getTheme = function () {
    return document.documentElement.getAttribute('data-theme') || 'light';
}

// Toggle theme picker menu
window.toggleThemePicker = function () {
    const menu = document.getElementById('theme-picker-menu');
    if (menu) {
        menu.classList.toggle('open');
    }
}

// Update theme picker UI
function updateThemePickerUI(theme) {
    document.querySelectorAll('.theme-option').forEach(btn => {
        btn.classList.toggle('active', btn.dataset.theme === theme);
    });
}

// Close theme picker when clicking outside
document.addEventListener('click', function (e) {
    const picker = document.querySelector('.theme-picker');
    const menu = document.getElementById('theme-picker-menu');
    if (picker && menu && !picker.contains(e.target)) {
        menu.classList.remove('open');
    }
});
