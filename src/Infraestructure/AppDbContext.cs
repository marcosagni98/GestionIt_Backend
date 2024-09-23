using Domain.Entities; 
using Microsoft.EntityFrameworkCore;

namespace Infraestructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<IncidentHistory> IncidentHistories { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<WorkLog> WorkLogs { get; set; }
    public DbSet<UserFeedback> UserFeedbacks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar la relación entre Incident y User (creador del incidente)
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.User)
            .WithMany(u => u.Incidents)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Configura el comportamiento de eliminación según necesites

        // Configurar la relación entre Incident y User (técnico asignado)
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.Technician)
            .WithMany() // No necesitas una colección en User para Technician
            .HasForeignKey(i => i.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar la relación entre IncidentHistory y Incident
        modelBuilder.Entity<IncidentHistory>()
            .HasOne(ih => ih.Incident)
            .WithMany(i => i.IncidentHistories)
            .HasForeignKey(ih => ih.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar la relación entre IncidentHistory y User (quien cambió el estado)
        modelBuilder.Entity<IncidentHistory>()
            .HasOne(ih => ih.ChangedByUser)
            .WithMany()
            .HasForeignKey(ih => ih.ChangedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar la relación entre Message y Incident
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Incident)
            .WithMany(i => i.Messages)
            .HasForeignKey(m => m.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar la relación entre Message y User (quien envía el mensaje)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar la relación entre WorkLog y Incident
        modelBuilder.Entity<WorkLog>()
            .HasOne(w => w.Incident)
            .WithMany(i => i.WorkLogs)
            .HasForeignKey(w => w.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar la relación entre WorkLog y User (técnico)
        modelBuilder.Entity<WorkLog>()
            .HasOne(w => w.Technician)
            .WithMany()
            .HasForeignKey(w => w.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar la relación entre UserFeedback y Incident
        modelBuilder.Entity<UserFeedback>()
            .HasOne(uf => uf.Incident)
            .WithMany(i => i.UserFeedbacks)
            .HasForeignKey(uf => uf.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar la relación entre UserFeedback y User
        modelBuilder.Entity<UserFeedback>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.UserFeedbacks)
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
