namespace ChuvsuEvents.ViewModels {
    public class CreateUserViewModel {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserFIO { get; set; }
    }
    public class EditUserViewModel {
        public string Id { get; set; }
        public string UserFIO { get; set; }
    }
}
