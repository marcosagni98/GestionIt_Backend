using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace InfrastructureTests
{
    public class UnitOfWorkTests
    {
        [Fact]
        public async Task SaveAsync_ShouldPersistUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UnitOfWork_SaveUser_Test")
                .Options;

            var testUser = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com"
            };

            // Act
            using (var context = new AppDbContext(options))
            {
                context.Users.Add(testUser);

                var unitOfWork = new UnitOfWork(context);
                await unitOfWork.SaveAsync();
            }

            // Assert
            using (var context = new AppDbContext(options))
            {
                var savedUser = await context.Users.FindAsync(testUser.Id);
                Assert.NotNull(savedUser);
                Assert.Equal(testUser.Name, savedUser.Name);
                Assert.Equal(testUser.Email, savedUser.Email);
            }
        }

        [Fact]
        public async Task SaveAsync_ShouldPersistIncidentWithRelations()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UnitOfWork_SaveIncident_Test")
                .Options;

            var userId = 1;
            var technicianId = 2;
            var incidentId = 3;

            // Act
            using (var context = new AppDbContext(options))
            {
                var user = new User
                {
                    Id = userId,
                    Name = "User",
                    Email = "user@example.com",
        
                };

                var technician = new User
                {
                    Id = technicianId,
                    Name = "Technician",
                    Email = "tech@example.com"
                };

                var incident = new Incident
                {
                    Id = incidentId,
                    Title = "Test Incident",
                    Description = "Test Description",
                    UserId = userId,
                    TechnicianId = technicianId,
                    CreatedAt = DateTime.UtcNow,
                };

                context.Users.AddRange(user, technician);
                context.Incidents.Add(incident);

                var unitOfWork = new UnitOfWork(context);
                await unitOfWork.SaveAsync();
            }

            // Assert
            using (var context = new AppDbContext(options))
            {
                var savedIncident = await context.Incidents
                    .Include(i => i.User)
                    .Include(i => i.Technician)
                    .FirstOrDefaultAsync(i => i.Id == incidentId);

                Assert.NotNull(savedIncident);
                Assert.Equal("Test Incident", savedIncident.Title);
                Assert.Equal("Test Description", savedIncident.Description);

                Assert.NotNull(savedIncident.User);
                Assert.Equal(userId, savedIncident.UserId);
                Assert.Equal("User", savedIncident.User.Name);

                Assert.NotNull(savedIncident.Technician);
                Assert.Equal(technicianId, savedIncident.TechnicianId);
                Assert.Equal("Technician", savedIncident.Technician.Name);
            }
        }

        [Fact]
        public async Task SaveAsync_WithMultipleEntities_ShouldPersistAll()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UnitOfWork_SaveMultiple_Test")
                .Options;

            long userId = 1;
            long incidentId =2;
            long messageId = 3;
            long workLogId = 4;

            // Act
            using (var context = new AppDbContext(options))
            {
                var user = new User
                {
                    Id = userId,
                    Name = "Test User",
                    Email = "user@example.com",
                };

                var incident = new Incident
                {
                    Id = incidentId,
                    Title = "Complex Test",
                    Description = "Testing multiple entities",
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                var message = new Message
                {
                    Id = messageId,
                    IncidentId = incidentId,
                    SenderId = userId,
                    Text = "Test message",
                };

                var workLog = new WorkLog
                {
                    Id = workLogId,
                    IncidentId = incidentId,
                    TechnicianId = userId,
                    LogDate = DateTime.UtcNow,
                    MinWorked = 60,
                };

                context.Users.Add(user);
                context.Incidents.Add(incident);
                context.Messages.Add(message);
                context.WorkLogs.Add(workLog);

                var unitOfWork = new UnitOfWork(context);
                await unitOfWork.SaveAsync();
            }

            // Assert
            using (var context = new AppDbContext(options))
            {
                var user = await context.Users.FindAsync(userId);
                Assert.NotNull(user);

                var incident = await context.Incidents.FindAsync(incidentId);
                Assert.NotNull(incident);

                var message = await context.Messages.FindAsync(messageId);
                Assert.NotNull(message);
                Assert.Equal("Test message", message.Text);

                var workLog = await context.WorkLogs.FindAsync(workLogId);
                Assert.NotNull(workLog);
                Assert.Equal(60, workLog.MinWorked);
            }
        }
    }
}