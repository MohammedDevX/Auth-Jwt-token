using System.ComponentModel.DataAnnotations;
using User_service.Enums;

namespace User_service.ViewModels
{
    public class UserRegisterVM
    {
        public string NomUser { get; set; }
        public string Email { get; set; }
        public string MotPasse { get; set; }
        public Role Role { get; set; }
    }
}
