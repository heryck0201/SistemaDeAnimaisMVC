using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SistemaDeAnimaisMVC.Servico;
using SistemaDeAnimaisMVC.Servico.Interfaces;

namespace SistemaDeAnimaisMVC
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Usuario/IndexLogin";
                    });

            
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
          
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<ISessaoLogin, SessaoLogin>();

            //services.AddSession(o =>
            //{
            //    o.Cookie.HttpOnly = true;
            //    o.Cookie.IsEssential = true;
            //});


            //var app = services.BuildServiceProvider();
            services.AddSession();
            services.AddRazorPages();
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(options =>
            //{
            //    /*https://localhost:7266*/
            //    options.Authority = "https://localhost:44367/";
            //    options.Audience = "sua-aplicacao";
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Usuario/Error");
            }

            
            app.UseSession();

            /*app.UseHttpsRedirection();*///coloquei para ver se parava o erro
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCookiePolicy();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Usuario}/{action=IndexUsuario}");
            });

        }
    } 
}
