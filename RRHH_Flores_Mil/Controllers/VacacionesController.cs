using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RRHH_Flores_Mil.Interface;
using RRHH_Flores_Mil.Models;
using RRHH_Flores_Mil.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RRHH_Flores_Mil.Controllers
{
    public class VacacionesController : Controller
    {
        private readonly IConsultarVacaciones _consultarVacaciones;
        private readonly IEnviarEmail _enviarEmail;
        PostgreSQLConnection postgreSQLExample = new PostgreSQLConnection();
        List<AprobarVacaciones> pendingRequests = new List<AprobarVacaciones>();
        public VacacionesController(IConsultarVacaciones consultarVacaciones, IEnviarEmail enviarEmail)
        {
            _consultarVacaciones = consultarVacaciones;
            _enviarEmail = enviarEmail;
        }

        [HttpGet]
        public async Task<IActionResult> Request()
        {            
            List<string[]> rsp = await postgreSQLExample.ConsultarJefeSucursal(HttpContext.Session.GetString("usuario"),
                HttpContext.Session.GetString("contrasena"));

            if (rsp.Count == 0)
            {
                HttpContext.Session.SetString("usuarioNA", "Usuario no tiene Sucursal o Jefe asigando.");
                return RedirectToAction("Login", "Account");
            }
                

            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < rsp.Count; i++)
            {
                items.Add(new SelectListItem { Value = rsp[i][0].ToString(), Text = rsp[i][1]});
            }
            Vacaciones model = new Vacaciones
            {
                Nombre = HttpContext.Session.GetString("nombrecompleto"),
                Sucursal = HttpContext.Session.GetString("Sucursal") == null ? string.Empty : HttpContext.Session.GetString("Sucursal"),
                DiasPendiente = HttpContext.Session.GetString("DiasPendiente") == null ? string.Empty : HttpContext.Session.GetString("DiasPendiente"),
                FechaContrato = HttpContext.Session.GetString("FechaContrato") == null ? string.Empty : HttpContext.Session.GetString("FechaContrato"),
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now,
                Tipo = new List<SelectListItem>
                {
                    new SelectListItem { Value = "vacaciones", Text = "Vacaciones" },
                    new SelectListItem { Value = "permiso", Text = "Permiso" }
                },
                JefeSucursal = items
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Request(Vacaciones request)
        {
            string mensaje = string.Empty;
            if (request.FechaFin < request.FechaInicio)
            {
                ViewData["Message"] = "Rango de fechas incorrectas.";
                return View(request);
            }
            if (ModelState.IsValid && (!string.IsNullOrEmpty(request.SelectedOption)
                && !string.IsNullOrEmpty(request.SelectJefeSucursal)))
            {
                TimeSpan difference = request.FechaFin.Subtract(request.FechaInicio);

                string response = await postgreSQLExample.InsertarSolicitud((int)HttpContext.Session.GetInt32("codigo"),
                     Convert.ToInt32(request.SelectJefeSucursal),
                   request.FechaInicio, request.FechaFin, request.FechaFin.AddDays(1), difference.Days,
                   string.IsNullOrEmpty(request.Observación) ? string.Empty : request.Observación, request.SelectedOption);

                // Envio notificación empleado
                string send = await _enviarEmail.EnviarEmailSmtp(request.Nombre, string.IsNullOrEmpty(request.Observación) ? string.Empty : request.Observación,
                    HttpContext.Session.GetString("email"), request.SelectedOption,
                    request.FechaInicio.ToString("yyyy-MM-dd"), request.FechaFin.ToString("yyyy-MM-dd"));

                // Envio notificación empleado
                List<string> datosJefe = await postgreSQLExample.ConsultarEmailJefe(Convert.ToInt32(request.SelectJefeSucursal));
                string sendJefe = await _enviarEmail.EnviarEmailJefe(request.Nombre, datosJefe[1], string.IsNullOrEmpty(request.Observación) ? string.Empty : request.Observación,
                    request.SelectedOption, datosJefe[0], request.FechaInicio.ToString(), request.FechaFin.ToString(), datosJefe[2]);

                mensaje = "Solicitud registrado exitosamente, " + send;
            }
            else
                mensaje = "Campos Incompletos!";

            ViewData["Message"] = mensaje;
            request = new Vacaciones()
            { 
                Observación = string.Empty
            };
            return View(request);
        }
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> Response(string searchString)
        {
            if(pendingRequests.Count == 0)
                pendingRequests = await _consultarVacaciones.ConsultaVacaciones();
            if (!string.IsNullOrEmpty(searchString))
            {
                pendingRequests = pendingRequests.Where(i => i.Nombre.Contains(searchString)).ToList();
            }
            ViewData["Message"] = HttpContext.Session.GetString("Estado");

            return View(pendingRequests);
        }
        [HttpPost]
        public async Task<IActionResult> Approve(int id, string observacion)
        {
            try
            {
                await _consultarVacaciones.AprobarRechazar(id, "aprobada", observacion);
                string emailSend = await enviarNotificacionesFinal(observacion, id, "APROBADA");
                if(!string.Equals(emailSend, "OK"))
                    HttpContext.Session.SetString("Estado", emailSend);
                HttpContext.Session.SetString("Estado", "Solicitud aprobada exitosamente!!");

                return RedirectToAction("Response", "Vacaciones");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string observacion)
        {
            await _consultarVacaciones.AprobarRechazar(id, "rechazada", observacion);
            string emailSend = await enviarNotificacionesFinal(observacion, id, "RECHAZADA");
            if (!string.Equals(emailSend, "OK"))
                HttpContext.Session.SetString("Estado", emailSend);
            HttpContext.Session.SetString("Estado", "Solicitud rechazada exitosamente!!");
            return RedirectToAction("Response", "Vacaciones");
        }

        public IActionResult Success()
        {
            return View();
        }

        public async Task<string> enviarNotificacionesFinal(string Observacion, int codigo, string decision)
        {
            try
            {
                List<string> dtlleSolicitud = await postgreSQLExample.SolicitudAproEmpl(codigo);
                // Envio notificación empleado respuesta
                string send = await _enviarEmail.EnviarEmailSmtpDecision(
                    dtlleSolicitud[0],
                    Observacion,
                    dtlleSolicitud[1],
                    decision,
                    dtlleSolicitud[2],
                    dtlleSolicitud[3],
                    dtlleSolicitud[4]);

                //Envio notificación RRHH

                if (decision.Equals("APROBADA"))
                {
                    List<string> datosRRHH = await postgreSQLExample.SolicitudAproRRH();

                    string sendJefe = await _enviarEmail.EnviarEmailSmtpRRHH(
                        dtlleSolicitud[0],
                        datosRRHH[0],
                        Observacion,
                        datosRRHH[1],
                        "APROBADA",
                        dtlleSolicitud[2],
                        dtlleSolicitud[3],
                        dtlleSolicitud[4]);
                }                   
                return "OK";

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
