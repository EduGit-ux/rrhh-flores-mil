using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RRHH_Flores_Mil.Models;
using RRHH_Flores_Mil.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RRHH_Flores_Mil.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            ////var existingUser = Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            ///
            if (string.IsNullOrEmpty(user.Usuario) || string.IsNullOrEmpty(user.Contraseña))
            {
                TempData["ErrorMessage"] = "Campos vacios.!! ";
                return RedirectToAction("Login", "Account");
            }
            PostgreSQLConnection postgreSQLExample = new PostgreSQLConnection();
            List<User> rsp = await postgreSQLExample.Login(user.Usuario, user.Contraseña);
            if (rsp.Count == 0 || (string.IsNullOrEmpty(user.Usuario) || string.IsNullOrEmpty(user.Contraseña)))
            {
                TempData["ErrorMessage"] = "Usuario o Contraseña Incorrecto";
                return RedirectToAction("Login", "Account");
            }
            else {

                // Guardar datos en la sesión
                HttpContext.Session.SetString("usuario", user.Usuario);
                HttpContext.Session.SetString("contrasena", user.Contraseña);
                HttpContext.Session.SetString("nombrecompleto", rsp[0].NombreCompleto);
                HttpContext.Session.SetString("email", rsp[0].Email);
                HttpContext.Session.SetInt32("codigo", rsp[0].Codigo);
                HttpContext.Session.SetString("FechaContrato", rsp[0].FechaContrato.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString("Sucursal", rsp[0].Sucursal);
                HttpContext.Session.SetString("DiasPendiente", rsp[0].DiasPendientes.ToString());

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Usuario)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                
                if (rsp[0].Rol.ToUpper().Equals("EMPLEADO"))
                    return RedirectToAction("Request", "Vacaciones");
                return RedirectToAction("Response", "Vacaciones");                
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
