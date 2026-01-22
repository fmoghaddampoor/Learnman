using Learnman.Components;
using Microsoft.EntityFrameworkCore;
using Learnman.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpClient();
builder.Services.AddScoped<Learnman.Services.IAITutorService, Learnman.Services.OllamaAITutorService>();
builder.Services.AddScoped<Learnman.Services.ITextToSpeechService, Learnman.Services.MockTextToSpeechService>();
builder.Services.AddDbContext<Learnman.Data.AppDbContext>(options =>
    options.UseSqlite("Data Source=learnman.db"));
builder.Services.AddScoped<Learnman.Services.AuthService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Learnman.Client._Imports).Assembly);



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Mock/Seed Data
    var auth = scope.ServiceProvider.GetRequiredService<Learnman.Services.AuthService>();
    try
    {
         if (!db.Users.Any(u => u.Email == "test@test.com"))
         {
             auth.RegisterAsync("Test", "User", "test@test.com", "test").GetAwaiter().GetResult();
         }
    }
    catch { /* Ignore if already exists */ }
}

app.Run();
