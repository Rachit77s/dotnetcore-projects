using Microsoft.EntityFrameworkCore;
using NotesApp.Components;
using NotesApp.Data;
using NotesApp.Services;

namespace NotesApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();

                // Add DbContext with error handling
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
                }

                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Add services
                builder.Services.AddScoped<INoteService, NoteService>();

                var app = builder.Build();

                // Verify database connection on startup
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<ApplicationDbContext>();
                        // Check if database can be connected
                        context.Database.CanConnect();
                        app.Logger.LogInformation("Database connection verified successfully");
                    }
                    catch (Exception ex)
                    {
                        app.Logger.LogError(ex, "An error occurred while connecting to the database. Please ensure the database is accessible and migrations are applied.");
                        // Note: Application will continue to run, but database operations will fail
                    }
                }

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseStaticFiles();
                app.UseAntiforgery();

                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();

                app.Logger.LogInformation("Application started successfully");
                app.Run();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Configuration Error: {ex.Message}");
                Console.WriteLine("Please check your appsettings.json file and ensure the connection string is properly configured.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal Error: An unexpected error occurred while starting the application.");
                Console.WriteLine($"Error details: {ex.Message}");
                throw;
            }
        }
    }
}
