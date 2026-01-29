// tv-navigation.js

document.addEventListener('DOMContentLoaded', () => {
    // Initial focus
    const focusables = getFocusables();
    if (focusables.length > 0) {
        focusables[0].classList.add('focused');
        focusables[0].focus();
    }

    document.addEventListener('keydown', handleKeyNavigation);

    // Also handle mouseover to update focus state for hybrid usage
    document.addEventListener('mouseover', (e) => {
        const card = e.target.closest('.media-card, .btn-play, .episode-item, .player-control');
        if (card) {
            document.querySelectorAll('.focused').forEach(el => el.classList.remove('focused'));
            card.classList.add('focused');
            card.focus();
        }
    });
});

function getFocusables() {
    return Array.from(document.querySelectorAll('a, button, [tabindex="0"], .media-card, .episode-item, input, .btn-play'));
}

function handleKeyNavigation(e) {
    const focusables = getFocusables();
    const current = document.activeElement;
    let index = focusables.indexOf(current);

    // If lost focus, reset likely to 0 or find first visible
    if (index === -1 && focusables.length > 0) {
        // Try to find the one with .focused class
        const preFocused = document.querySelector('.focused');
        if (preFocused) {
            preFocused.focus();
            return;
        }
        index = 0;
        focusables[0].focus();
        return;
    }

    let nextIndex = index;

    // Simple Grid/List Logic
    // This is a naive implementation. For true 2D navigation (Up/Down/Left/Right) between rows, 
    // we normally calculate geometric distance. For now, we rely on DOM order mostly or 
    // structured assumption (Rows are containers).

    // Let's implement a smarter "Directional" navigation if possible, or fallback to sequential for tab.
    // Arrow Keys are usually mapped to spatial navigation.

    // We will use specialized logic: 
    // If in a row (horizontal), Left/Right moves index -1/+1
    // If Up/Down, we try to jump to the closest element in the next container vertically.

    switch (e.key) {
        case 'ArrowRight':
            navigateHorizontal(1);
            e.preventDefault();
            break;
        case 'ArrowLeft':
            navigateHorizontal(-1);
            e.preventDefault();
            break;
        case 'ArrowDown':
            navigateVertical(1);
            e.preventDefault();
            break;
        case 'ArrowUp':
            navigateVertical(-1);
            e.preventDefault();
            break;
        case 'Enter':
            if (current) current.click();
            break;
        case 'Backspace':
        case 'Escape':
            window.history.back();
            break;
    }
}

function navigateHorizontal(dir) {
    const current = document.activeElement;
    if (!current) return;

    // Is it in a scroll container?
    const container = current.closest('.row-scroller');
    if (container) {
        const items = Array.from(container.querySelectorAll('.media-card'));
        const idx = items.indexOf(current);
        const nextIdx = idx + dir;

        if (nextIdx >= 0 && nextIdx < items.length) {
            setFocus(items[nextIdx]);
        }
        return;
    }

    // General case
    const focusables = getFocusables();
    const idx = focusables.indexOf(current);
    const nextIdx = idx + dir;
    if (nextIdx >= 0 && nextIdx < focusables.length) {
        setFocus(focusables[nextIdx]);
    }
}

function navigateVertical(dir) {
    const current = document.activeElement;
    if (!current) return;

    // Find current center Y
    const rect = current.getBoundingClientRect();
    const cy = rect.top + rect.height / 2;
    const cx = rect.left + rect.width / 2;

    const all = getFocusables();
    // Filter those strictly above or below
    const candidates = all.filter(el => {
        if (el === current) return false;
        const r = el.getBoundingClientRect();
        const elCy = r.top + r.height / 2;

        if (dir > 0) return elCy > (rect.bottom - 10); // Down: below current bottom
        else return elCy < (rect.top + 10); // Up: above current top
    });

    if (candidates.length === 0) return;

    // Find closest by X alignment then Y distance
    let closest = null;
    let minDist = Infinity;

    candidates.forEach(el => {
        const r = el.getBoundingClientRect();
        const elCx = r.left + r.width / 2;
        const elCy = r.top + r.height / 2;

        // Distance
        const dx = Math.abs(cx - elCx);
        const dy = Math.abs(cy - elCy);

        // Weight Y slightly less than X to prefer vertically aligned items? 
        // Actually for "Down" we want the one visually closest. 
        // Simple Euclidean usually works fine for grid
        const dist = Math.sqrt(dx * dx + dy * dy * 4); // Penisalize vertical distance to prefer closer rows? No.

        // We prioritize elements that are "visually" aligned in X
        const weightedDist = dx + dy * 0.5;

        if (weightedDist < minDist) {
            minDist = weightedDist;
            closest = el;
        }
    });

    if (closest) {
        setFocus(closest);
    }
}

function setFocus(el) {
    document.querySelectorAll('.focused').forEach(e => e.classList.remove('focused'));
    el.classList.add('focused');
    el.focus();
    el.scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'center' });
}
