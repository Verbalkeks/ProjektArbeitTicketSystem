using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Models.Database;

namespace TicketSystem.Models
{
    public static class EnsureDatabase
    {
        private const string superAdminRole = "SuperAdmin";
        private const string adminRole = "Admin";
        private const string devRole = "Developer";
        private const string userRole = "User";

        private const string superAdminName = "ToSe";
        private const string adminName = "Admin";

        private const string superAdminPW = "SuperSecret123$";
        private const string PW = "Secret123$";

        public static void Migrate(IApplicationBuilder app)
        {
            AppDbContext ctx = app
                           .ApplicationServices
                           .CreateScope()
                           .ServiceProvider
                           .GetRequiredService<AppDbContext>();
            if (ctx.Database.GetPendingMigrations().Any())
            {
                ctx.Database.Migrate();
            }
        }

        public static async Task SeedDefaultAccounts(IApplicationBuilder app)
        {
            RoleManager<IdentityRole> roleManager = app.ApplicationServices
                                                       .CreateScope()
                                                       .ServiceProvider
                                                       .GetRequiredService<RoleManager<IdentityRole>>();

            UserManager<AppUser> userManager = app.ApplicationServices
                                                  .CreateScope()
                                                  .ServiceProvider
                                                  .GetRequiredService<UserManager<AppUser>>();

            if (!await roleManager.RoleExistsAsync(userRole))
            {
                IdentityRole role = new IdentityRole(userRole);
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync(devRole))
            {
                IdentityRole role = new IdentityRole(devRole);
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                IdentityRole role = new IdentityRole(adminRole);
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync(superAdminRole))
            {
                IdentityRole role = new IdentityRole(superAdminRole);
                await roleManager.CreateAsync(role);
            }

            AppUser? superAdmin = await userManager.FindByNameAsync(superAdminName);
            if (superAdmin == null)
            {
                superAdmin = new AppUser()
                {
                    Firstname = "Tom",
                    Lastname = "Selig",
                    UserName = superAdminName
                };
                await userManager.CreateAsync(superAdmin, superAdminPW);
                await userManager.AddToRolesAsync(superAdmin, new List<string> { superAdminRole, adminRole});
            }

            AppUser? admin = await userManager.FindByNameAsync(adminName);
            if (admin == null)
            {
                admin = new AppUser()
                {
                    Firstname = "Admin",
                    Lastname = "Admin",
                    UserName = adminName
                };
                await userManager.CreateAsync(admin, PW);
                await userManager.AddToRoleAsync(admin, adminRole);
            }

            AppUser? bjoern = await userManager.FindByNameAsync("BjSa");
            if (bjoern == null)
            {
                bjoern = new AppUser()
                {
                    Firstname = "Björn",
                    Lastname = "Sauskojus",
                    UserName = "BjSa"
                };
                await userManager.CreateAsync(bjoern, PW);
                await userManager.AddToRoleAsync(bjoern, devRole);
            }

            AppUser? corin = await userManager.FindByNameAsync("CoPr");
            if (corin == null)
            {
                corin = new AppUser()
                {
                    Firstname = "Corin",
                    Lastname = "Prescher",
                    UserName = "CoPr"
                };
                await userManager.CreateAsync(corin, PW);
                await userManager.AddToRoleAsync(corin, devRole);
            }

            AppUser? nico = await userManager.FindByNameAsync("NiHe");
            if (nico == null)
            {
                nico = new AppUser()
                {
                    Firstname = "Nico",
                    Lastname = "Heße",
                    UserName = "NiHe"
                };
                await userManager.CreateAsync(nico, PW);
                await userManager.AddToRoleAsync(nico, devRole);
            }

            AppUser? chris = await userManager.FindByNameAsync("ChDo");
            if (chris == null)
            {
                chris = new AppUser()
                {
                    Firstname = "Chris",
                    Lastname = "Domberg",
                    UserName = "ChDo"
                };
                await userManager.CreateAsync(chris, PW);
                await userManager.AddToRoleAsync(chris, userRole);
            }
            await app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>().SaveChangesAsync();
        }

