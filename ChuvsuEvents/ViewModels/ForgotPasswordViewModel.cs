using System.ComponentModel.DataAnnotations;

namespace ChuvsuEvents.Models {
    public class ForgotPasswordViewModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
