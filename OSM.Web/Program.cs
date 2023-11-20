using OSM;
using OSM.Interfaces;
using OSM.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IOsmClient, OsmClient>();
builder.Services.AddTransient<BasicAuthMiddleware>();
builder.Services.AddControllersWithViews();

Console.WriteLine(builder.Configuration["OsmOptions:ClientId"]);

var app = builder.Build();

app.UseMiddleware<BasicAuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();