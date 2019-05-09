using System;

namespace ChuvsuEvents.ViewModels {
    public class SearchAuditViewModel {
        public DateTime Data { get; set; }
        public string Building { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public bool Projector { get; set; }
        public bool Computer { get; set; }
    }
}
