using ChuvsuEvents.Models;
using Microsoft.AspNetCore.Http;
using PagerApp.Models;
using System.Collections.Generic;

namespace ChuvsuEvents.ViewModels {
    public class MyViewModel {
        public IEnumerable<Audience> Locations { get; set; }
        public Audience Location { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
    public class MyViewModelEmployees {
        public IEnumerable<EmploymentAudienceViewModel> AEmployes { get; set; }
        public IEnumerable<Audience> Locations { get; set; }
        public IEnumerable<EventsViewModel> Events { get; set; }

        public EmploymentAudienceViewModel AEmploye { get; set; }
    }
    public class MyViewModelSearch {
        public IEnumerable<Audience> AudiencesMod { get; set; }
        public Audience Location { get; set; }
        public SearchAuditViewModel SearchAuditMod { get; set; }
        public EventsViewModel EventsMod { get; set; }
        public MyViewModelSearch() {
            AudiencesMod = new List<Audience>();
            SearchAuditMod = new SearchAuditViewModel();
        }
    }
    public class EventsShow {
        public IEnumerable<EventsViewModel> EventsViewMods { get; set; }
        public EventsViewModel EventsViewMod { get; set; }
    }
    public class NewsModelShow {
        public IEnumerable<NewsViewModel> NewsMode { get; set; }
        public IEnumerable<EventsViewModel> EventMode { get; set; }
    }
    public class EventDetailedModel {
        public mParticipantViewModel ParticipantMod { get; set; }
        public EventsViewModel EventMod { get; set; }
        public IEnumerable<mParticipantViewModel> Participants { get; set; }
    }
}
