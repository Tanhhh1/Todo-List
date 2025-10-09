using Microsoft.AspNetCore.Mvc;
using Todo_List.Filters;
using Todo_List.Models;
using X.PagedList.Extensions;

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
            HttpContext.Session.SetString("Avatar", existingUser.Avatar ?? string.Empty);
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
            if (!ModelState.IsValid) return View(user);// Kiểm tra hợp lệ dữ liệu (như [Required], [Email], [MinLength], ...)
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
        public IActionResult TodoList(int pageNumber = 1, int pageSize = 8)
        {
            // Lấy ID người dùng hiện tại từ session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            // Lấy danh sách task của user và phân trang
            var tasks = db.Tasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Id)
                .ToPagedList(pageNumber, pageSize);

            return View(tasks);
        }


        [AuthenticationFilter]
        [HttpGet("search")]
        public IActionResult Search(string query, string status, string sort, int pageNumber = 1, int pageSize = 8)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            // Lấy danh sách task ban đầu
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

            // Sắp xếp
            if (!string.IsNullOrEmpty(sort))
            {
                sort = sort.Trim().ToLower();
                switch (sort)
                {
                    case "asc":
                        tasks = tasks.OrderBy(t => t.DueDate);
                        break;
                    case "desc":
                        tasks = tasks.OrderByDescending(t => t.DueDate);
                        break;
                    default:
                        tasks = tasks.OrderByDescending(t => t.Id);
                        break;
                }
            }
            // Áp dụng phân trang
            var pagedResult = tasks.ToPagedList(pageNumber, pageSize);
            // Giữ lại giá trị lọc để hiển thị lại trong View
            ViewBag.Query = query;
            ViewBag.Status = status;
            ViewBag.Sort = sort;

            return View("TodoList", pagedResult);
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
            if (userId == null) return RedirectToAction("Login");
            task.UserId = userId.Value; 
            if (!ModelState.IsValid) return View(task);

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
            if (userId == null) return RedirectToAction("Login");
            var task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
            if (task == null) return NotFound("Công việc không tồn tại hoặc bạn không có quyền xem.");

            return View(task);
        }

        [AuthenticationFilter]
        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, Todo_List.Models.Task updatedTask)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
            if (task == null) return NotFound("Công việc không tồn tại hoặc bạn không có quyền xem.");
            if (!ModelState.IsValid) return View(updatedTask);

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
            if (userId == null) return RedirectToAction("Login");
            // Lấy task theo id và thuộc về user hiện tại
            var task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
            if (task == null) return NotFound("Công việc không tồn tại hoặc bạn không có quyền xem.");
            return View(task);
        }

        [AuthenticationFilter]
        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
            if (task == null) return NotFound("Công việc không tồn tại hoặc bạn không có quyền xóa.");

            db.Tasks.Remove(task);
            db.SaveChanges();

            return RedirectToAction("TodoList");
        }

        [AuthenticationFilter]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var user = db.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user == null) return RedirectToAction("Login");

            return View(BuildProfileModel(user));
        }

        [AuthenticationFilter]
        [HttpPost("profile")]
        public IActionResult Profile(UpdateProfileModel updatedUser, IFormFile? avatarFile)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var user = db.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user == null) return RedirectToAction("Login");

            // Kiểm tra validate
            if (!ModelState.IsValid)
            {
                updatedUser.Avatar = user.Avatar;
                ViewBag.ActiveTab = "account";
                return View(BuildProfileModel(user, updatedUser));
            }
            // Cập nhật thông tin user
            user.Fullname = updatedUser.Fullname;
            user.Email = updatedUser.Email;
            user.Phone = updatedUser.Phone;
            user.Address = updatedUser.Address;
            user.UpdatedAt = DateTime.Now;

            // Xử lý avatar nếu có upload
            if (avatarFile != null && avatarFile.Length > 0)
            {
                // Nếu user đã có avatar cũ, xóa file cũ để tránh rác file
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    var oldAvatar = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Avatar.TrimStart('/')); // Tạo đường dẫn tuyệt đối tới file avatar cũ
                    if (System.IO.File.Exists(oldAvatar)) // Kiểm tra file có tồn tại hay không, nếu có thì xóa
                        System.IO.File.Delete(oldAvatar);
                }
                // Tạo tên file mới duy nhất bằng GUID + giữ nguyên phần mở rộng của file gốc
                var fileName = Guid.NewGuid() + Path.GetExtension(avatarFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                // Lưu file mới lên server
                using var stream = new FileStream(filePath, FileMode.Create);
                avatarFile.CopyTo(stream);
                // Cập nhật đường dẫn avatar mới vào database (đường dẫn dùng cho view là tương đối từ wwwroot)
                user.Avatar = "/uploads/" + fileName;
                HttpContext.Session.SetString("Avatar", user.Avatar);
            }
            db.SaveChanges();
            HttpContext.Session.SetString("Email", user.Email);
            // Sau khi cập nhật thành công, trả về view chính với thông báo và dữ liệu mới
            ViewBag.ActiveTab = "account";
            ViewBag.Message = "Cập nhật thành công!";
            return View(BuildProfileModel(user));
        }

        [AuthenticationFilter]
        [HttpPost("change-password")]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var user = db.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user == null) return RedirectToAction("Login");

            // Nếu validation lỗi
            if (!ModelState.IsValid)
            {
                ViewBag.ActiveTab = "password"; 
                return View("Profile", BuildProfileModel(user, null, model));
            }
            // Cập nhật mật khẩu
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            ViewBag.Message = "Đổi mật khẩu thành công!";
            ViewBag.ActiveTab = "password";
            return View("Profile", BuildProfileModel(user));
        }

        //Factory Method/Helper Method tạo và khởi tạo một Parent ViewModel từ các dữ liệu khác nhau
        private ProfileModel BuildProfileModel(User user, UpdateProfileModel? updateProfile = null, ChangePasswordModel? changePassword = null)
        {
            return new ProfileModel
            {
                UpdateProfile = updateProfile ?? new UpdateProfileModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Avatar = user.Avatar
                },
                ChangePassword = changePassword ?? new ChangePasswordModel()
            };
        }
    }
}
