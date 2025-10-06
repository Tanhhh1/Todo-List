const calendar = document.getElementById("calendar");
const monthYear = document.getElementById("monthYear");
const selectedDateEl = document.getElementById("selectedDate");
const todoListEl = document.getElementById("todoList");
const newTaskInput = document.getElementById("newTask");
const addTaskBtn = document.getElementById("addTask");

let currentDate = new Date();
let selectedDate = null;

// Render Calendar
function renderCalendar() {
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();

    const firstDay = new Date(year, month, 1).getDay();
    const lastDate = new Date(year, month + 1, 0).getDate();

    monthYear.textContent = `${currentDate.toLocaleString("default", { month: "long" })} ${year}`;
    calendar.innerHTML = "";

    // blank cells
    for (let i = 0; i < firstDay; i++) {
        calendar.innerHTML += `<div></div>`;
    }

    for (let day = 1; day <= lastDate; day++) {
        const dateStr = `${year}-${month + 1}-${day}`;
        const div = document.createElement("div");
        div.textContent = day;

        div.addEventListener("click", () => {
            document.querySelectorAll("#calendar div").forEach(d => d.classList.remove("active"));
            div.classList.add("active");
            selectedDate = dateStr;
            loadTasks(dateStr); // gọi server lấy task
        });

        calendar.appendChild(div);
    }
}

// Prev/Next month
document.getElementById("prev").addEventListener("click", () => {
    currentDate.setMonth(currentDate.getMonth() - 1);
    renderCalendar();
});
document.getElementById("next").addEventListener("click", () => {
    currentDate.setMonth(currentDate.getMonth() + 1);
    renderCalendar();
});

// init
renderCalendar();
