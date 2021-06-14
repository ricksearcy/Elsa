
using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Elsa.Persistence.MongoDb.Extensions;
using Elsa.Workflow.Workflows;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;

namespace Elsa.Server
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IWebHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            const string dbName = "CorasCloud_WorkFlow";
            var elsaSection = Configuration.GetSection("Elsa");
            // Elsa services.
            services
                .AddElsa(elsa => elsa.AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddQuartzTemporalActivities()
                    .AddJavaScriptActivities()
                    .AddHttpActivities()
                    .AddConsoleActivities()
                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddEmailActivities(elsaSection.GetSection("Smtp").Bind)
                    .AddWorkflowsFrom<Startup>()
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlite())
                    .AddWorkflow<ApprovalWorkflow>()
                );

            // Elsa API endpoints.
            services.AddElsaApiEndpoints();

            // For Dashboard.
            services.AddRazorPages();
        
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseStaticFiles() // For Dashboard.
                .UseHttpActivities()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
                    endpoints.MapControllers();

                    // For Dashboard.
                    endpoints.MapFallbackToPage("/_Host");
                });
        }
    }
}
