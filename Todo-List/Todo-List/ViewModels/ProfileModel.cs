using Microsoft.AspNetCore.Mvc;

namespace Todo_List.ViewModels
{
    public class ProfileModel
    {
        public UpdateProfileModel UpdateProfile { get; set; }
        public ChangePasswordModel ChangePassword { get; set; }
    }
}
