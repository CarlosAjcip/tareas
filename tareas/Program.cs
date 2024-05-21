using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using tareas.Servicios;

namespace tareas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //filtro global solo para usuarios autenticados
            var politicaUsuarioAutenticado = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Add services to the container.
            builder.Services.AddControllersWithViews(opciones =>
            {
                opciones.Filters.Add(new AuthorizeFilter(politicaUsuarioAutenticado));
            }).AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(opciones =>
            {
                opciones.DataAnnotationLocalizerProvider = (_, fatoria) =>
                fatoria.Create(typeof(RecursoCompartido));
            });
            //DB conexion
            builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=defaulConnection"));
            //Usuarios
            builder.Services.AddAuthentication();
            builder.Services.AddIdentity<IdentityUser,IdentityRole>(opciones =>
            {
                opciones.SignIn.RequireConfirmedAccount = false;

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            //
            builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
            {
                opciones.LoginPath = "/usuarios/login";
                opciones.AccessDeniedPath = "/usuarios/login";
            });
            //localizacion para que cambie el idioma
            builder.Services.AddLocalization(opciones =>
            {
                opciones.ResourcesPath = "Recursos";
            });

            //
            builder.Services.AddTransient<IServiciosUsuarios, ServicioUsuario>();
            //AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));
            /////
            var app = builder.Build();
           
            //
          
            app.UseRequestLocalization(opciones =>

            {

                opciones.DefaultRequestCulture = new RequestCulture("es");  // ESTA ES LA CULTURA POR DEFECTO.

                opciones.SupportedUICultures = Constantes.CulturasUISoportadas
                    .Select(cultura => new CultureInfo(cultura.Value)).ToList();

            });

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
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}