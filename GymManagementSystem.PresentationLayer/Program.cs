using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.BusinessLayer.Services;
using GymManagementSystem.DataLayer.Database;
using GymManagementSystem.DataLayer.Interfaces;
using GymManagementSystem.DataLayer.Repositories.RepositoryClasses;
using GymManagementSystem.DataLayer.SeedData;
using GymManagementSystem.PresentationLayer.BackgroundGobs;
using GymManagementSystem.PresentationLayer.ExceptionHandlers;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .WriteTo.Console()
           .WriteTo.Seq(ctx.Configuration["Seq:ServerUrl"]!));

    builder.Services.AddControllersWithViews();
    builder.Services.AddDbContext<GymDatabaseContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddScoped<ICleanUpDeletedRows, CleanUpDeletedRowsServices>();
    builder.Services.AddHostedService<SoftDeleteCleanUp>();
    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<IMemberService, MemberService>();
    builder.Services.AddScoped<IExportService, ExportService>();
    var app = builder.Build();

    using var scope = app.Services.CreateScope(); 
    var context = scope.ServiceProvider.GetRequiredService<GymDatabaseContext>();
    await DatabaseSeed.SeedAsync(context);

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();


    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