        public static void SeedDatabase(IApplicationBuilder app)
        {
            var _ctx = app
            .ApplicationServices
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<AppDbContext>();

            if (!_ctx.Projects.Any())
            {
                _ctx.Projects.AddRange(
                    new Project()
                    {
                        Title = "Lunaria",
                        Description = "Ein 2D Sandbox Game indem der Spieler die Möglichkeit besitzt seine eigene kleine Mondbasis zu erbauen, Nicht-Spieler Characktere dort einziehen zu lassen, Viele verschiedene Bosse mit einer Großen Menge an Waffen bekämpfen und die Geheimnisse des Mondes zu erforschen",
                        StartDate = DateTime.Today,
                        EndDate = new DateTime(2030, 1, 1)
                    },
                    new Project()
                    {
                        Title = "DeinCraft",
                        Description = "Es ist NICHT ein Minecraft Clone! Ich wiederhole, NICHT EIN MINECRAFT Clone. Es ist ein Idle-Spiel indem man durchs Klicken auf verschiedene Blöcke die dazugehörigen Resourcen Erbeudet um darauf eine Vielfalt an Items herzustellen um schneller mehr und neue Resourcen zu erhalten.",
                        StartDate = DateTime.Today,
                        EndDate = new DateTime(2026, 12, 24)
                    },
                    new Project()
                    {
                        Title = "First Ark",
                        Description = "Ein MMO Lifesim indem man in einer Fantasy Welt lebt. Man kann sich auf viele Verschiedene Berufe Konzentrieren, eine Eigene Familie Aufbauen und die Welt von First Ark erkunden. Der Kern des Spieles ist es, die Wirtschaft vom Spiel nur durch Spieler zu generieren.",
                        StartDate = new DateTime(2023, 6, 24),
                        EndDate = new DateTime(2125, 6, 24)
                    },
                    new Project()
                    {
                        Title = "Taschen Kreaturen",
                        Description = "Ein Monster-Collecting Rollenspiel, in dem man sogenannte Taschen Kreaturen in kleinen, handlichen Bällen fängt und gegeneinander kämpfen lässt.",
                        StartDate = new DateTime(2023, 7, 1),
                        EndDate = new DateTime(2025, 7, 2),
                        Closed = true,
                        ClosedAt = new DateTime(2025, 7, 2)
                    });
                _ctx.SaveChanges();
            }

            if (!_ctx.Tickets.Any())
            {
                _ctx.Tickets.AddRange(
                    new Ticket() 
                    {
                        Title = "Black Screen",
                        Description = "Ein Fehler im Spiel sorgt dafür, das Spieler einen Blackscreen bekommen. Dies passiert laut Reports immer dann, wenn ein Spieler für Längere Zeit vom PC weg geht. Allen anschein nach, Meldet der Fehler denn Nutzer auch nach dem BlackScreen ab und zeigt ihm den Login Bereich ihres Betriebssystems an. Muss Dringend vor Releas gefixt werden.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "CoPr"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "CoPr").Id,
                        AssignedDate = DateTime.Now.AddDays(-12),
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-12)
                    },
                    new Ticket() 
                    {
                        Title = "Fehlende Gegenstände",
                        Description = "Anscheinend werden ab einen Gewissen Punkt im Spiel mehr Resourcen vom Spieler abgezogen für die Upgrades, als angegeben. Spieler berichten, das sobald wenn sie mit denn Händler Agieren, Mehr Saphire Abgezogen werden, als der Händler als Preis angibt. Genauere berichte sagen, das es sich hierbei um die Items Handeln, welche Viel zu Teuer währen, weshalb sie wohlen, das sie die Kosten Zurückerstattet bekommen können im Spiel.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "BjSa")!,
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket() 
                    {
                        Title = "BlueScreen",
                        Description = "Eine Große Anzahl an Reports meldet, das unsere Ultra Realistische Lava im neuem Vulkangebiet zu Realistisch sei. Sie lässt PCs überhitzen und sorgt für BlueScreens. Dies Passiert genau dann, wenn man für 42 Sekunden die Lava anschaut.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "NiHe"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket() 
                    {
                        Title = "OrangeScreen",
                        Description = "Einige User melden, dass ihre Bildschirme plötzlich Feuer fangen. Hilfe.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "GreenScreen",
                        Description = "",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "NiHe"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket()//
                    {
                        Title = "Bäcker backt Holz",
                        Description = "Beruf „Bäcker“ produziert aktuell statt Brot hölzerne Brötchen. Sehr nahrhaft mit hohem Brennwert.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "BjSa"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == superAdminName),
                        CreatorId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket()//
                    {
                        Title = "Kinder wachsen in Sekunden auf",
                        Description = "Nach dem Familiengründen überspringt das Kind alle Entwicklungsphasen und ist direkt Steuerberater.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "NiHe"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        AssignedDate = DateTime.Now.AddDays(-2),
                        Creator = _ctx.Users.First(a => a.UserName == "CoPr"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-1)
                    },
                    new Ticket()//
                    {
                        Title = "Marktpreise explodieren – wortwörtlich",
                        Description = "Bei zu vielen Transaktionen im Marktmenü werden die Zahlen so groß, dass sie den Bildschirm verlassen.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "CoPr"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == "BjSa"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket()
                    {
                        Title = "Spieler wird Bürgermeister – gegen seinen Willen",
                        Description = "Nach dem Login ist der Spieler plötzlich Dorfvorstand mit 300 offenen Anträgen. Kein Opt-Out möglich.\r\n\r\n",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "BjSa"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == superAdminName),
                        CreatorId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        CreatedDate = DateTime.Now
                    }, 
                    new Ticket()
                    {
                        Title = "Heiler heilt... die Gegner",
                        Description = "Der Berufs-Heiler heilt aktuell alles – inklusive Wildschweine, Banditen und explodierte Öfen.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "BjSa"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == "NiHe"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        CreatedDate = DateTime.Now
                    }, 
                    new Ticket()
                    {
                        Title = "Haus läuft weg",
                        Description = "Nach Platzieren eines Hauses beginnt es sich langsam über die Karte zu bewegen. Vielleicht auf Wohnungssuche?",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "NiHe"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == superAdminName),
                        CreatorId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        CreatedDate = DateTime.Now
                    }, 
                    new Ticket()
                    {
                        Title = "Wirtschaft kollabiert durch Kuh",
                        Description = "Wird eine bestimmte Kuh verkauft, stürzt der Marktpreis für alles auf 0. Experten vermuten Insiderhandel.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "ChDo"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == "BjSa"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket()
                    {
                        Title = "Spiel erkennt Hochzeiten nicht mehr",
                        Description = "Verheiratete Spieler erhalten bei Login die Nachricht „Willkommen, Fremder.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == "BjSa"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        CreatedDate = DateTime.Now
                    }, new Ticket()
                    {
                        Title = "Angeln fängt Spieler",
                        Description = "Beim Angeln kann der Spieler sich selbst fangen und wird kurzzeitig ins Inventar verschoben.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "BjSa"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == "CoPr"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket()
                    {
                        Title = "Beruf „Faulenzer“ hat höchste Einnahmen",
                        Description = "Spieler, die nichts tun, erhalten passiv Gold. Und Respekt. Und Landtitel. Warum auch immer.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "First Ark")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "BjSa"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket()/////
                    {
                        Title = "Kreatur hat Existenzangst",
                        Description = "Beim Fangen der Kreatur „Flauschi“ erscheint der Text: \"Warum? Ich wollte nur Blumen pflücken…\"",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "BjSa"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        AssignedDate = DateTime.Now,
                        Creator = _ctx.Users.First(a => a.UserName == superAdminName),
                        CreatorId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        CreatedDate = DateTime.Now,
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Bälle fliegen in falsche Richtung",
                        Description = "Wirft man einen Ball auf ein wildes Monster, trifft er den eigenen Begleiter. Dieser ist jetzt beleidigt.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Kreatur nutzt „Flamme“ und Spiel schmilzt",
                        Description = "Bei Nutzung der Attacke „Flamme Ultra“ erhitzt sich die CPU auf 102 °C. Spiel reagiert dann etwas... flüssig.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Kampfmusik spielt beim Öffnen des Inventars",
                        Description = " Öffnet man das Inventar, ertönt dramatische Kampfmusik. Es passiert aber nichts. Nur Panik.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "BjSa"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "CoPr"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Level 1 Kreatur besiegt Boss mit Blick",
                        Description = "„Glubschi“ (Level 1) besiegt Endboss „Titanus“ mit einem kritischen Starren. Wahrscheinlich ein Bug. Oder göttlich.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "NiHe"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Kreaturen entwickeln sich rückwärts",
                        Description = "Nach Erreichen von Level 20 verwandelt sich „Brutzal“ zurück in ein Ei. Spieler auch, innerlich.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == superAdminName),
                        CreatorId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen,
                        AssignedUser = _ctx.Users.First(a => a.UserName == "ChDo"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Ball fängt Ausrufezeichen",
                        Description = "Manchmal fängt man beim Wurf nicht die Kreatur, sondern das „!“ über ihrem Kopf. Kein Effekt. Aber seltsam.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Namensgenerator schlägt nur „Hans“ vor",
                        Description = "Egal welche Kreatur – der Standardname ist immer „Hans“. Auch bei Drachen oder Schleimwesen.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen,
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Kreaturen kämpfen... im Ladebildschirm",
                        Description = "Zwei Kreaturen beginnen einen Kampf im Ladebildschirm. Der Sieger bestimmt, ob das Spiel lädt oder abstürzt.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Gegnerische Kreatur stellt philosophische Fragen",
                        Description = "Statt zu kämpfen fragt das wilde „Schnorkl“: \"Glaubst du, ich habe eine Wahl?\" Spiel friert dann ein.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Taschen Kreaturen")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "CoPr"),
                        CreatorId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        CreatedDate = new DateTime(2023, 7, 15),
                        Standing = Standings.geschlossen,
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "NPC hat Existenzkrise",
                        Description = "Bewohner Ulfbert steht seit 3 Tagen regungslos auf der Stelle und murmelt „Ich bin kein echtes Skript... bin ich?“",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Mondboden absorbiert Inventar",
                        Description = "Nach dem Platzieren einer Kiste wird sie vom Mondboden verschluckt. Alle Items sind vermutlich jetzt Eigentum der NASA.“",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "NiHe")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        CreatedDate = DateTime.Now,
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Boss „Mondwurm“ kämpft gegen sich selbst",
                        Description = "Der Boss hat sich beim Spawnen selbst angegriffen. Vermutlich schlechte Laune?",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "NiHe")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket()
                    {
                        Title = "Raketenstuhl startet, aber Spieler bleibt zurück",
                        Description = "Beim Teststart des Raketenstuhls bleibt der Spieler zurück, aber die Kamera folgt der Rakete.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "CoPr")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "NPC wohnt in Luft",
                        Description = "Nach Zuweisung eines Betts entscheidet sich der NPC dafür, 3 Tiles über dem Boden zu wohnen. Freigeist.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == superAdminName)!,
                        CreatorId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        CreatedDate = DateTime.Now.AddDays(-7).AddYears(-1),
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-1)
                    },
                    new Ticket()
                    {
                        Title = "Tag-Nacht-Zyklus wird durch Pause-Taste beleidigt",
                        Description = "Wird das Spiel pausiert, rast die Zeit trotzdem weiter. Der Mond lacht über unsere Gesetze.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = DateTime.Now,
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Gravitation ist beleidigt",
                        Description = "Manchmal springt der Spieler 30 Sekunden lang, ohne zurückzukehren. Physik hat gekündigt.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "CoPr")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket()
                    {
                        Title = "Baum fällt horizontal",
                        Description = "Gefällte Bäume kippen nicht, sondern legen sich wie müde Welpen flach hin.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "NiHe")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Bossmusik bleibt für immer",
                        Description = "Nach einem Bosskampf bleibt die dramatische Musik bestehen – auch beim Pflanzen von Karotten.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "NiHe")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        CreatedDate = DateTime.Now
                    },
                    new Ticket()
                    {
                        Title = "Helm macht unverwundbar (aber nur optisch)",
                        Description = "Nach Anlegen eines Helms denkt der Spieler, er sei unverwundbar. Gegner denken das nicht.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "Lunaria")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Spieler klickt so schnell, dass Zeit stehen bleibt",
                        Description = "Bei über 100 Klicks pro Sekunde friert das Spiel ein und reflektiert über seine Existenz.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "BjSa")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Baum gibt Eisen",
                        Description = "Beim Klicken auf einen Baum erhält man Eisen. Wahrscheinlich ein Baumarkt.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "BjSa")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == "NiHe"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Fortschrittsbalken hat keine Motivation",
                        Description = "Der Fortschrittsbalken bleibt bei 42 %. Hat wahrscheinlich seinen Sinn des Lebens erreicht.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == "NiHe"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Auto-Clicker wurde selbstbewusst",
                        Description = "Nach Einsatz eines Auto-Clickers beginnt das Spiel, selbstständig zu klicken – auch im Menü.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "ChDo")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == "BjSa"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "BjSa")!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Crafting gibt dir… nichts",
                        Description = "Manchmal entsteht beim Crafting nichts. Spieler fühlt sich wie bei Ikea ohne Anleitung.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "CoPr")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Ressourcen respawnen schneller als Licht",
                        Description = "Abgebauter Block erscheint vor den Augen des Spielers erneut. Vermutlich Quantenphysik.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "CoPr")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Menü ignoriert Spieler komplett",
                        Description = "Klicken auf \"Optionen\" führt zu… nichts. Das Menü macht heute Homeoffice.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == superAdminName)!,
                        CreatorId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == "ChDo"),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == "ChDo")!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Tooltip zeigt lateinische Lebensweisheit",
                        Description = "Statt Itembeschreibung erscheint der Text: Lorem ipsum dolor sit amet....",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "NiHe")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "NiHe")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    },
                    new Ticket()
                    {
                        Title = "Musik spielt rückwärts",
                        Description = "Nach langem Spielen läuft der Soundtrack rückwärts. Wahrscheinlich dämonischer Easter Egg.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "CoPr")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        Standing = Standings.geschlossen
                    },
                    new Ticket()
                    {
                        Title = "Spiel behauptet es sei Minecraft",
                        Description = "Beim Startbildschirm steht kurz „Minecraft 1.2.3 Alpha“ in der Titelleiste. Bitte ignorieren. Nur ein kleiner Identitätsfehler.",
                        AssignedProject = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!,
                        AssignedProjectId = _ctx.Projects.FirstOrDefault(a => a.Title == "DeinCraft")!.Id,
                        Creator = _ctx.Users.First(a => a.UserName == "CoPr")!,
                        CreatorId = _ctx.Users.First(a => a.UserName == "CoPr")!.Id,
                        CreatedDate = DateTime.Now.AddDays(-3).AddYears(-2),
                        AssignedUser = _ctx.Users.First(a => a.UserName == superAdminName),
                        AssignedUserId = _ctx.Users.First(a => a.UserName == superAdminName)!.Id,
                        AssignedDate = DateTime.Now.AddDays(-3).AddYears(-2)
                    }
                    );
                    
                _ctx.SaveChanges();
            }
            if (!_ctx.TicketAssignHistories.Any() && _ctx.Tickets.Any()) 
            {
                var ticketList = _ctx.Tickets
                    .Include(t => t.AssignedUser)
                    .Include(t => t.Creator).ToList();
                foreach (var ticket in ticketList) // Schleife zum Erzeugen der TicketAssignHistory
                {
                    if (ticket.AssignedUser != null)
                    {

                        _ctx.TicketAssignHistories.Add(
                            new TicketAssignHistory()
                            {
                                TicketId = ticket.Id,
                                AssignedAt = ticket.AssignedDate,
                                AssignedBy = ticket.Creator,
                                AssignedById = ticket.Creator.Id,
                                AssignedTo = ticket.AssignedUser,
                                AssignedToId = ticket.AssignedUser.Id
                            });
                    }
                }
                _ctx.SaveChanges();
            }
        }
    }
}
