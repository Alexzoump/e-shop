using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Data;
using Business.Interfaces;
using Business.Services;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory // Χρησιμοποιεί την κατάλληλη διαδρομή (path) στο σύστημα αρχείων (filesystem)
});

// Προσθήκη των controllers και ρυθμίσεις JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Swagger API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotSkroutz API", Version = "v1" });
});

// Πρόσβαση στο HttpContext που χρειάζεται στο AuthService
builder.Services.AddHttpContextAccessor();

// Ρύθμιση του DbContext με χρήση SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Καταχώρηση των Repositories και Services στο Dependency Injection container
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();

// Ρύθμιση CORS policy για επιτρεπτές συνδέσεις
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true) // Επιτρέπει οποιοδήποτε origin
            .AllowCredentials(); // Απαραίτητο για χρήση cookies σε authentication
    });
});

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";  // Path για μη εξουσιοδοτημένους
        options.LogoutPath = "/logout"; // Path για logout
        options.Cookie.Name = "NotSkroutzAuth"; // Όνομα του authentication cookie
        options.Cookie.HttpOnly = true; // Αποτρέπει την πρόσβαση μέσω JavaScript
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict; // Περιορίζει τη χρήση των cookies
        // options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ενεργοποίηση μόνο για HTTPS σε production environment
    });

var app = builder.Build();

// Δημιουργία της βάσης δεδομένων εάν δεν υπάρχει (ή εκτέλεση migrations αν υπάρχουν).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // ή db.Database.Migrate() αν χρησιμοποιούνται migrations...
    DbInitializer.Seed(db); // Εισαγωγή αρχικών δεδομένων στη βάση...
}

// Ενεργοποίηση Swagger μόνο σε περιβάλλον development...
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotSkroutz API v1"));
}

// Στατική εξυπηρέτηση αρχείων από το φάκελο "Frontend"
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Frontend")),
    RequestPath = ""
});

// Εφαρμογή της πολιτικής CORS
app.UseCors("AllowAll");

// Ενεργοποίηση Authentication & Authorization middleware
app.UseAuthentication(); // Διαχείριση authentication μέσω cookies
app.UseAuthorization();  // Διαχείριση authorization βάσει ρόλων

// Χαρτογράφηση των controllers
app.MapControllers();

// Χρήση των προεπιλεγμένων αρχείων (π.χ. index.html)
app.UseDefaultFiles();

// Ορισμός του path για το frontend...
var frontendPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Frontend"));

// Έλεγχος αν ο φάκελος του frontend υπάρχει...
if (!Directory.Exists(frontendPath))
{
    Console.WriteLine($"Δε βρέθηκε το frontend στον κατάλογο {frontendPath}...");
}
else
{
    // Εξυπηρέτηση αρχείων από το frontend...
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPath),
        RequestPath = ""
    });
}

// Εκκίνηση της εφαρμογής
app.Run();

// Ορισμός κλάσης Program ως public για δυνατότητα testing
public partial class Program { }