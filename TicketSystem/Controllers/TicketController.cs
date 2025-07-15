using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using TicketSystem.Migrations;
using TicketSystem.Models;
using TicketSystem.Models.Database;

namespace TicketSystem.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly string _uploadPath;
        private readonly string[] _fileEndings = [".apng", ".png", ".avif", ".gif", ".jpg", ".jpeg", ".jfif", ".pjpeg", 
            ".pjp", ".png", ".svg", ".webp", ".bmp", ".ico", ".cur", ".tif", ".tiff"];
        private AppDbContext _ctx;
        private UserManager<AppUser> _userManager;
        public TicketController(IWebHostEnvironment env, AppDbContext ctx, UserManager<AppUser> userManager)
        {
            _uploadPath = Path.Combine(env.WebRootPath, "uploads");
            _ctx = ctx;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> TicketStart(TicketFilterViewModel model)
        {
            var tickets = await _ctx.Tickets
                //.Where(t => t.Standing != Standings.geschlossen) Geschlossene Tickets sollen angezeigt werden UserStory 3.4
                .Where(t => model.ProjectId != null ? (t.AssignedProjectId == model.ProjectId) : true) // Filtert nach Project
                .Where(t => model.CreatorId != null ? (t.Creator.Id == model.CreatorId) : true) // Filtert nach Ersteller
                .Where(t => model.Standing != null ? (t.Standing == model.Standing) : true) // Filtert nach Standing
                .Where(t => model.AssignedUserId != null ? (t.AssignedUserId == model.AssignedUserId) : true) // Filtert nach zugewiesener
                .OrderByDescending(t => t.AssignedProject.Title) // absteigend sortiert nach Projekttitel
                .ThenByDescending(t => t.CreatedDate) // absteigend sortiert nach Erstellungsdatum
                .ToListAsync();
            var startModel = new TicketStartViewModel
            {
                Filter = model,
                TicketList = tickets
            };
            ViewBag.Projects = new SelectList(await _ctx.Projects//.Where(p => !p.Closed) <- User Story 3.4
                .ToListAsync(), "Id", "Title");
            ViewBag.Users = new SelectList(await _ctx.Users.ToListAsync(), "Id", "Fullname");
            ViewBag.Standings = new SelectList(Enum.GetNames(typeof(Standings)).ToList());
            return View(startModel);
        }
        public async Task<IActionResult> DetailsTicket(int id)
        {
            if(!await _ctx.Tickets.Select(t => t.Id).ContainsAsync(id)) // prüft ob das Ticket (noch) Existiert
            {
                TempData["TicketNotFound"] = "Das angefragte Ticket konnte nicht gefunden werden.";
                return RedirectToAction(nameof(TicketStart), new TicketFilterViewModel());
            }
            var ticket = await _ctx.Tickets
                .Include(t => t.AssignedProject) // Project
                .Include(t => t.AssignedUser) // zugewiesener User
                .Include(t => t.Creator) // Ticketersteller
                .FirstOrDefaultAsync(t => t.Id == id); // Ticketmodel mit der entsprechenden Id
            if (ticket == null)
            {
                TempData["TicketNotFound"] = "Das angefragte Ticket konnte nicht gefunden werden.";
                return RedirectToAction(nameof(TicketStart), new TicketFilterViewModel());
            }
            // hier wird das Ticket nach Blockings überprüft
            if (await TicketNotBlocked(ticket.Id) && await TicketAssigned(ticket.Id))
            {
                ticket.Standing = Standings.bearbeitung;
                await _ctx.SaveChangesAsync();
            }
            var comments = await _ctx.Comments // lädt die Kommentare für den View
                .Where(c => c.TicketId == ticket.Id)
                .Include(c => c.Author) // Autoren der Kommentare
                .Include(c => c.Ticket) // Ticket der Kommentare
                .OrderByDescending(c => c.Id) // sortiert von neu nach alt
                .ToListAsync();

            var files = await _ctx.Files // lädt die Datenbankeinträge der Bilder des Tickets
                .Where(f => f.TicketId == ticket.Id)
                .Include(f => f.User)
                .Select(f => new FileInfoViewModel() // holt sich die u.U. relevanten Infos
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FileSize = f.FileSize,
                    UserId = f.UserId,
                    User = f.User,
                    UploadedAt = f.UploadedAt,
                    TicketId = f.TicketId
                })
                .ToListAsync();

            var model = new DetailsTicketViewModel() { Ticket = ticket, Comments = comments, Files = files };
            
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadComment(Comment model)
        {
            if (model.Message.IsNullOrEmpty()) // verhindert null oder leere Kommentare
            {
                TempData["EmptyComment"] = "Keine leeren Kommentare!";
                return RedirectToAction("DetailsTicket", new { id = model.TicketId });
            }
            var user = await _userManager.GetUserAsync(User); // holt sich nochmal den aktuellen user
            if (user == null) //wenn kein Login in der aktuellen session gefunden wird
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Ticket/DetailsTicket/{model.TicketId}" });
            }
            else //im else Zweig, damit falls man zwischendurch ausgeloggt wird, eine gewisse Sicherheit besteht
            {
                model.AuthorId = user.Id;
            }
            model.CreatedAt = DateTime.Now;
            _ctx.Comments.Add(model);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(DetailsTicket), new { id = model.TicketId}); // redirect auf das Ticket, für das man den Kommentar gepostet hat
        }
        [HttpGet]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var toDelete = await _ctx.Comments.FirstOrDefaultAsync(c => c.Id == id); // holt sich den Kommentar, den man versucht, zu löschen
            if (toDelete == null) // falls Kommentar nicht gefunden werden kann
            {
                TempData["CommentNotFound"] = "Der Kommentar konnte nicht gefunden werden!";
                return RedirectToAction(nameof(TicketStart), new TicketFilterViewModel());
            }
            var returnID = toDelete.TicketId; // holt sich Id des Tickets, zu dem der Kommentar gehört
            _ctx.Comments.Remove(toDelete);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(DetailsTicket), new { id = returnID }); // redirect auf das Ticket
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFileInDetails(FileUploadViewModel model)
        {
            if (model.TicketId <= 0 || model.TicketId > await _ctx.Tickets.Select(t => t.Id).MaxAsync()) // falls die mitgegebene TicketId nicht gültig ist
            {
                TempData["TicketNotFound"] = "Eine wichtige Komponente für den Upload wurde nicht gefunden!";
                return RedirectToAction(nameof(TicketStart), new TicketFilterViewModel());
            }
            if (model.File == null || model.File.Length <= 0) // falls die Datei null oder leer ist
            {
                TempData["FileNull"] = "Datei zum Hochladen auswählen!";
                return RedirectToAction(nameof(DetailsTicket), new { id = model.TicketId }); // redirect auf das Ticket
            }
            if (model.File.Length > 48000000) // falls die Datei zu groß ist
            {
                TempData["FileTooBig"] = "Dateigröße liegt über dem Uploadlimit!";
                return RedirectToAction(nameof(DetailsTicket), new { id = model.TicketId });
            }
            var user = await _userManager.GetUserAsync(User); // holt sich aktuell eingeloggten User
            if (user == null) // falls kein User gefunden wird
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Ticket/DetailsTicket/{model.TicketId}"});
            }
            if (!_fileEndings.Contains(Path.GetExtension(model.File.FileName))) // falls die Datei keine Bilddatei ist
            {
                TempData["WrongFileType"] = "Dateityp nicht zulässig!";
                return RedirectToAction(nameof(DetailsTicket), new { id = model.TicketId });
            }
            var ticket = await _ctx.Tickets // holt sich das Ticket inklusive Projekt
                .Include(t => t.AssignedProject)
                .FirstOrDefaultAsync(t => model.TicketId == t.Id);
            if (ticket == null) // wenn Ticket nicht existiert
            {
                TempData["TicketNull"] = "Ticket konnte nicht gefunden werden!";
                return RedirectToAction(nameof(TicketStart), new TicketFilterViewModel());
            }
            var projectPath = Path.Combine(_uploadPath, ticket.AssignedProject.Title); // holt sich den Uploadpfad für das Projekt
            if (!Directory.Exists(projectPath)) // legt Uploadpfad für das Projekt an
            {
                Directory.CreateDirectory(projectPath);
                TempData["ProjectDirectoryCreated"] = $"Das Verzeichnis für das Projekt {ticket.AssignedProject.Title} wurde erfolgreich erstellt.";
            }
            var ticketPath = Path.Combine(projectPath, ticket.Title); // holt sich den Uploadpfad für das Ticket
            if (!Directory.Exists(ticketPath)) // legt Uploadpfad für das Ticket an
            {
                Directory.CreateDirectory(ticketPath);
                TempData["TicketDirectoryCreated"] = $"Das Verzeichnis für das Ticket {ticket.Title} wurde erfolgreich erstellt.";
            }
            var fileName = $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(model.File.FileName)}"; // guid für filename, vermeidung doppelter namen
            var filePath = Path.Combine(ticketPath, fileName); // holt sich den Uploadpfad für die Datei
            if (!System.IO.File.Exists(filePath)) // checkt, ob Datei mit selbem Namen schon existiert
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream); // lädt Datei in angegebenen Pfad hoch
                }
                _ctx.Add(new FileModel() // legt Infos zum späteren Abrufen in die Datenbank
                {
                    FileName = fileName,
                    FilePath = filePath,
                    FileSize = model.File.Length,
                    UploadedAt = DateTime.Now,
                    UserId = user.Id,
                    TicketId = model.TicketId
                });
                await _ctx.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsTicket), new { id = model.TicketId }); // redirect auf das Ticket
            }
            else // falls es dennoch mal passiert, astronomisch geringe chance, aber kann nicht schaden
            {
                TempData["FileNameDuplicate"] = "Eine Datei mit demselben Namen existiert bereits!";
                return RedirectToAction(nameof(DetailsTicket), new { id = model.TicketId });
            }
        }
        public async Task<IActionResult> Download(string model)
        {
            if (!string.IsNullOrEmpty(model)) // wenn string existiert und nicht leer ist
            {
                var file = await _ctx.Files // holt sich hinterlegte Daten zur Datei inklusive Ticket und Projekt
                    .Include(f => f.Ticket)
                        .ThenInclude(t => t.AssignedProject)
                    .FirstOrDefaultAsync(f => f.FileName == model);
                if (file == null || file.Ticket == null || file.Ticket.AssignedProject == null) // wenn eins der drei null ist
                {
                    TempData["TicketNotFound"] = "Eine wichtige Komponente für den Download wurde nicht gefunden!";
                    return RedirectToAction(nameof(TicketStart), new TicketFilterViewModel());
                }
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.Ticket.AssignedProject.Title,
                    file.Ticket.Title, model); // holt sich den Pfad, an dem die angefragte Datei liegt
                if (System.IO.File.Exists(filepath)) // wenn die Datei existiert
                {
                    var contType = "application/octet-stream";
                    return PhysicalFile(filepath, contType, model);
                }
                var returnId = file.TicketId; // zur Übergabe an den Redirect
                return RedirectToAction(nameof(DetailsTicket), new { id  = returnId }); // zurück zur Detailübersicht
            }
            TempData["FileNameNotFound"] = "Die Datei konnte nicht gefunden werden!";
            return RedirectToAction(nameof(TicketStart), new TicketFilterViewModel());
        }
        [HttpGet]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var toDelete = await _ctx.Files.FirstOrDefaultAsync(c => c.Id == id); // holt sich die Datei, die man versucht, zu löschen
            if (toDelete == null) // falls Datei nicht gefunden werden kann
            {
                TempData["FileNotFound"] = "Die Datei konnte nicht gefunden werden!";
                return RedirectToAction(nameof(TicketStart), new TicketFilterViewModel());
            }
            var returnID = toDelete.TicketId; // holt sich Id des Tickets, zu dem die Datei gehört
            if (System.IO.File.Exists(toDelete.FilePath)) // Löschung, wenn Ticket existiert
            {
                System.IO.File.Delete(toDelete.FilePath);
                _ctx.Files.Remove(toDelete);
                await _ctx.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsTicket), new { id = returnID }); // redirect auf das Ticket
            }
            TempData["FileNotFound"] = "Die Datei konnte nicht gefunden werden!";
            return RedirectToAction(nameof(DetailsTicket), new { id = returnID }); // redirect auf das Ticket
        }
        [HttpGet]
        public async Task<IActionResult> CreateTicket(int id)
        {

            var project = await _ctx.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (project != null && !project.Closed)
            {
                var model = new CreateTicketViewModel()
                {
                    AssignedProjectId = project.Id,
                };
                ViewData["AssignedUserId"] = new SelectList(await _ctx.Users.ToListAsync(), "Id", "Fullname"); // Eine Auswahl an Nutzern Bereit stellen
                ViewData["TicketDependencies"] = await _ctx.Tickets
                                                 .Where(t => t.AssignedProject.Id == project.Id)
                                                 .Select(t => new SelectListItem
                                                 {
                                                     Value = t.Id.ToString(),
                                                     Text = t.Title
                                                 })
                                                 .ToListAsync();
                return View(model);
            }
            TempData["TicketNotSaved"] = "Projekt ist beendet oder konnte nicht gefunden werden";
            return RedirectToAction();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketViewModel model) 
        {
            var user = await _userManager.GetUserAsync(User); // Holt aktuell eingeloggten User
            if (user == null) //Schaut ob die Sitzung noch aktiv ist wenn nicht AccessDenied
            {
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = "/Ticket/TicketStart" }); // wenn Zeit ist vielleicht eine elegantere Lösung finden mit neuem einloggen und erneutem übersenden des Formulars?
            }
            //ProjektId überprüfen und Projekt einfügen
            Project? project = await _ctx.Projects.FirstOrDefaultAsync(p => p.Id == model.AssignedProjectId);
            var assignedToProject = project != null;
            if (project != null) // Projekt darf nicht null sein
            {
                ValidateTicketAssignedProjectIsNotClosed(project);

                if (model.Title == null)
                {
                    ModelState.AddModelError("Title", "Bitte Gib dem Ticket einen Titel");
                }
                else
                {
                    await ValidateTicketNoDuplicate(model.Title, project?.Id); // überprüft ob ein Ticket mit dem Selben Namen im Project bereits existiert
                    ValidateTicketTitleLength(model.Title); // überprüft die Titellänge
                }

                var assignedUser = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == model.AssignedUserId);
                var assignedToUser = assignedUser != null;
                if (model.Description == null)
                {
                    ModelState.AddModelError("Title", "Bitte füge dem Ticket eine Beschreiung hinzu.");
                }
                else
                {
                    ValidateTicketDescriptionLength(model.Description); //überprüft die Beschreibungslänge
                }
                if (ModelState.IsValid && assignedToProject)
                {
                    


                    var ticket = new Ticket()
                    {
                        Title = model.Title!, // Titel setzen
                        Description = model.Description!, //Beschreibung setzen (null wird vorher abgefangen)
                        AssignedProject = project!, // project wird vorher abgefangen
                        AssignedProjectId = project!.Id,
                        AssignedUser = assignedToUser ? assignedUser : null,
                        AssignedUserId = assignedToUser ? assignedUser!.Id : null,
                        AssignedDate = assignedToUser ? DateTime.Now : DateTime.MinValue,
                        Creator = user, // setzt den Creator auf user
                        CreatorId = user.Id, // passende UserID dazu
                        CreatedDate = DateTime.Now, // Erstellungsdatum jetzt
                        Standing = assignedToUser ? Standings.zuteilung : Standings.offen,
                    };
                    

                    _ctx.Add(ticket); // Ticket in den Context packen
                    
                    //
                    if (!model.TicketDependencies.IsNullOrEmpty()) //hier werden die Abhängigkeiten Iteriert und in den Context gepoackt
                    {
                        foreach (var depId in model.TicketDependencies!) 
                        {
                            var ticketDependency = await _ctx.Tickets.FirstOrDefaultAsync(t => t.Id == depId);
                            if (ticketDependency != null)
                            {
                                _ctx.Add(new Dependency
                                {
                                    DependentTicket = ticket,
                                    DependentTicketId = ticket.Id,
                                    TicketDependency = ticketDependency,
                                    TicketDependencyId = ticketDependency.Id
                                });
                            }
                        }
                    }

                    await _ctx.SaveChangesAsync(); // Context speichern
                    if (assignedToUser)
                    {
                        var historyEntry = new TicketAssignHistory()
                        {
                            AssignedAt = DateTime.Now,
                            AssignedById = user.Id,
                            AssignedBy = user,
                            AssignedToId = assignedUser.Id,
                            AssignedTo = assignedUser,
                            TicketId = ticket.Id

                        };
                        _ctx.Add(historyEntry);
                        await _ctx.SaveChangesAsync();
                    }
                    var ticketId = new { id = ticket.Id };
                    TempData["TicketSaved"] = "Ticket erfolgreich gespeichert"; // kleinen Hinweis der Ticketerstellung für den User
                    return RedirectToAction(nameof(DetailsTicket), ticketId); // zurück zur Ticketliste
                }
                ViewData["AssignedUserId"] = new SelectList(await _ctx.Users.ToListAsync(), "Id", "Fullname"); // Eine Auswahl an Nutzern Bereit stellen

                ViewData["TicketDependencies"] = await _ctx.Tickets
                                                     .Where(t => t.AssignedProject.Id == project!.Id)
                                                     .Select(t => new SelectListItem
                                                     {
                                                         Value = t.Id.ToString(),
                                                         Text = t.Title
                                                     })
                                                     .ToListAsync();
                return View(model); // zurück zum Formular
            }
            TempData["TicketNotSaved"] = "Whoopsy Projekt nicht gefunden.";
            return RedirectToAction(nameof(TicketStart));
        }
        [HttpGet]
        public async Task<IActionResult> EditTicket(int? id)
        {
            if (id != null)
            {
                var ticket = await _ctx.Tickets
                .FirstOrDefaultAsync(t => t.Id == id); // Ticket mit der entsprechenden Id
                var user = await _userManager.GetUserAsync(User);
                if (ticket != null)
                {
                    if (ticket.ReturnStanding() == 4)
                    {
                        TempData["TicketClosed"] = "Bearbeitung eines geschlossenen Tickets nicht möglich";
                        return RedirectToAction(nameof(DetailsTicket), new {id = ticket.Id});
                    }
                    var blockings = await _ctx.Dependencies
                                    .Where(d => d.DependentTicketId == ticket.Id //nur Blockings des ausgewählten Tickets
                                        && d.TicketDependency.Standing != Standings.geschlossen) // nur nicht geschlossene Abhängigkeiten
                                    .Select(d => d.TicketDependency) // nur das blockierende Ticket
                                    .ToListAsync();
                    if (user != null && (user.Id == ticket.AssignedUserId ||
                        user.Id == ticket.CreatorId ||
                        User.IsInRole("Admin")))
                    {
                        EditTicketViewModel model = new EditTicketViewModel // ViewModel vorbereiten
                        {
                            Id = ticket.Id,
                            Description = ticket.Description,
                            AssignedUserId = ticket.AssignedUserId,
                            Blockings = blockings,
                            Standing = null
                        };
                        ViewData["AssignedUserId"] = new SelectList(await _ctx.Users.ToListAsync(), "Id", "Fullname"); // Eine Auswahl an Nutzern Bereit stellen
                        return View("EditTicket", model);
                    }
                    
                }
            }            
            TempData["TicketSaved"] = "Angefordertes Ticket nicht gefunden"; //Botschaft an den Nutzer falls was schiefgeht. Wird im TicketStart aufgerufen
            return RedirectToAction(nameof(TicketStart)); //zurück zu TicketStart
        }

        [HttpPost]
        public async Task<IActionResult> EditTicket(EditTicketViewModel model)
        {

            if (!ModelState.IsValid)
            {
                ViewData["AssignedUserId"] = new SelectList(await _ctx.Users.ToListAsync(), "Id", "Fullname"); // Eine Auswahl an Nutzern Bereit stellen
                return View(model);
            }
            var ticket = await _ctx.Tickets.FirstOrDefaultAsync(t => t.Id == model.Id); //ticket finden
            var user = await _userManager.GetUserAsync(User); //derzeitigen user abrufen
            var assignedUser = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == model.AssignedUserId);
            if (user != null)
            {
                var role = await _userManager.GetRolesAsync(user);

                if (ticket != null) //ticket vorhanden?
                {

                    if (user.Id == ticket.AssignedUserId || user.Id == ticket.CreatorId || role.Contains("Admin")) //User berechtigt?
                    {
                        if (ticket.ReturnStanding() != 4)
                        {
                            ticket.Description = model.Description;
                            if (ticket.AssignedUserId != model.AssignedUserId && assignedUser != null)
                            {
                                var historyEntry = new TicketAssignHistory()
                                {
                                    AssignedAt = DateTime.Now,
                                    AssignedById = user.Id,
                                    AssignedBy = user,
                                    AssignedToId = assignedUser.Id,
                                    AssignedTo = assignedUser,
                                    TicketId = ticket.Id

                                };
                                _ctx.Add(historyEntry);
                            }
                            //if (ticket.AssignedUserId != model.AssignedUserId && assignedUser != null)
                            //{
                            //    SeedTicketAssignHistory(user, assignedUser, ticket.Id);
                            //}
                            ticket.AssignedUserId = model.AssignedUserId;
                            if (ticket.AssignedUser == null && assignedUser != null)
                            {
                                ticket.AssignedDate = DateTime.Now;
                            }
                            ticket.AssignedUser = assignedUser;
                            if (ticket.ReturnStanding() == 1 && ticket.AssignedUser != null)
                            {
                                ticket.Standing = Standings.zuteilung;
                            }

                            // es folgt eine überprüfung ob das Ticket bearbeitet werden kann
                            if (await TicketNotBlocked(ticket.Id) && await TicketAssigned(ticket.Id))
                            {
                                ticket.Standing = Standings.bearbeitung;//ticket wird auf bearbeitung gesetzt
                            }
                            if (model.Standing != null && await TicketNotBlocked(ticket.Id))
                            {
                                ticket.Standing = Standings.geschlossen;
                            }

                                await _ctx.SaveChangesAsync();
                            TempData["TicketSaved"] = "Ticket erfolgreich bearbeitet";
                            return RedirectToAction(nameof(DetailsTicket), new {id = ticket.Id});
                        }
                        TempData["TicketClosed"] = "Bearbeitung eines geschlossenen Tickets nicht möglich";
                        return RedirectToAction(nameof(DetailsTicket), new {id = ticket.Id});
                    }
                    TempData["TicketNotSaved"] = "Bearbeitung nicht möglich/erlaubt";
                    return RedirectToAction(nameof(TicketStart));
                }
            }

            TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
            return RedirectToAction("AccessDenied", "Account");
            
        }

        // Zwischenmethode zum entgegen nehmen des Projekts, welche aufgerufen wird wenn der User ein Ticket erstellen möchte. 
        [HttpGet]
        public async Task<IActionResult> SelectProject()
        {
            ViewBag.Projects = new SelectList(await _ctx.Projects.Where(p => !p.Closed).ToListAsync(), "Id", "Title");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SelectProject(Project project)
        {
            var p = await _ctx.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
            if(p != null && !project.Closed)
            {
                return RedirectToAction(nameof(CreateTicket), new { id = p.Id });
            }
            TempData["TicketNotSaved"] = "Projekt ist beendet oder konnte nicht gefunden werden";
            return RedirectToAction(nameof(TicketStart));


        }

        public async Task<IActionResult> History(int id)
        {
            var ticket = await _ctx.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket != null)
            {
                var history = await _ctx.TicketAssignHistories.Where(tah => tah.TicketId == ticket.Id)
                    .Include(tah => tah.AssignedBy)
                    .Include(tah => tah.AssignedTo)
                    .ToListAsync();
                ViewData["TicketName"] = $"{ticket.Title}";
                return View(history);
            }
            TempData["TicketNotFound"] = "Ticket konnte nicht gefunden werden.";
            return RedirectToAction(nameof(TicketStart));
        }



        // es folgen ein paar Validierunsmethoden
        private void ValidateTicketDescriptionLength(string description) 
        {
            if (description.Length > 20000)
            {
                ModelState.AddModelError("Description", "Beschreibung zu lang");
            }
            else if (description.Length < 20)
            {
                ModelState.AddModelError("Description", "Beschreibung zu kurz");
            }
            
        }
        private void ValidateTicketTitleLength(string description)
        {
            if (description.Length > 50)
            {
                ModelState.AddModelError("Title", "Titel zu lang");
            }
            else if (description.Length < 3)
            {
                ModelState.AddModelError("Title", "Titel zu kurz");
            }
        }
        private async Task<bool> ValidateTicketNoDuplicate(string? title, int? projId) 
        {
            var tickets = await _ctx.Tickets.Where(t => t.Title == title).ToListAsync();
            foreach(var t in tickets)
            {
                if (t.AssignedProjectId == projId)
                {
                    ModelState.AddModelError("Title", $"Es existiert bereits ein Ticket mit dem selben Namen im selben Project");
                    return false;
                }
            }
            return true;
        }
        private bool ValidateTicketAssignedProjectIsNotClosed(Project project)
        {
            
            if (project.Closed == true)
            {
                ModelState.AddModelError("AssignedProjectId", "Projekt wurde bereits beendet.");
                return false;
            }
            return true;
        }

        private async Task<bool> TicketAssigned(int id)
        {
            var ticket = await _ctx.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket != null && ticket.ReturnStanding() == 2)
            {
                return true;
            }
            return false;
        }


        private async Task<bool> TicketNotBlocked(int id)
        {
            var ticket = await _ctx.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket != null)
            {
                var countBlockings = _ctx.Dependencies
                    .Where(d => d.DependentTicketId == ticket.Id) //Filtert alle Abhänigkeiten des aktuellen Titel
                    .Count(d => d.TicketDependency.Standing != Standings.geschlossen); // zählt nur die Abhängigkeiten die nicht geschlossen sind

                if (countBlockings == 0)
                {
                    return true;
                }
            }
            return false;
        }
        //private void SeedTicketAssignHistory(AppUser by, AppUser to, int ticketId)
        //{

        //    var historyEntry = new TicketAssignHistory()
        //    {
        //        AssignedAt = DateTime.Now,
        //        AssignedById = by.Id,
        //        AssignedBy = by,
        //        AssignedToId = to.Id,
        //        AssignedTo = to,
        //        TicketId = ticketId

        //    };
        //    _ctx.Add(historyEntry);
        //    await _ctx.SaveChangesAsync();
        //}

    }
}
