using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using provaDatabase;
using provaweb;
using System.Reflection;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ActiveUsersService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ActiveUsersService>());
builder.Services.AddRelaySwitch();
builder.Services.Addreceiver_esp8266Service();
builder.Services.addServiceDatabase();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen();
var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
var DataProtection = Path.Combine(programData, "provaweb/DataProtection - Keys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(DataProtection))
    .SetApplicationName("provaweb");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/Account/cookie";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/";
    options.Cookie.Name = "esp8266";
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ClaimsIssuer = "claims";
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ReturnUrlParameter = "esp8266";
    options.ExpireTimeSpan = TimeSpan.FromDays(365);
    options.SlidingExpiration = false;
    options.Events = new CookieAuthenticationEvents
    {

        OnValidatePrincipal = context =>
        {

            if (context.Principal?.Identity?.IsAuthenticated == true)
            {
                if (context.Principal.Identity.Name != null)
                    context.HttpContext.RequestServices.GetRequiredService<ActiveUsersService>().SetLastVisit(context.Principal.Identity.Name);
                context.HttpContext.Response.StatusCode = 200;
            }

            return Task.CompletedTask;
        }


    };

    options.Validate(CookieAuthenticationDefaults.AuthenticationScheme);
});



var app = builder.Build();

var asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFileServer(new FileServerOptions()
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(asmDir!, "dist")),
    RequestPath = "",
 
});



/*app.Map("/api", p => p.RunHttpProxy("http://192.168.1.2/api", x =>
{
    x.WithAfterReceive((c, r) =>
    {
        r.Headers.Clear();
        return Task.CompletedTask;
    });
}));*/
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();