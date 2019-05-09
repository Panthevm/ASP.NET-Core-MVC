using System;

namespace ChuvsuEvents.ViewModels {
    public class EventsViewModel {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Building { get; set; }
        public int Numer { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PersoneId { get; set; }

        public string UrlImage { get; set; }

        public bool Publish { get; set; }
    }
}
