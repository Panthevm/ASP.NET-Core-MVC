using System.ComponentModel.DataAnnotations;

namespace ChuvsuEvents.ViewModels {
    public class ChangePasswordViewModel {
        public string Id { get; set; }
        public string Email { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string OldPassword { get; set; }
    }
}
