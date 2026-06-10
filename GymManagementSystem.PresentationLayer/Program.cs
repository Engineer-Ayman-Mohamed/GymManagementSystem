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

    var isTesting = builder.Environment.IsEnvironment("Testing");

    if (!isTesting)
    {
        builder.Host.UseSerilog((ctx, cfg) =>
            cfg.ReadFrom.Configuration(ctx.Configuration)
               .WriteTo.Console()
               .WriteTo.Seq(ctx.Configuration["Seq:ServerUrl"]!));
    }

    builder.Services.AddControllersWithViews();

    if (!isTesting)
    {
        builder.Services.AddDbContext<GymDatabaseContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
        });
    }

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddScoped<ICleanUpDeletedRows, CleanUpDeletedRowsServices>();
    builder.Services.AddHostedService<SoftDeleteCleanUp>();
    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<IMemberService, MemberService>();
    builder.Services.AddScoped<ISessionService, SessionService>();
    builder.Services.AddScoped<IBookingService, BookingService>();
    builder.Services.AddScoped<IExportService, ExportService>();
    builder.Services.AddScoped<IAttendanceService, AttendanceService>();
    builder.Services.AddScoped<ITrainerService, TrainerService>();
    builder.Services.AddScoped<IPlanService, PlanService>();
    builder.Services.AddScoped<IMembershipService, MembershipService>();
    builder.Services.AddScoped<IHealthRecordService, HealthRecordService>();
    var app = builder.Build();

    using var scope = app.Services.CreateScope(); 
    var context = scope.ServiceProvider.GetRequiredService<GymDatabaseContext>();
    await DatabaseSeed.SeedAsync(context);

    if (!isTesting) app.UseSerilogRequestLogging();
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
