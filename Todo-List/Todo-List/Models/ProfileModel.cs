using Microsoft.AspNetCore.Mvc;

namespace Todo_List.Models
{
    public class ProfileModel
    {
        public UpdateProfileModel UpdateProfile { get; set; }
        public ChangePasswordModel ChangePassword { get; set; }
    }
}
