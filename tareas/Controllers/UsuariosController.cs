using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using tareas.Migrations;
using tareas.Models;
using tareas.Servicios;

namespace tareas.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;

        public UsuariosController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Email };

            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string mensaje = null)
        {
            if (mensaje is not null)
            {
                ViewData["mensaje"] = mensaje;
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o passsword incorrecto.");
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        //Registro externo E: google o microsoft
        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult LoginExterno(string proveedor, string urlRetorno = null)
        {
            var urlDireccion = Url.Action("RegistrarUsuarioExterno", values: new { urlRetorno });
            var propiedades = signInManager
                .ConfigureExternalAuthenticationProperties(proveedor, urlDireccion);
            return new ChallengeResult(proveedor, propiedades);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuarioExterno(string urlRetorno = null, string remoteError = null)
        {
            urlRetorno = urlRetorno ?? Url.Content("~/");
            var mensaje = "";

            if (remoteError is not null)
            {
                mensaje = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info is not null)
            {
                mensaje = $"Error cargando la data del login externo";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            //Ya la cuenta existe
            if (resultadoLoginExterno.Succeeded)
            {
                return LocalRedirect(urlRetorno);
            }

            string email = "";

            if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                mensaje = "Error leyendo el email del usuario del proveedor";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var usuario = new IdentityUser { Email = email, UserName = email };
            var resultadoCrearUsuario = await userManager.CreateAsync(usuario);

            if (!resultadoCrearUsuario.Succeeded)
            {
                mensaje = resultadoCrearUsuario.Errors.First().Description;
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var resultadoAgregarLogin = await userManager.AddLoginAsync(usuario, info);
            if (resultadoAgregarLogin.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);
                return LocalRedirect(urlRetorno);
            }

            mensaje = "ha ocurrido un error agregando el Login";
            return RedirectToAction("login", routeValues: new { mensaje });
        }

        //
        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Listado(string mensaje = null)
        {
            var usuario = await context.Users.Select(u => new UsuarioViewModel
            {
                Email = u.Email
            }).ToListAsync();

            var modelo = new UsusariosListadoViewModel();
            modelo.Usuarios = usuario;
            modelo.Mensaje = mensaje;
            return View(modelo);

        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> HacerAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (usuario is null)
            {
                return NotFound();
            }
            await userManager.AddToRoleAsync(usuario, Constantes.RolAdmin);
            return RedirectToAction("Listado", routeValues: new { mensaje = "Rol asignado correctamente a " + email });
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> RemoverAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (usuario is null)
            {
                return NotFound();
            }
            await userManager.RemoveFromRoleAsync(usuario, Constantes.RolAdmin);
            return RedirectToAction("Listado", routeValues: new { mensaje = "Rol removido correctamente a " + email });
        }
    }
}
