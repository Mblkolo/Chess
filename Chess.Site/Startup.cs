using System;
using Microsoft.Data.Sqlite;

namespace Chess.Site
{
    using Dal;
    using Integration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DbConnectionOptions>(Configuration.GetSection("DbConnection"));
            services.Configure<SlackOptions>(Configuration.GetSection("Slack"));

            services.AddTransient<SessionFactory>();
            services.AddTransient<SlackService>();
            services.AddTransient<RatingRepository>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SessionFactory sessionFactory)
        {
            app.UsePathBase(Configuration.GetSection("ServerPathBase").Value);

            CreateDateBaseIfNotExist(sessionFactory);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Rating}/{action=Index}/{id?}");
            });
        }

        private void CreateDateBaseIfNotExist(SessionFactory sessionFactory)
        {
            sessionFactory.Execute(s =>
            {
                int tableCount = s.ExecuteScalar<int>("select count(*) from sqlite_master as tables where type='table'");
                if (tableCount == 0)
                {
                    s.Execute(
                        @"CREATE TABLE players(
                            id INTEGER PRIMARY KEY ASC AUTOINCREMENT,
                            name TEXT NOT NULL,
                            slackNickname TEXT,
                            decipoints INTEGER NOT NULL
                        )");

                    s.Execute(
                        @"CREATE TABLE gameResults(
                            id INTEGER PRIMARY KEY ASC AUTOINCREMENT,
                            whitePlayerId INTEGER NOT NULL REFERENCES players(ID),
                            blackPlayerId INTEGER NOT NULL REFERENCES players(ID),
                            whiteDeltaDecipoints INTEGER NOT NULL,
                            blackDeltaDecipoints INTEGER NOT NULL,
                            winner TEXT NOT NULL,
                            createdAt TEXT NOT NULL
                        )");
                }

                try
                {
                    s.Execute(
                        @"ALTER TABLE players
                            ADD insignias TEXT");
                }
                catch (SqliteException e)
                {
                    
                }
            });
        }
    }
}