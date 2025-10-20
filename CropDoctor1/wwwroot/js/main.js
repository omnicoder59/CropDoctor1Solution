// js/main.js
document.addEventListener('DOMContentLoaded', async () => {
    const userStatus = document.getElementById('user-status');
    const logoutBtn = document.getElementById('logout-btn');
    const authLinks = document.querySelectorAll('.requires-auth');
    const noAuthLinks = document.querySelectorAll('.no-auth');

    try {
        const response = await fetch('/api/account/me');
        if (!response.ok) throw new Error('Not logged in');
        const data = await response.json();

        if (data.isLoggedIn) {
            // User is logged in
            userStatus.textContent = `Welcome, ${data.email}`;
            logoutBtn.style.display = 'inline-block';
            authLinks.forEach(el => el.style.display = 'block');
            noAuthLinks.forEach(el => el.style.display = 'none');
        } else {
            throw new Error('Not logged in');
        }
    } catch (error) {
        // User is not logged in
        userStatus.textContent = '';
        if (logoutBtn) logoutBtn.style.display = 'none';
        authLinks.forEach(el => el.style.display = 'none');
        noAuthLinks.forEach(el => el.style.display = 'block');
    }

    if (logoutBtn) {
        logoutBtn.addEventListener('click', async () => {
            await fetch('/api/account/logout', { method: 'POST' });
            window.location.href = '/login.html'; // Redirect to login page
        });
    }
});