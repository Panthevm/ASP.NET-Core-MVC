using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChuvsuEvents.Models {
    public class Audience {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Корпус")]
        public string Building { get; set; } 

        [Required]
        [Display(Name = "Номер аудитории")]
        public int Number { get; set; } 

        public bool Projector { get; set; } 
        public bool Computer { get; set; } 
    }
}
