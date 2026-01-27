var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable default files (index.html, default.html, etc.)
app.UseDefaultFiles();

// Enable serving static files from wwwroot
app.UseStaticFiles();

// Fallback to serve index.html for client-side routing
app.MapFallbackToFile("index.html");

app.Run();



