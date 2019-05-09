using Microsoft.AspNetCore.Identity;

namespace ChuvsuEvents.Models {
    public class User : IdentityUser {
        public string UserFIO { get; set; }
    }
}
