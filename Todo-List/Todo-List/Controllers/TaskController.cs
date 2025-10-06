using Microsoft.AspNetCore.Mvc;
using Todo_List.Models;

namespace Todo_List.Controllers
{
    [Route("task")]
    public class TaskController : Controller
    {
        private readonly TodoListContext db = new TodoListContext();

        [HttpGet("login")]
        public IActionResult Login()
        {
            ViewBag.IsRegister = false;
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsRegister = true;
                return View("Login", user);
            }
            if (db.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                ViewBag.IsRegister = true;
                return View("Login", user);
            }
            if (db.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                ViewBag.IsRegister = true;
                return View("Login", user);
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.Now;
            user.ConfirmPassword = null;

            db.Users.Add(user);
            db.SaveChanges();

            return RedirectToAction("Login", "Task");
        }
        [HttpGet("todolist")]
        public IActionResult TodoList()
        {
            return View();
        }
        [HttpGet("calender")]
        public IActionResult Calender()
        {
            return View();
        }
        [HttpGet("add")]
        public IActionResult Add()
        {
            return View();
        }
        [HttpGet("edit")]
        public IActionResult Edit()
        {
            return View();
        }
        [HttpGet("detail")]
        public IActionResult Detail()
        {
            return View();
        }
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return View();
        }
    }
}
