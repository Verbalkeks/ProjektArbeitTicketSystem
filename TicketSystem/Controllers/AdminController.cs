using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TicketSystem.Models;
using TicketSystem.Models.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TicketSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private AppDbContext _ctx;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _ctx = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Dev: Chris
        // ADMIN STARTSEITEN BEREICH \\

        public async Task<IActionResult> AdminStart()
        {
            var Tickets = await _ctx.Tickets.ToListAsync();
            var Projects = await _ctx.Projects.ToListAsync();
            var User = await _ctx.Users.ToListAsync();
            StatisticViewModel Static = new StatisticViewModel();
            foreach (var ticket in Tickets)
            {
                Static.TicketTotal += 1;
                if (ticket.ReturnStanding() != 4)
                {
                    Static.TicketOpen += 1;
                }
                else
                {
                    Static.TicketClose += 1;
                }
            }
            foreach (var Project in Projects)
            {
                Static.ProjectTotal += 1;
                if (Project.Closed)
                {
                    Static.ProjectClose += 1;
                }
                else
                {
                    Static.ProjectOpen += 1;
                }
            }
            foreach (var user in User)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    Static.Admin += 1;
                }
                else if (await _userManager.IsInRoleAsync(user,"Developer"))
                {
                    Static.Dev += 1;
                }
                else if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    Static.Sup += 1;
                }
            }
            return View("AdminStart", Static); // Startseite für den Admin
        }

        // ADMIN PROJECT BEREICH \\

        public async Task<IActionResult> ProjectsList()
        {
            var projects = await _ctx.Projects
                .OrderBy(a => a.Closed)
                .ThenBy(a => a.EndDate)
                .ToListAsync(); // Holt alle Projekte aus der Datenbank zur Auflistung im View (Sortierung nach : Closed False => End Datum
            return View("ProjectList", projects);
        }

        public IActionResult ProjectDetails(int? id) // Bekommt die ID inform von int durch die ProjectsList
        {
            if (id == null) // Checkt ob Int vll Null ist.
            {
                TempData["ProjectNotFound"] = "Die ID des Projekts konnte nicht gefunden werden.";
                return RedirectToAction(nameof(ProjectsList));
            }
            var project = _ctx.Projects
                .FirstOrDefault(a => a.Id == id); // Sucht nach dem Projekt mit der passenden Id
            if (project == null)
            {
                TempData["ProjectNotFound"] = "Das Projekt konnte nicht gefunden werden.";
                return RedirectToAction(nameof(ProjectsList));
            }
            return View("ProjectDetails", project); // Sendet das project als Model an den View
        }

        [HttpGet]
        public IActionResult ProjectCreate()
        {
            return View("ProjectCreate");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectCreate([Bind("id, Title, Description, StartDate, EndDate")] Project project)
        {
            var Names = await _ctx.Projects.ToListAsync();
            foreach (var name in Names) // Forschleife für die Validirung ob der Projektname EInzigartig ist
            {
                if (name.Title == project.Title)
                {
                    ModelState.AddModelError(nameof(project.Title), "Es gibt bereits ein Projekt mit diesem Namen!");
                }
            }    
            if (project.Title?.Length > 100) // Validirung ob der Title nicht zu lang ist
            {
                ModelState.AddModelError(nameof(project.Title), "Der Title darf nicht länger als 100 Zeichen sein!");
            }
            if (project.StartDate < DateTime.Today) // Validirung des Startdatums
            {
                ModelState.AddModelError(nameof(project.StartDate), "Das Datum darf nicht in der Vergangenheit liegen!");
            }
            if (project.EndDate < project.StartDate) // Validirung des EndDatums
            {
                ModelState.AddModelError(nameof(project.EndDate), "Das Datum darf nicht vor dem Start Datum sein!");
            }
            if (ModelState.IsValid)
            {
                _ctx.Add(project);
                await _ctx.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectsList));
            }
            return View(project);
        }

        [HttpGet]
        public async Task<IActionResult> ProjectEdit(int? id) //Bekommt Später die ID durch die Detailansicht wenn man auf Edit klickt
        {
            if (id == null) // Checkt ob Int vll Null ist.
            {
                TempData["ProjectNotFound"] = "Die ID des Projekts konnte nicht gefunden werden.";
                return RedirectToAction(nameof(ProjectsList));
            }
            var project = await _ctx.Projects
                .FirstOrDefaultAsync(a => a.Id == id); // Sucht nach dem Projekt mit der passenden Id
            if (project == null)
            {
                TempData["ProjectNotFound"] = "Das Projekt konnte nicht gefunden werden.";
                return RedirectToAction(nameof(ProjectsList));
            }
            return View("ProjectEdit", project);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectEdit(int? id, Project project)
        {
            if (id == null)
            {
                TempData["ProjectNotFound"] = "Die ID des Projekts konnte nicht gefunden werden.";
                return RedirectToAction(nameof(ProjectsList));
            }
            var existingProject = await _ctx.Projects.FindAsync(id);
            if (existingProject == null)
            {
                TempData["ProjectNotFound"] = "Das Projekt konnte nicht gefunden werden.";
                return RedirectToAction(nameof(ProjectsList));
            }
            var Names = await _ctx.Projects.ToListAsync();
            if (existingProject.Title != project.Title)
            {
                foreach (var name in Names) // Forschleife für die Validirung ob der Projektname EInzigartig ist
                {
                    if (name.Title == project.Title)
                    {
                        ModelState.AddModelError(nameof(project.Title), "Es gibt bereits ein Projekt mit diesem Namen!");
                    }
                }
            }
            if (project.Title?.Length > 100) // Validirung ob der Title nicht zu lang ist
            {
                ModelState.AddModelError(nameof(project.Title), "Der Title darf nicht länger als 100 Zeichen sein!");
            }
            if (existingProject.StartDate < DateTime.Today) // Validirung des Startdatums
            {
                if (existingProject.StartDate != project.StartDate)
                ModelState.AddModelError(nameof(project.StartDate), "Datum an bereits gestartete Projekte dürfen nicht geändert werden!");
            }
            else if (project.StartDate < DateTime.Today)
            {
                ModelState.AddModelError(nameof(project.StartDate), "Das Datum darf nicht in der Vergangenheit liegen!");
            }
            if (project.EndDate < project.StartDate) // Validirung des EndDatums
            {
                ModelState.AddModelError(nameof(project.EndDate), "Das Datum darf nicht vor dem Start Datum sein!");
            }
            if (ModelState.IsValid)
            {
                existingProject.Title = project.Title;
                existingProject.Description = project.Description;
                existingProject.StartDate = project.StartDate;
                existingProject.EndDate = project.EndDate;
                if (project.Closed == true && existingProject.Closed == false) 
                {
                    existingProject.ClosedAt = DateTime.Now;
                    var Tickets = await _ctx.Tickets.Where(a => a.AssignedProjectId == id).ToListAsync();
                    foreach (var ticket in Tickets)
                    {
                        ticket.Standing = Standings.geschlossen;
                        var currentUser = await _userManager.GetUserAsync(User);
                        ticket.ClosedBy = currentUser;
                    }
                }
                existingProject.Closed = project.Closed;
                await _ctx.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectsList));
            }
            return View("ProjectEdit", project);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectDelete(int? id)
        {
            if (id == null)
            {
                TempData["ProjectNotFound"] = "Die ID des Projekts konnte nicht gefunden werden.";
                return RedirectToAction(nameof(ProjectsList));
            }
            var project = await _ctx.Projects.FindAsync(id);
            if (project == null)
            {
                TempData["ProjectNotFound"] = "Das Projekt konnte nicht gefunden werden.";
                return RedirectToAction(nameof(ProjectsList));
            }
            var Tickets = await _ctx.Tickets.Where(a => a.AssignedProjectId == id).ToListAsync();
            foreach (var ticket in Tickets)
            {
                ticket.Standing = Standings.geschlossen;
                var currentUser = await _userManager.GetUserAsync(User);
                ticket.ClosedBy = currentUser;
                ticket.AssignedProjectId = null;
            }

            _ctx.Projects.Remove(project);
            await _ctx.SaveChangesAsync();

            return View("ProjectDelete");
        }
        // End Dev: Chris

        // Dev: Corin

        // ADMIN USER BEREICH \\
        public async Task<IActionResult> AdminUserList()
        {
            var users = await _userManager.Users.ToListAsync(); // alle user aus dem user manager zwischenspeichern
            var viewModels = new List<AdminUserListViewModel>(); // Liste deklarieren
            foreach (var user in users) // Liste füllen
            {
                var roles = await _userManager.GetRolesAsync(user); // Zwischenvariable zur Weiterverarbeitung
                viewModels.Add(new AdminUserListViewModel() // viewmodels mit Daten füllen
                {
                    ID = user.Id,
                    Name = user.Fullname,
                    Nutzername = user.UserName!,
                    Email = CheckEmailAndPhone(user.Email!),
                    Telefonnummer = CheckEmailAndPhone(user.PhoneNumber!),
                    Roles = roles.ToList() // Zwischenvariable in Liste umwandeln
                });
            }
            var result = viewModels.OrderBy(u => u.Name).ToList(); // Liste sortieren
            ViewBag.Roles = new List<string>() { "SuperAdmin", "Admin", "Developer", "User" }; // nicht drüber nachedenken, dass ich das manuell als strings weitergebe
            return View(result); // Liste ans view weitergeben
        }

        // Methode um AdminUserList lesbarer zu halten
        private string CheckEmailAndPhone(string check)
        {
            if (check.IsNullOrEmpty()) // einfacher check um keine leeren Spalten in der Übersicht zu haben
            {
                return "---";
            }
            return check;
        }

        [HttpGet]
        public async Task<IActionResult> AdminUserEdit(string userId)
        {
            // holt sich momentan aktiven User und checkt dessen Login
            var curUser = await _userManager.GetUserAsync(User);
            if (curUser == null)
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Admin/AdminUserEdit/{userId}" });
            }
            // findet und checkt den Zieluser
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
            {
                TempData["UserNotFound"] = "Der User konnte nicht gefunden werden!";
                return RedirectToAction(nameof(AdminUserList));
            }
            var userRoles = await _userManager.GetRolesAsync(user); // Rollen des Zielusers zwischenspeichern
            var curUserRoles = await _userManager.GetRolesAsync(curUser); // Rollen des momentan aktiven Users zwischenspeichern
            // wenn Zieluser SuperAdmin ist, aber momentaner nicht oder wenn Admin versucht, einen andere Admin zu bearbeiten, Abbruch
            if (userRoles.Contains("SuperAdmin") && !curUserRoles.Contains("SuperAdmin") 
                || !curUserRoles.Contains("SuperAdmin") && curUserRoles.Contains("Admin")
                && userRoles.Contains("Admin") && curUser.Id != user.Id)
            {
                TempData["NotAuthorized"] = "Sie haben keine Berechtigung, diesen Nutzer zu bearbeiten!";
                return RedirectToAction(nameof(AdminUserList));
            }
            // alle Rollen, die vom momentan aktiven User für den Zieluser bearbeitbar sein dürfen
            var filterRoles = await FilterRolesUserEdit(curUser, user);
            if (filterRoles.IsNullOrEmpty())
            {
                TempData["NoRolesFound"] = "Es gab einen Fehler bei der Validierung der Rollen, bitte bei der Systemadministration melden.";
                RedirectToAction(nameof(AdminUserList));
            }
            var model = new AdminEditUserViewModel() // erstellt ViewModel auf Basis des gefundenen Zielusers
            {
                Id = user.Id,
                Username = user.UserName!,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                Roles = filterRoles! // gibt dem viewModel alle gefilterten Rollen mit, kreuzt aber nur die an, die bei dem Zieluser schon aktiv sind
                    .OrderBy(r => r.Name)
                    //extra viewModel, damit im editView alle gefilterten Rollen angezeigt und mit den vorhandenen Rollen des Zielusers verglichen werden können
                    .Select(r => new AdminRoleCheckbox
                    {
                        RoleName = r.Name!,
                        IsSelected = userRoles.Contains(r.Name!)
                    }).ToList()
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminUserEdit(AdminEditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id); // Zieluser nochmal explizit neu laden falls sich zwischen den beiden aufrufen etwas geändert hat
            if (user == null)
            {
                TempData["UserNotFound"] = "Der User konnte nicht gefunden werden!";
                return RedirectToAction(nameof(AdminUserList));
            }
            using (var transaction = await _ctx.Database.BeginTransactionAsync()) //transaction als Verhinderung unklarer Datenänderungen
            {
                var updateSuccess = await UpdateUserInfo(user, model); //allgemeine Infos
                var passwordSuccess = await TryUpdatePassword(user, model); //Passwort
                var rolesSuccess = await UpdateUserRoles(user, model); //Rollen
                if (updateSuccess && passwordSuccess && rolesSuccess) //check ob alle drei Schritte funktioniert haben
                {
                    await transaction.CommitAsync();
                    TempData["EditSuccess"] = $"Die Bearbeitung des Accounts von {user.Fullname} wurde erfolgreich durchgeführt.";
                    return RedirectToAction(nameof(AdminUserList));
                }
                await transaction.RollbackAsync();
                return View(model);
            }
        }

        //Anfang ausgelagerte Methoden für den UserEdit
        // Methode zum Filtern der bearbeitbaren Rollen
        public async Task<List<IdentityRole>?> FilterRolesUserEdit(AppUser curUser, AppUser editUser)
        {
            var curUserRoles = await _userManager.GetRolesAsync(curUser); // Rollen des momentan aktiven Users
            var editUserRoles = await _userManager.GetRolesAsync(editUser); // Rollen des Zielusers
            if (curUserRoles.IsNullOrEmpty() || editUserRoles.IsNullOrEmpty())
            {
                TempData["NoRolesFound"] = "Es gab einen Fehler bei der Validierung der Rollen, bitte bei der Systemadministration melden.";
                RedirectToAction(nameof(AdminUserList));
            }
            var allRoles = await _roleManager.Roles.Where(r => r.Name != "SuperAdmin").ToListAsync(); //alle Rollen außer SuperAdmin
            return curUser switch
            {
                // wenn SuperAdmin sich selbst bearbeitet, wird Admin entfernt
                _ when curUserRoles.Contains("SuperAdmin") && editUserRoles.Contains("SuperAdmin") => allRoles.Where(r => r.Name != "Admin").ToList(),
                // wenn SuperAdmin jemand anderes bearbeitet, alle Rollen außer SuperAdmin
                _ when curUserRoles.Contains("SuperAdmin") => allRoles,
                // wenn Admin einen User bearbeitet, wird Admin entfernt
                _ when curUserRoles.Contains("Admin") => allRoles.Where(r => r.Name != "Admin").ToList(),
                // wenn irgendwas nicht validiertes passiert, wird eine neue Liste übergeben um vor Manipulation der Rollen zu schützen
                _ => new List<IdentityRole>()
            };
        }

        //allgemeine Infos
        private async Task<bool> UpdateUserInfo(AppUser user, AdminEditUserViewModel model)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // pattern zur Validierung von Email Adressen
            string phonePattern = @"^(\+?\d{1,3}[-./\s]?)?(\(?\d{2,4}\)?[-./\s]?)?[\d\s./-]{6,}$"; // pattern zur Validierung von Telefonnummern
            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.UserName = model.Username;
            // Validierung der Email Adresse
            if (model.Email != null && model.Email != string.Empty && !Regex.IsMatch(model.Email, emailPattern, RegexOptions.IgnoreCase))
            {
                ModelState.AddModelError("Email", "Bitte eine gültige Email angeben!");
                return false;
            }
            user.Email = model.Email;
            // Validierung der Telefonnummer
            if (model.Phone != null && model.Phone != string.Empty && !Regex.IsMatch(model.Phone, phonePattern))
            {
                ModelState.AddModelError("Phone", "Bitte eine gültige Telefonnummer angeben!");
                return false;
            }
            user.PhoneNumber = model.Phone;
            // update
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return false;
            }
            return true;
        }

        //Passwort
        private async Task<bool> TryUpdatePassword(AppUser user, AdminEditUserViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return true; //keine Passwortänderung angefragt
            }
            if (model.NewPassword != model.ConfirmPassword) // wenn Passwörter nicht übereinstimmen
            {
                ModelState.AddModelError("ConfirmPassword", "Passwörter stimmen nicht überein!");
                return false;
            }
            // token für die Berechtigung, das Passwort zurückzusetzen
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // update
            var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!resetResult.Succeeded)
            {
                foreach (var error in resetResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return false;
            }
            return true;
        }

        //Rollen
        private async Task<bool> UpdateUserRoles(AppUser user, AdminEditUserViewModel model)
        {
            var currentRoles = await _userManager.GetRolesAsync(user); //momentane Rollen vom User
            if (model.Roles == null || !model.Roles.Any(r => r.IsSelected) && currentRoles.IsNullOrEmpty()) //wenn Model keine Rollen (ausgewählt) hat
            {
                ModelState.AddModelError("Roles", "Es muss mindestens eine Rolle ausgewählt werden!");
                return false;
            }
            // Auswahl der Rollen, die im momentanen Kontext bearbeitet werden dürfen
            var relevantRoles = currentRoles.Except(await SelectRolesToNotRemove(user));
            // Rollen die in der Form angekreuzt sind
            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();
            // Rollen, die dem User hinzugefügt werden sollen (angekreuzte minus schon vorhandene)
            var rolesToAdd = selectedRoles.Except(currentRoles);
            // Rollen, die dem User abgezogen werden sollen und dürfen (schon vorhandene (gefiltert) minus angekreuzte)
            var rolesToRemove = relevantRoles.Except(selectedRoles);
            // hinzufügen
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            // entziehen
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            // nochmal validieren, dass der user auch am Ende des Prozesses noch eine Rolle hat
            var checkRoles = await _userManager.GetRolesAsync(user);
            if (checkRoles.IsNullOrEmpty())
            {
                ModelState.AddModelError("Roles", "Es muss mindestens eine Rolle ausgewählt werden!");
                return false;
            }
            if (!addResult.Succeeded || !removeResult.Succeeded)
            {
                foreach (var error in addResult.Errors.Concat(removeResult.Errors))
                {
                    ModelState.AddModelError("", error.Description);
                }
                return false;
            }
            return true;
        }

        // Auswahl von Rollen
        private async Task<List<string>> SelectRolesToNotRemove(AppUser target)
        {
            // Auswählen und Validieren des momentan aktiven Users
            var curUser = await _userManager.GetUserAsync(User);
            if (curUser == null)
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Admin/AdminUserList" });
            }
            // Auswahl der Rollen des momentan aktiven Users
            var curUserRoles = await _userManager.GetRolesAsync(curUser!);
            // Auswahl und Validierung der Rollen des Zielusers
            var targetUserRoles = await _userManager.GetRolesAsync(target);
            if (targetUserRoles.IsNullOrEmpty())
            {
                TempData["NoRolesFound"] = "Es gab einen Fehler bei der Validierung der Rollen, bitte bei der Systemadministration melden.";
                RedirectToAction(nameof(AdminUserList));
            }
            // standardmäßig darf SuperAdmin von niemandem manipuliert werden, selbst vom SuperAdmin selbst
            var result = new List<string>() { "SuperAdmin" };
            // SuperAdmin darf sich selbst nie die Adminrechte entziehen
            if (targetUserRoles.Contains("SuperAdmin"))
            {
                result.Add("Admin");
            }
            // ein Admin darf keine Adminrechte manipulieren, auch nicht bei sich selbst
            if (!curUserRoles.Contains("SuperAdmin"))
            {
                result.Add("Admin");
            }
            return result;
        }
        //Ende ausgelagerte Methoden für den UserEdit

        [HttpGet]
        public async Task<IActionResult> AdminUserCreate()
        {
            // Auswahl und Validierung des momentan aktiven Users
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Admin/AdminUserList" });
            }
            // speichere mir "leeres" ViewModel Objekt zwischen, damit ich das dem View weitergeben kann
            // Auswahl und Validierung aller hinzufügbaren Rollen
            var allRoles = await FilterRolesUserCreate(user);
            if (allRoles.IsNullOrEmpty())
            {
                TempData["NoRolesFound"] = "Es gab einen Fehler bei der Validierung der Rollen, bitte bei der Systemadministration melden.";
                RedirectToAction(nameof(AdminUserList));
            }
            var modelRoles = allRoles!
                .OrderBy(r => r.Name)
                .Select(r => new AdminRoleCheckbox
                {
                    RoleName = r.Name!,
                    IsSelected = false
                }).ToList();
            var model = new AdminCreateUserViewModel()
            {
                Username = string.Empty,
                Firstname = string.Empty,
                Lastname = string.Empty,
                Email = string.Empty,
                Phone = string.Empty,
                Password = string.Empty,
                ConfirmPassword = string.Empty,
                Roles = modelRoles
            };
            return View(model);
        }

        // Methode zum Filtern der hinzufügbaren Rollen
        public async Task<List<IdentityRole>?> FilterRolesUserCreate(AppUser curUser)
        {
            // speichert sich Rollen des momentan aktiven Users zwischen
            var curUserRoles = await _userManager.GetRolesAsync(curUser);
            if (curUserRoles.IsNullOrEmpty())
            {
                TempData["NoRolesFound"] = "Es gab einen Fehler bei der Validierung der Rollen, bitte bei der Systemadministration melden.";
                RedirectToAction(nameof(AdminUserList));
            }
            // standardmäßig darf SuperAdmin von niemandem hinzugefügt werden, selbst vom SuperAdmin selbst
            var allRoles = await _roleManager.Roles.Where(r => r.Name != "SuperAdmin").ToListAsync();
            return curUser switch
            {
                // SuperAdmin darf alle Rollen hinzufügen
                _ when curUserRoles.Contains("SuperAdmin") => allRoles,
                // Admin darf Admin nicht hinzufügen
                _ when curUserRoles.Contains("Admin") => allRoles.Where(r => r.Name != "Admin").ToList(),
                // wenn irgendwas nicht validiertes passiert, wird eine neue Liste übergeben um vor Manipulation der Rollen zu schützen
                _ => new List<IdentityRole>()
            };
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminUserCreate(AdminCreateUserViewModel model)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // pattern zur Validierung von Email Adressen
            string phonePattern = @"^(\+?\d{1,3}[-./\s]?)?(\(?\d{2,4}\)?[-./\s]?)?[\d\s./-]{6,}$"; // pattern zur Validierung von Telefonnummern
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(model.Password))
            {
                // Validierung der Email Adresse
                if (!model.Email.IsNullOrEmpty() && !Regex.IsMatch(model.Email!, emailPattern, RegexOptions.IgnoreCase))
                {
                    ModelState.AddModelError("Email", "Bitte eine gültige Email angeben!");
                    return View(model);
                }
                // Validierung der Telefonnummer
                if (!model.Phone.IsNullOrEmpty() && !Regex.IsMatch(model.Phone!, phonePattern))
                {
                    ModelState.AddModelError("Phone", "Bitte eine gültige Telefonnummer angeben!");
                    return View(model);
                }
                // Validierung der Rollen
                if (model.Roles == null || !model.Roles.Any(r => r.IsSelected))
                {
                    ModelState.AddModelError("Roles", "Es muss mindestens eine Rolle ausgewählt werden!");
                    return View(model);
                }
                // Validierung Passwort
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwörter stimmen nicht überein!");
                    return View(model);
                }
                //Erstellung & Zwischenspeicherung neuer AppUser
                var user = new AppUser()
                {
                    UserName = model.Username,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Email = model.Email ?? string.Empty,
                    PhoneNumber = model.Phone ?? string.Empty
                };
                var createResult = await _userManager.CreateAsync(user, model.Password); //AppUser -> Datenbank
                if (!createResult.Succeeded) //Überprüfung auf Probleme bei der Erstellung
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                var rolesToAdd = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList(); //Rollen, die bei der Erstellung angekreuzt waren
                var addRoleResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addRoleResult.Succeeded) //Überprüfung auf Probleme beim Hinzufügen der Rollen
                {
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                TempData["UserCreated"] = $"Der Benutzer mit dem Namen {user.Fullname} wurde erfolgreich erstellt.";
                return RedirectToAction(nameof(AdminUserList));
            }
            // Validierung Passwort & Bestätigung des Passworts
            if (model.Password == null || model.Password == string.Empty)
            {
                ModelState.AddModelError("Password", "Es muss ein Passwort eingetragen werden!");
                return View(model);
            }
            if (model.ConfirmPassword == null || model.ConfirmPassword == string.Empty)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwort zum Abgleich angeben!");
                return View(model);
            }
            TempData["WHAT?!"] = "Ein unerwarteter Fehler ist bei der Erstellung aufgetreten. Bitte wenden Sie sich an die Systemadministration.";
            return View(model);
        }
        // End Dev: Corin
    }
}