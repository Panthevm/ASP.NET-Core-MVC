using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChuvsuEvents.Models;
using ChuvsuEvents.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChuvsuEvents.Controllers {
    public class AccountController : Controller {
        private readonly ApplicationContext db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context) {
            _userManager = userManager;
            _signInManager = signInManager;
            db = context;
        }

        [Authorize]
        public async Task<IActionResult> Index() {
            var organizations = from m in db.Organizations select m;
            var org = organizations.Where(s => (s.PersoneId.Contains(_userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.Id))).FirstOrDefault();
            var userFIO = _userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.UserFIO;
            EventsShow mymodel = new EventsShow {
                EventsViewMods = await db.Eventsees.ToListAsync()
            };
            List<EventsViewModel> EventsList = new List<EventsViewModel>();
            foreach (EventsViewModel item in mymodel.EventsViewMods) {
                if (item.PersoneId == _userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.Id) {
                    EventsList.Add(item);
                }
            }
            ViewBag.Audience = EventsList.ToList();
            ViewBag.userFIO = userFIO;
            if (org != null) {
                ViewBag.Organizations = org.Name;
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> ChangePassword() {
            var userCurrent = await GetCurrentUserAsync();

            User user = await _userManager.FindByIdAsync(userCurrent?.Id);
            if (user == null) {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            if (ModelState.IsValid) {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null) {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded) {
                        return RedirectToAction("Index", "Account");
                    }
                    else {
                        foreach (var error in result.Errors) {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Edit() {
            var userCurrent = await GetCurrentUserAsync();

            User user = await _userManager.FindByIdAsync(userCurrent?.Id);
            if (user == null) {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel { Id = user.Id, UserFIO = user.UserFIO };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model) {
            if (ModelState.IsValid) {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null) {
                    user.UserFIO = model.UserFIO;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded) {
                        return RedirectToAction("Index", "Account");
                    }
                    else {
                        foreach (var error in result.Errors) {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                User user = new User { Email = model.Email, UserName = model.Email, UserFIO = model.UserFIO };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user,"Студент");
                if (result.Succeeded) {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme);
                    EmailService emailService = new EmailService();
                    await emailService.SendEmailAsync(model.Email, "Подтверждение адреса электронной почты",
                        $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");

                    return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                }
                else {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code) {
            if (userId == null || code == null) {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null) {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null) {
                    if (!await _userManager.IsEmailConfirmedAsync(user)) {
                        ModelState.AddModelError(string.Empty, "Вы не подтвердили свой email");
                        return View(model);
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded) {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) {
                        return Redirect(model.ReturnUrl);
                    }
                    else {
                        return RedirectToAction("Index", "Account");
                    }
                }
                else {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff() {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user))) {
                    return View("ForgotPasswordConfirmation");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(model.Email, "Reset Password",
                    $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null) {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null) {
                return View("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded) {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        public async Task<IActionResult> EventEdit(int? id) {
            if (id != null) {
                EventsViewModel model = await db.Eventsees.FirstOrDefaultAsync(e => e.Id == id);

                if (model != null) {
                    return PartialView(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EventEdit(EventsViewModel mod) {

            db.Eventsees.Update(mod);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> OnPostDeleteAsync(int? id) {
            if (id != null) {
                EventsViewModel model = await db.Eventsees.FirstOrDefaultAsync(e => e.Id == id);
                if (model != null) {
                    db.Eventsees.Remove(model);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Администратор,Модератор")]
        public IActionResult SetOrganization() {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор,Модератор")]
        public async Task<IActionResult> SetOrganization(OrganizationViewModel model) {

            var organizations = from m in db.Organizations select m;
            bool dublicate = organizations.Where(s => (s.Name.Contains(model.Name))).Any();

            if (model != null && !dublicate) {

                model.PersoneId = _userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.Id;
                db.Organizations.Add(model);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else {
                return View();
            }
        }

        [Authorize(Roles = "Администратор,Модератор")]
        public async Task<IActionResult> EditOrganization() {
            OrganizationViewModel model = await db.Organizations.FirstOrDefaultAsync(e => (e.PersoneId.Contains(_userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.Id)));

            if (model != null) {
                TempData["min"] = model.Id;
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор,Модератор")]
        public async Task<IActionResult> EditOrganization(OrganizationViewModel model) {
            model.Id = (int)TempData["min"];
            model.PersoneId = _userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.Id;

            db.Organizations.Update(model);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Модератор, Администратор")]
        public async Task<IActionResult> MyEventDetailed(int? id) {
            if (id != null) {
                var participants = from m in db.ParMod select m;
                participants = participants.Where(p => (p.Event == id));
                EventsViewModel evente = await db.Eventsees.FirstOrDefaultAsync(p => p.Id == id);


                EventDetailedModel mymodel = new EventDetailedModel {
                    EventMod = evente,
                    Participants = participants

                };
                if (mymodel != null) {
                    return View(mymodel);
                } 
            }
            return NotFound();
        }

        [Authorize]
        public IActionResult MyRequest() {
            var personReq = from m in db.ParMod select m;
            personReq = personReq.Where(p => p.UserId.Contains(_userManager.FindByNameAsync(_userManager.GetUserName(User)).Result.Id));
            return View(personReq);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostDeleteSub(int? id) {
            if (id != null) {
                mParticipantViewModel model = await db.ParMod.FirstOrDefaultAsync(e => e.Id == id);
                if (model != null) {
                    db.ParMod.Remove(model);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("MyRequest");
        }
    }
}