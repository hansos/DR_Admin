namespace DR_Home
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Enable serving static files from wwwroot
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Minimal API endpoint for health check (future extensibility)
            app.MapGet("/api/health", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }))
                .WithName("HealthCheck");

            app.Run();
        }
    }
}
