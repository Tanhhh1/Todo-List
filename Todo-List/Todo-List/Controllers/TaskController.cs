using Microsoft.AspNetCore.Mvc;
using Todo_List.Models;
using Todo_List.Filters;

namespace Todo_List.Controllers
{
    [Route("task")]
    public class TaskController : Controller
    {
        private readonly TodoListContext db = new TodoListContext();

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel user)
        {
            // Tìm người dùng trong database theo Username
            var existingUser = db.Users.FirstOrDefault(u => u.Username == user.Username); 
            if (existingUser == null) // Nếu không tồn tại username
            {
                ModelState.AddModelError("Username", "Tài khoản không tồn tại.");
                return View(user);
            }
            // Kiểm tra mật khẩu bằng cách so sánh mật khẩu người dùng nhập với mật khẩu mã hóa trong DB
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password);  
            if (!isPasswordValid) // Nếu mật khẩu sai
            {
                ModelState.AddModelError("Password", "Mật khẩu không chính xác.");
                return View(user);
            }
            // Nếu đăng nhập thành công => Lưu thông tin vào session
            HttpContext.Session.SetInt32("UserId", existingUser.Id);
            HttpContext.Session.SetString("Username", existingUser.Username);
            HttpContext.Session.SetString("Email", existingUser.Email);

            return RedirectToAction("TodoList");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ dữ liệu trong session
            return RedirectToAction("Login");
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterModel user)
        {
            if (!ModelState.IsValid) // Kiểm tra hợp lệ dữ liệu (như [Required], [Email], [MinLength], ...)
            {
                return View(user);
            }
            if (db.Users.Any(u => u.Username == user.Username)) // Kiểm tra trùng username
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(user);
            }
            if (db.Users.Any(u => u.Email == user.Email)) // Kiểm tra trùng email
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(user);
            }
            var newUser = new User // Tạo đối tượng người dùng mới
            {
                Username = user.Username,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                CreatedAt = DateTime.Now
            };
            // Lưu người dùng mới vào database
            db.Users.Add(newUser);
            db.SaveChanges();

            return RedirectToAction("Login");
        }

        [AuthenticationFilter]
        [HttpGet("todolist")]
        public IActionResult TodoList()
        {
            // Lấy ID người dùng hiện tại từ session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            // Lấy danh sách task của user từ DB
            var tasks = db.Tasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Id)
                .ToList();

            return View(tasks);
        }

        [AuthenticationFilter]
        [HttpGet("search")]
        public IActionResult Search(string query, string status, string sort)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            // Lấy tất cả task của user
            var tasks = db.Tasks.Where(t => t.UserId == userId);
            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(query))
            {
                tasks = tasks.Where(t => t.Title.Contains(query));
            }
            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                status = status.Trim().ToLower();
                switch (status)
                {
                    case "in-progress":
                        tasks = tasks.Where(t => t.Status.ToLower() == "in progress");
                        break;
                    case "done":
                        tasks = tasks.Where(t => t.Status.ToLower() == "done");
                        break;
                    case "pending":
                        tasks = tasks.Where(t => t.Status.ToLower() == "pending");
                        break;
                }
            }

            var result = tasks.ToList();
            return View("TodoList", result); // Trả về view TodoList với danh sách task đã lọc
        }

        [AuthenticationFilter]
        [HttpGet("calender")]
        public IActionResult Calender()
        {
            return View();
        }

        [AuthenticationFilter]
        [HttpGet("add")]
        public IActionResult Add()
        {
            return View();
        }

        [AuthenticationFilter]
        [HttpPost("add")]
        public IActionResult Add(Todo_List.Models.Task task)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            task.UserId = userId.Value;

            if (!ModelState.IsValid)
            {
                return View(task);
            }

            task.Status = "Pending";
            task.CreatedAt = DateTime.Now;
            task.UpdatedAt = DateTime.Now;

            db.Tasks.Add(task);
            db.SaveChanges();

            return RedirectToAction("TodoList");
        }


        [AuthenticationFilter]
        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
            if (task == null)
            {
                return NotFound("Công việc không tồn tại hoặc bạn không có quyền xem.");
            }

            return View(task);
        }

        [AuthenticationFilter]
        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, Todo_List.Models.Task updatedTask)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
            if (task == null)
            {
                return NotFound("Công việc không tồn tại hoặc bạn không có quyền xem.");
            }
            if (!ModelState.IsValid)
            {
                return View(updatedTask);
            }

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.DueDate = updatedTask.DueDate;
            task.Status = updatedTask.Status;
            task.UpdatedAt = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("TodoList");
        }

        [AuthenticationFilter]
        [HttpGet("detail/{id}")]
        public IActionResult Detail(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            // Lấy task theo id và thuộc về user hiện tại
            var task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
            if (task == null)
            {
                return NotFound("Công việc không tồn tại hoặc bạn không có quyền xem.");
            }
            return View(task); 
        }

        [AuthenticationFilter]
        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
            if (task == null)
            {
                return NotFound("Công việc không tồn tại hoặc bạn không có quyền xóa.");
            }
            db.Tasks.Remove(task);
            db.SaveChanges();

            return RedirectToAction("TodoList");
        }

        [AuthenticationFilter]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return View();
        }
    }
}
