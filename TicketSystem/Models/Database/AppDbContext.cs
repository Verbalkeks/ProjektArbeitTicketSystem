using System;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TicketSystem.Models.Database
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<MsgUser> MsgUsers { get; set; }
        public DbSet<Msgmsg> MsgMsgs { get; set; }
        public DbSet<Dependency> Dependencies { get; set; }
        public DbSet<TicketAssignHistory> TicketAssignHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //// Selektiert alle Entitäten (Tabellen), außer Identity
            //foreach (var entityType in builder.Model
            //            .GetEntityTypes()
            //            .Where(et => !et.Name.Contains("AspNet")))
            //{
            //    // Ändert alle ForeignKeys, die auf Cascade stehen zu Restrict
            //    entityType.GetForeignKeys()
            //        .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade)
            //        .ToList()
            //        .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
            //}

            builder.Entity<Msgmsg>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); //SQL NERVT

            builder.Entity<Msgmsg>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.Chatid)
                .OnDelete(DeleteBehavior.Restrict); // MICH ZUSEHR

            builder.Entity<MsgUser>()
                .HasOne(m => m.User1)
                .WithMany()
                .HasForeignKey(m => m.UserId1)
                .OnDelete(DeleteBehavior.Restrict); // MIT CASCADE

            builder.Entity<MsgUser>()
                .HasOne(m => m.User2)
                .WithMany()
                .HasForeignKey(m => m.UserId2)
                .OnDelete(DeleteBehavior.Restrict); // ERRORS!!

            builder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // ICH HASSE SQL UND DAS DRECKS CASCADE

            builder.Entity<FileModel>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Naja, wenn man einmal weiß, dass man drauf achten muss, ist es doch nicht so schlimm

            builder.Entity<FileModel>()
                .HasOne(f => f.Ticket)
                .WithMany()
                .HasForeignKey(f => f.TicketId)
                .OnDelete(DeleteBehavior.Restrict); // Trotzdem nervig

            builder.Entity<Dependency>()
                .HasOne(d => d.TicketDependency)
                .WithMany()
                .HasForeignKey(d => d.TicketDependencyId)
                .OnDelete(DeleteBehavior.Restrict); // BRAUCH MAN DAS??

            builder.Entity<TicketAssignHistory>()
                .HasOne(tah => tah.AssignedBy)
                .WithMany()
                .HasForeignKey(tah => tah.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketAssignHistory>()
                .HasOne(tah => tah.AssignedTo)
                .WithMany()
                .HasForeignKey(tah => tah.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Ticket>()
                //.HasOne(a => a.AssignedProject)
                //.WithMany(a => a.Tickets)
                //.HasForeignKey(t => t.AssignedProjectId)
                //.OnDelete(DeleteBehavior.SetNull);
        }
    }
}
