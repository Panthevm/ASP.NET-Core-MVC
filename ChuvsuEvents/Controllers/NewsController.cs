using ChuvsuEvents.Models;
using ChuvsuEvents.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace ChuvsuEvents.Controllers {
    public class NewsController : Controller {

        private readonly ApplicationContext db;
        private readonly UserManager<User> _userManager;

        private async Task<string> GetOrgNameAsync() {
            OrganizationViewModel org = await db.Organizations.FirstOrDefaultAsync(e => (e.PersoneId.Contains(_userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.Id)));
            return org.Name;
        }

        public NewsController(ApplicationContext context, UserManager<User> userManager) {
            _userManager = userManager;
            db = context;
        }

        public async Task<IActionResult> Index(string organization) {
            var news = from m in db.Newses select m;
            var events = from m in db.Eventsees select m;

            var orgModel = await db.Organizations.FirstOrDefaultAsync(s => s.Name.Contains(organization));

            news = news.Where(r => (r.Organization.Contains(organization)));
            events = events.Where(r => (r.PersoneId.Contains(orgModel.PersoneId) && (r.Publish == true)));

            NewsModelShow mymodel = new NewsModelShow {
                NewsMode = news,
                EventMode = events
            };

            if (mymodel != null) {
                ViewBag.Organization = organization;
                return View(mymodel);
            }
            else {
                return NotFound();
            }

        }
        [Authorize(Roles = "Администратор,Модератор")]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор,Модератор")]
        public async Task<IActionResult> Create(NewsViewModel model) {

            if (model != null) {

                model.Organization = await GetOrgNameAsync();
                model.PubDate = DateTime.Now;

                db.Newses.Add(model);
                await db.SaveChangesAsync();

                return View();
            }

            return NotFound();

        }

        public async Task<IActionResult> Detailed(int? id) {

            var model = await db.Newses.FirstOrDefaultAsync(p => p.Id == id);

            if (model != null) {
                return View(model);
            }

            return NotFound();
        }

        [Authorize(Roles = "Администратор,Модератор")]
        public async Task<IActionResult> Edit(int? id) {
            if (id != null) {
                var model = await db.Newses.FirstOrDefaultAsync(p => p.Id == id);
                if (model != null) {
                    TempData["Date"] = model.PubDate;
                    TempData["Id"] = model.Id;
                    return View(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор,Модератор")]
        public async Task<IActionResult> Edit(NewsViewModel model) {
            model.PubDate = (DateTime)TempData["Date"];
            model.Id = (int)TempData["Id"];
            model.Organization = await GetOrgNameAsync();
            db.Newses.Update(model);
            await db.SaveChangesAsync();

            return View();
        }
        public async Task<IActionResult> NewsList() {
            var posts = from m in db.Newses select m;
            NewsViewModel model = new NewsViewModel();
            model.Organization = await GetOrgNameAsync();
            posts = posts.Where(s => s.Organization.Contains(model.Organization));

            return View(posts);
        }


        public async Task<IActionResult> EventDetailed(int? id) {

            var evente = await db.Eventsees.FirstOrDefaultAsync(p => p.Id == id);

            EventDetailedModel mymodel = new EventDetailedModel {
                EventMod = evente
            };
            if (mymodel != null) {
                return View(mymodel);
            }

            return NotFound();
        }

        [Authorize]
        public IActionResult Subscribe(int? id) {
            if(id !=null) {
                TempData["Event"] = id;
                return PartialView();
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Subscribe(EventDetailedModel persone) {
            int eventId = (int)TempData["Event"];
            persone.ParticipantMod.Event = eventId;
            var partic = from m in db.ParMod select m;
            var model = await db.Eventsees.FirstOrDefaultAsync(p => p.Id == eventId);
            if (persone != null && model != null) {
                persone.ParticipantMod.EventName = model.Title;
                persone.ParticipantMod.UserId = _userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.Id;
                persone.ParticipantMod.SubDate = DateTime.Now;

                bool dublicate = partic.Where(p => (persone.ParticipantMod.Event == p.Event) && (p.UserId.Contains(persone.ParticipantMod.UserId))).Any();

                if (!dublicate) {
                    db.ParMod.Add(persone.ParticipantMod);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", "Account");
        }
    }
}
