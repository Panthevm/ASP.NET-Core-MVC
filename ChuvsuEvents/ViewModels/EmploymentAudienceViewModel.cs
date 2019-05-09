using System.ComponentModel.DataAnnotations;


namespace ChuvsuEvents.ViewModels {

    public enum Weekday {
        [Display(Name = "Воскресенье")] Воскресенье,
        [Display(Name = "Понедельник")] Понедельник,
        [Display(Name = "Вторник")] Вторник,
        [Display(Name = "Среда")] Среда,
        [Display(Name = "Четверг")] Четверг,
        [Display(Name = "Пятница")] Пятница,
        [Display(Name = "Суббота")] Суббота
    }

    public class EmploymentAudienceViewModel {
        public int Id { get; set; }

        [Required]
        public Weekday Weekdays { get; set; }

        [Required]
        [Display(Name = "Четность")]
        public bool Chetnost { get; set; }

        [Required]
        [Display(Name = "Корпус")]
        public string Building { get; set; }

        [Required]
        [Display(Name = "Номер аудитории")]
        public int Number { get; set; }

        [Required]
        [Display(Name = "Номер пары")]
        public int PairNum { get; set; }
    }
}
