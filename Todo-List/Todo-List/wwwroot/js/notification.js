const alertDiv = document.getElementById("alertOverlay");
if (alertDiv) {
    setTimeout(() => {
        alertDiv.style.transition = "opacity 0.5s";
        alertDiv.style.opacity = 0;
        setTimeout(() => alertDiv.remove(), 500); 
    }, 2500);
}