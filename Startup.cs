using ValueCards.Data;
using ValueCards.Models;
using ValueCards.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;

namespace ValueCards
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      Configuration = configuration;
      Env = env;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Env { get; set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(
              Configuration.GetConnectionString("DefaultConnection")));

      services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

      services.AddControllersWithViews()
        .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
      //services.AddRazorPages()
      //  .AddRazorRuntimeCompilation();

      // configure strongly typed settings objects
      var appSettingsSection = Configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSettingsSection);

      // configure jwt authentication
      var appSettings = appSettingsSection.Get<AppSettings>();
      var key = Encoding.ASCII.GetBytes(appSettings.Secret);

      IMvcBuilder builder = services.AddRazorPages();
#if DEBUG
      if (Env.IsDevelopment())
      {
        builder.AddRazorRuntimeCompilation();
      }
#endif

      services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
      {
        options.SignIn.RequireConfirmedAccount = true;
      })
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddSignInManager<ApplicationSignInManager>()
      .AddDefaultTokenProviders();

      services.AddAuthentication();

      services.AddScoped<IAuthenticationHandlerProvider, MyAuthenticationHandlerProvider>();

      services.AddAuthorization();

      services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

      services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
      services.Configure<CookieTempDataProviderOptions>(options =>
      {
        options.Cookie.IsEssential = true;
      });

      services.Configure<WebServiceOption>(Configuration.GetSection("WebService"));

      services.AddHealthChecks();

      //services.AddSignalR();

      services.AddKendo();

      services.AddSingleton<IEmailSender, EmailSender>();
      services.AddSingleton<IRichEmailSender, EmailSender>(x => (EmailSender)x.GetService<IEmailSender>());
      services.AddSingleton<IConsumerService, ConsumerService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      //app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        //endpoints.MapControllers();
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=consumers}/{action=Index}/{id?}");
        endpoints.MapRazorPages();
        endpoints.MapHealthChecks("/healthz", new HealthCheckOptions() { });
      });

    }
  }
}
