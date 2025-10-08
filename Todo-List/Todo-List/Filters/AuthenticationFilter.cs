using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Todo_List.Filters
{
    //AuthenticationFilter kế thừa từ ActionFilterAttribute (lớp này để kiểm tra trạng thái đăng nhập của người dùng)
    public class AuthenticationFilter : ActionFilterAttribute
    {
        // Phương thức này được gọi trước khi một Action trong Controller được thực thi
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.Session.GetString("Username"); // Lấy thông tin tên người dùng (Username) từ Session

            if (string.IsNullOrEmpty(user)) // Nếu không có dữ liệu trong session (nghĩa là người dùng chưa đăng nhập)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Controller", "Task"},
                        {"Action", "Login"}
                    });
            }

            base.OnActionExecuting(context); // Gọi phương thức gốc để tiếp tục xử lý pipeline bình thường
        }
    }
}
