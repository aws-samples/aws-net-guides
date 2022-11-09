using AspNetCoreWebApiRds.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWebApiRds;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        ApplicationOptions applicationOptions = new ApplicationOptions();

        Configuration.Bind(applicationOptions);    

        string connectionString = SecretsService.GetConnectionStringFromSecret(applicationOptions).GetAwaiter().GetResult();
        services.AddDbContext<BillingContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BillingContext billingContext)
    {
        Console.WriteLine("Trying to connect to database...");
        billingContext.Database.EnsureCreated();
        billingContext.Seed();
        Console.WriteLine("Successfully connected to database...");

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}