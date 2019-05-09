using System.ComponentModel.DataAnnotations;

namespace ChuvsuEvents.ViewModels {
    public class OrganizationViewModel {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Тематика")]
        public string Theme { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        public string UrlImage { get; set; }

        public string PersoneId { get; set; }
    }
}
