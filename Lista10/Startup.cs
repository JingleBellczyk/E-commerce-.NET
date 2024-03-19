using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lista10.Data;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Lista10.Models;

namespace Lista10
{
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
        services.AddSingleton<IRepository, MemoryRepository>();

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

        services.AddDistributedMemoryCache();

        services.AddAuthorization(options => {
            options.AddPolicy("RequireRoleForTurnOnOff", policy =>
            policy.RequireRole("Administrator", "PowerUser", "BackupAdministrator"));
            });

        //autoryzacja
        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<MyDbContext>();

        services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                //TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

        services.AddControllersWithViews();
        services.AddDbContextPool<MyDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MyDb")));
    }
       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();


        //autoryzacja
        app.UseAuthentication();
        app.UseAuthorization();

        MyIdentityDataInitializer.SeedData(userManager,roleManager);

        //cookies
        app.UseSession();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });
    }
}
}

