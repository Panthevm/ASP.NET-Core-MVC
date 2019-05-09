
using System;
using System.ComponentModel.DataAnnotations;

namespace ChuvsuEvents.ViewModels {
    public class mParticipantViewModel {
        public int Id { get; set; }

        [Display(Name = "Студент")]
        public string UserId { get; set; }

        [Display(Name = "Мероприятие")]
        public int Event { get; set; }

        [Display(Name = "Название мероприятия")]
        public string EventName { get; set; }

        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }


        [Display(Name = "Дата подачи")]
        public DateTime SubDate { get; set; }
    }
}
