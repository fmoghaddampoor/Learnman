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
let _currentTheme = 'light';

// Initialize theme on page load
(function initTheme() {
    const savedTheme = localStorage.getItem(THEME_KEY) || 'light';
    _currentTheme = savedTheme;
    document.documentElement.setAttribute('data-theme', savedTheme);

    // Ensure UI is updated (checkmark)
    // Defer slightly to ensure DOM is ready if script runs in head
    setTimeout(() => updateThemePickerUI(savedTheme), 100);

    // Immediate poll to sync with Tray App on startup
    pollTheme();

    // Start Polling for external changes (Tray App)
    setInterval(pollTheme, 1000);
})();

async function pollTheme() {
    try {
        const response = await fetch('/api/theme');
        if (response.ok) {
            const data = await response.json();
            if (data.theme && data.theme !== _currentTheme) {
                console.log('Syncing theme from external:', data.theme);
                // Update local without triggering sending back to server (prevent loop)
                _currentTheme = data.theme;
                document.documentElement.setAttribute('data-theme', data.theme);
                localStorage.setItem(THEME_KEY, data.theme);
                updateThemePickerUI(data.theme);
            }
        }
    } catch (e) { /* Ignore poll errors */ }
}

// Set theme and save to localStorage
window.setTheme = function (theme) {
    if (THEMES.includes(theme)) {
        _currentTheme = theme;
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(THEME_KEY, theme);
        updateThemePickerUI(theme);

        // Notify server
        fetch('/api/theme', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ theme: theme })
        }).catch(err => console.error('Failed to sync theme:', err));
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
