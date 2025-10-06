using Microsoft.AspNetCore.Mvc;

namespace Todo_List.Controllers
{
    public class TaskController : Controller
    {
        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet("/todolist")]
        public IActionResult TodoList()
        {
            return View();
        }
        [HttpGet("/calender")]
        public IActionResult Calender()
        {
            return View();
        }
        [HttpGet("/add")]
        public IActionResult Add()
        {
            return View();
        }
        [HttpGet("/edit")]
        public IActionResult Edit()
        {
            return View();
        }
        [HttpGet("/detail")]
        public IActionResult Detail()
        {
            return View();
        }
        [HttpGet("/profile")]
        public IActionResult Profile()
        {
            return View();
        }
    }
}
