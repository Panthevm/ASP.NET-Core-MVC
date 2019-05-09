using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChuvsuEvents.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChuvsuEvents.ViewModels;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using PagerApp.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ChuvsuEvents.Controllers {
    [Authorize(Roles = "Администратор,Модератор")]
    public class AudienceController : Controller {
        private readonly ApplicationContext db;
        private readonly UserManager<User> _userManager;

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public AudienceController(UserManager<User> userManager, ApplicationContext context) {
            _userManager = userManager;
            db = context;
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Index(int page = 1) {
            int pageSize = 10;

            IQueryable<Audience> source = db.Audiences.OrderByDescending(i => i.Id);

            var count = await source.CountAsync();
            source.Reverse();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

            MyViewModel mymodel = new MyViewModel {
                PageViewModel = pageViewModel,
                Locations = items
            };

            ViewBag.Message = TempData["Message"];
            return View(mymodel);
        }

        [Authorize(Roles = "Администратор")]
        public IActionResult Create() {
            return View();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]//Добавление аудитории
        public async Task<IActionResult> Create(MyViewModel room) {
            var rooms = from m in db.Audiences select m;
            rooms = rooms.Where(s => (s.Building.Contains(room.Location.Building)) && (s.Number == room.Location.Number));
            if (rooms.Count() == 0) {
                db.Audiences.Add(room.Location);
                await db.SaveChangesAsync();
                TempData["Message"] = false;
            }
            else {
                TempData["Message"] = true;
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> OnPostDeleteAsync(int? id) {
            if (id != null) {
                Audience audience = await db.Audiences.FirstOrDefaultAsync(p => p.Id == id);
                if (audience != null)
                    return PartialView(audience);
            }
            return NotFound();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> OnPostDeleteAsync(Audience room) {
            if (room != null) {
                db.Audiences.Remove(room);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int? id) {
            if (id != null) {
                Audience audience = await db.Audiences.FirstOrDefaultAsync(p => p.Id == id);
                if (audience != null)
                    return PartialView(audience);
            }
            return NotFound();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]//Редактивирование аудитории
        public async Task<IActionResult> Edit(Audience room) {
            var rooms = from m in db.Audiences select m;
            rooms = rooms.Where(s => (s.Building.Contains(room.Building)) && (s.Number == room.Number) && (s.Projector == room.Projector) && (s.Computer == room.Computer));
            if (rooms.Count() == 0) {
                db.Audiences.Update(room);
                await db.SaveChangesAsync();
                TempData["Message"] = false;
            }
            else {
                TempData["Message"] = true;
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Employment() {

            MyViewModelEmployees model = new MyViewModelEmployees {
                AEmployes = await db.Employments.ToListAsync(),
                Locations = await db.Audiences.ToListAsync()
            };
            ViewBag.ErrorMassage = TempData["ErrorMassage"];
            return View(model);
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]//Добавление рассписания
        public async Task<IActionResult> Createemp(MyViewModelEmployees room) {
            var rooms = from m in db.Audiences select m;
            rooms = rooms.Where(s => (s.Building.Contains(room.AEmploye.Building)) && (s.Number == room.AEmploye.Number));
            if (rooms.Count() != 0) {
                db.Employments.Add(room.AEmploye);
                await db.SaveChangesAsync();
                TempData["ErrorMassage"] = false;
            }
            else {
                TempData["ErrorMassage"] = true;
            }

            return RedirectToAction("Employment");
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> EmpOnPostDeleteAsync(int id) {
            var product = await db.Employments.FindAsync(id);
            if (product != null) {
                db.Employments.Remove(product);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Employment");
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Empedit(int? id) {
            if (id != null) {
                EmploymentAudienceViewModel emplAudi = await db.Employments.FirstOrDefaultAsync(e => e.Id == id);

                if (emplAudi != null) {
                    return PartialView(emplAudi);
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> Empedit(EmploymentAudienceViewModel emplAudi) {

            db.Employments.Update(emplAudi);
            await db.SaveChangesAsync();
            return RedirectToAction("Employment");
        }


        public async Task<IActionResult> Occupy(MyViewModelSearch model) {

            List<Audience> freeAudits = new List<Audience>();

            MyViewModelEmployees modelforsearch = new MyViewModelEmployees {
                AEmployes = await db.Employments.ToListAsync(),
                Locations = await db.Audiences.ToListAsync(),
                Events = await db.Eventsees.ToListAsync()
            };

            bool chetnost, free;
            int Weekday = (int)model.SearchAuditMod.Data.DayOfWeek;
            GregorianCalendar cal = new GregorianCalendar();
            int weekNumber = cal.GetWeekOfYear(model.SearchAuditMod.Data, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            if (weekNumber % 2 == 1) { chetnost = true; }
            else { chetnost = false; }

            bool Intersection(int eMin, int eMax, int sMin, int sMax) {
                SortedSet<int> evente = new SortedSet<int>();
                SortedSet<int> search = new SortedSet<int>();
                for (int i = eMin; i <= eMax; i++) { evente.Add(i); }
                for (int i = sMin; i <= sMax; i++) { search.Add(i); }
                return search.Intersect(evente).Count() == 0 ? false : true;
            }

            foreach (Audience audience in modelforsearch.Locations) {

                if (String.Equals(audience.Building, model.SearchAuditMod.Building)) {
                    free = true;
                    foreach (EmploymentAudienceViewModel employ in modelforsearch.AEmployes) {
                        if (String.Equals(audience.Building, employ.Building)
                            && audience.Number == employ.Number
                            && employ.Chetnost == chetnost
                            && (int)employ.Weekdays == Weekday
                            && Intersection(employ.PairNum, employ.PairNum, model.SearchAuditMod.min, model.SearchAuditMod.max)) {
                            free = false;
                        }
                    }

                    foreach (EventsViewModel even in modelforsearch.Events) {
                        if (String.Equals(audience.Building, even.Building)
                            && audience.Number == even.Numer
                            && even.Date == model.SearchAuditMod.Data
                            && Intersection(even.min,even.max,model.SearchAuditMod.min,model.SearchAuditMod.max)) {
                            free = false;
                        }
                    }

                    if (free) {
                        freeAudits.Add(audience);
                    }
                }
            }
            TempData["min"] = model.SearchAuditMod.min;
            TempData["max"] = model.SearchAuditMod.max;
            TempData["Data"] = model.SearchAuditMod.Data;
            TempData["Building"] = model.SearchAuditMod.Building;

            ViewBag.Success = TempData["Success"];
            model.AudiencesMod = freeAudits.ToList();
            return View(model);
        }

        public async Task<IActionResult> ReservedAudit(int? id) {
            if (id != null) {
                MyViewModelSearch model = new MyViewModelSearch();
                model.Location = await db.Audiences.FirstOrDefaultAsync(p => p.Id == id);
                model.SearchAuditMod.Building = (string)TempData["Building"];
                model.SearchAuditMod.min = (int)TempData["min"];
                model.SearchAuditMod.max = (int)TempData["max"];
                model.SearchAuditMod.Data = (DateTime)TempData["Data"];
                return PartialView(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ReservedAudit(MyViewModelSearch model) {
            model.EventsMod.PersoneId = _userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.Id;
            TempData["Success"] = true;
            db.Eventsees.Add(model.EventsMod);
            await db.SaveChangesAsync();
            return RedirectToAction("Occupy");
        }

       
    }
}
