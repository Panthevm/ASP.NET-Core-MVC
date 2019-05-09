using System;
using System.ComponentModel.DataAnnotations;

namespace ChuvsuEvents.ViewModels {
    public class NewsViewModel {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Дата публикации")]
        public DateTime PubDate { get; set; }

        public string UrlImage { get; set; }

        public string Organization { get; set; }
    }
}






