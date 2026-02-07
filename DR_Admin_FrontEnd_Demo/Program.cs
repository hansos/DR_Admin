namespace DR_Admin_FrontEnd_Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add minimal services for API endpoints
            builder.Services.AddControllers();
            
            // Add distributed memory cache (required for session)
            builder.Services.AddDistributedMemoryCache();
            
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add HttpClient for calling DR_Admin API
            builder.Services.AddHttpClient("DrAdminApi", client =>
            {
                var baseUrl = builder.Configuration["DrAdminApi:BaseUrl"];
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    client.BaseAddress = new Uri(baseUrl);
                }
                client.Timeout = TimeSpan.FromSeconds(
                    builder.Configuration.GetValue<int>("DrAdminApi:Timeout", 30)
                );
            });

            // Add CORS to allow calling the API
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("https://localhost:7201")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Configure middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            // Serve static files (HTML, CSS, JS)
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            // Map API controllers
            app.MapControllers();

            app.Run();
        }
    }
}
