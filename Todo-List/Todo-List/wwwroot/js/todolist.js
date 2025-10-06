document.querySelectorAll('.dropdown-toggle').forEach(btn => {
    btn.addEventListener('click', e => {
        e.preventDefault();
        e.stopPropagation();

        const dropdown = btn.nextElementSibling;

        document.querySelectorAll('.dropdown, .dropdown-status').forEach(menu => {
            if (menu !== dropdown) menu.style.display = 'none';
        });

        dropdown.style.display =
            dropdown.style.display === 'flex' ? 'none' : 'flex';
    });
});

document.addEventListener('click', () => {
    document.querySelectorAll('.dropdown, .dropdown-status').forEach(menu => {
        menu.style.display = 'none';
    });
});
