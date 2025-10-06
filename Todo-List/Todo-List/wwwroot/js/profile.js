const menuLinks = document.querySelectorAll('.menu a[data-target]');
const forms = document.querySelectorAll('.form-section');

menuLinks.forEach(link => {
    link.addEventListener('click', function (e) {
        e.preventDefault();

        menuLinks.forEach(l => l.classList.remove('active'));
        this.classList.add('active');

        forms.forEach(f => f.classList.remove('active'));

        const target = this.getAttribute('data-target');
        document.getElementById(target).classList.add('active');
    });
});


const avatar = document.getElementById('avatar');
const avatarInput = document.getElementById('avatarInput');

avatar.addEventListener('click', () => {
    avatarInput.click();
});

avatarInput.addEventListener('change', () => {
    const file = avatarInput.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = (e) => {
            avatar.src = e.target.result;
        };
        reader.readAsDataURL(file);
    }
});

