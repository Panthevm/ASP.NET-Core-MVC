using ChuvsuEvents.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChuvsuEvents.Models {

    public class ApplicationContext : IdentityDbContext<User> {

        public DbSet<Audience> Audiences { get; set; }
        public DbSet<EmploymentAudienceViewModel> Employments { get; set; }
        public DbSet<EventsViewModel> Eventsees { get; set; }
        public DbSet<NewsViewModel> Newses { get; set; }
        public DbSet<OrganizationViewModel> Organizations { get; set; }
        public DbSet<mParticipantViewModel> ParMod { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options) {
            Database.EnsureCreated();
        }

    }
}
