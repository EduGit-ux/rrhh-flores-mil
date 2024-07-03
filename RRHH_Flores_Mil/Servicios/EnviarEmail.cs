using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using MimeKit;
using MimeKit.Utils;
using RRHH_Flores_Mil.Interface;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RRHH_Flores_Mil.Servicios
{
    public class EnviarEmail : IEnviarEmail
    {
        public async Task<string> EnviarEmailJefe(string nombre,string nombreJefe , string detalle, string tipo, string emailJefe, string fechaInicio, string fechaFin, string url)
        {
            string requestSMTP = string.Empty;
            try
            {
                // Crear el mensaje de correo
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Flores Mil", "jorge.shigui123@gmail.com"));
                message.To.Add(new MailboxAddress(nombre, emailJefe));
                message.Subject = "Solicitud de Aprobación";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                <html>
                <body style='color: #121b49;'>
                    <h2>Estimado {nombreJefe}</h2></br>
                    <p>Le informamos que tiene una nueva solicitud de {tipo} de {nombre} pendiente de revisión</p>
                    <p>A continuación, encontrará los detalles de la solicitud: </p> 
                    <p>Datos de la Solicitud: </p>
                    <p>
                        <ol>
                          <li> Empleado: {nombre}</li>
                          <li> Fecha de Inicio: {fechaInicio}</li>
                          <li> Fecha de Fin: {fechaFin}</li>
                          <li> Observación: {detalle}</li>
                        </ol>
                    </p>
                    <p>Puede aprobar la solicitud en el siguiente link:  <a href='{url}'>aquí</a>.</p> 
                    <footer>
                        <p>Saludos cordiales,</p>
                        <p>Equipo de Recursos Humanos</p>
                        <p>Flores Mil</p></br>
                        <p>Nota de Descargo: Este correo electrónico es generado automáticamente. Por favor, no responda a este mensaje.</p>
                    </footer>
                </body>
                </html>",
                    TextBody = $@"
                Hola {nombre},

                {detalle}

                Saludos,
                Tu Nombre"
                };

                //bodyBuilder.LinkedResources.Add(image);
                message.Body = bodyBuilder.ToMessageBody();

                // Configurar el cliente SMTP
                using (var client = new SmtpClient())
                {
                    // Conectar al servidor SMTP
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    // Autenticación
                    client.Authenticate("jorge.shigui123@gmail.com", "tegy zupw uhib guky");

                    // Enviar el mensaje
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }

                requestSMTP = $"notificación enviada!";
            }
            catch (Exception ex)
            {
                requestSMTP = ex.Message + " ex: " + ex.ToString();
            }
            return requestSMTP;
        }

        public async Task<string> EnviarEmailSmtp(string nombre, string detalle, string email, string tipo, string fechaInicio, string fechaFin)
        {
            string requestSMTP = string.Empty;
            try
            {
                // Crear el mensaje de correo
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Flores Mil", "jorge.shigui123@gmail.com"));
                message.To.Add(new MailboxAddress(nombre, email));
                message.Subject = "Solicitud de Aprobación " + tipo;
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                <html>
                <body style='color: #121b49;'>
                    <h2>Estimado/a {nombre} </h2></br>
                    <p>Le informamos que su solicitud de {tipo} ha sido creada exitosamente. </br>
                    A continuación, encontrará los detalles de su solicitud:</p> 
                    <p>Datos de la Solicitud: </p>
                    <p>
                        <ol>
                          <li> Empleado: {nombre}</li>
                          <li> Fecha de Inicio: {fechaInicio}</li>
                          <li> Fecha de Fin: {fechaFin}</li>
                          <li> Observación: {detalle}</li>
                        </ol>
                    </p>
                    <p>Nota: Esta solicitud está sujeta a revisión y aprobación por parte de su jefe de sucursal.</p>
                    <footer>
                        <p>Saludos cordiales,</p>
                        <p>Equipo de Recursos Humanos</p>
                        <p>Flores Mil</p></br>
                        <p>Nota de Descargo: Este correo electrónico es generado automáticamente. Por favor, no responda a este mensaje.</p>
                    </footer>
                </body>
                </html>",
                    TextBody = $@"
                Hola {nombre},

                {detalle}

                Saludos,
                Tu Nombre"
                };

                //bodyBuilder.LinkedResources.Add(image);
                message.Body = bodyBuilder.ToMessageBody();

                // Configurar el cliente SMTP
                using (var client = new SmtpClient())
                {
                    // Conectar al servidor SMTP
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    // Autenticación
                    client.Authenticate("jorge.shigui123@gmail.com", "tegy zupw uhib guky");

                    // Enviar el mensaje
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            requestSMTP = $"notificación enviada!";
            }
            catch (Exception ex)
            {
                requestSMTP = ex.Message +" ex: "+ ex.ToString();
            }
            return requestSMTP;
        }


        public async Task<string> EnviarEmailSmtpDecision(string nombre, string detalle, string email, string decision, string fechaInicio, string fechaFin, string tipo)
        {
            string requestSMTP = string.Empty;
            try
            {
                // Crear el mensaje de correo
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Flores Mil", "jorge.shigui123@gmail.com"));
                message.To.Add(new MailboxAddress(nombre, email));
                message.Subject = "Aprobación o rechazo de solicitud";
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                <html>
                <body style='color: #121b49;'>
                    <h2>Estimado/a {nombre} </h2></br>
                    <p>Le informamos que su solicitud de {tipo} ha sido {decision}.</br>
                     A continuación, encontrará los detalles de la decisión: </p> 
                    <p>Datos de la Solicitud: </p>
                    <p>
                        <ol>
                          <li> Empleado: {nombre}</li>
                          <li> Fecha de Inicio: {fechaInicio}</li>
                          <li> Fecha de Fin: {fechaFin}</li>
                          <li> Fecha de revisión: {DateTime.Now}</li>
                          <li> Observación: {detalle}</li>
                        </ol>
                    </p>
                    <footer>
                        <p>Saludos cordiales,</p>
                        <p>Equipo de Recursos Humanos</p>
                        <p>Flores Mil</p></br>
                        <p>Nota de Descargo: Este correo electrónico es generado automáticamente. Por favor, no responda a este mensaje.</p>
                    </footer>
                </body>
                </html>",
                    TextBody = $@"
                Hola {nombre},

                {detalle}

                Saludos,
                Tu Nombre"
                };

                //bodyBuilder.LinkedResources.Add(image);
                message.Body = bodyBuilder.ToMessageBody();

                // Configurar el cliente SMTP
                using (var client = new SmtpClient())
                {
                    // Conectar al servidor SMTP
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    // Autenticación
                    client.Authenticate("jorge.shigui123@gmail.com", "tegy zupw uhib guky");

                    // Enviar el mensaje
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
                requestSMTP = $"notificación enviada!";
            }
            catch (Exception ex)
            {
                requestSMTP = ex.Message + " ex: " + ex.ToString();
            }
            return requestSMTP;
        }

        public async Task<string> EnviarEmailSmtpRRHH(string nombreEmp, string nombre, string detalle, string email, string decision, string fechaInicio, string fechaFin, string tipo)
        {
            string requestSMTP = string.Empty;
            try
            {
                // Crear el mensaje de correo
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Flores Mil", "jorge.shigui123@gmail.com"));
                message.To.Add(new MailboxAddress(nombre, email));
                message.Subject = "Nueva solicitud aprobada";
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                <html>
                <body style='color: #121b49;'>
                    <h2>Estimado/a {nombre} </h2></br>
                    <p>Le informamos que una nueva solicitud de {tipo} ha sido aprobada.</br>
                     A continuación, encontrará los detalles de la decisión: </p> 
                    <p>Datos de la Solicitud: </p>
                    <p>
                        <ol>
                          <li> Empleado: {nombreEmp}</li>
                          <li> Fecha de Inicio: {fechaInicio}</li>
                          <li> Fecha de Fin: {fechaFin}</li>
                          <li> Observación: {detalle}</li>
                        </ol>
                    </p>
                    <p>Por favor, planifique y tome las acciones necesarias.</p>
                    <footer>
                        <p>Saludos cordiales,</p>
                        <p>Equipo de Recursos Humanos</p>
                        <p>Flores Mil</p></br>
                        <p>Nota de Descargo: Este correo electrónico es generado automáticamente. Por favor, no responda a este mensaje.</p>
                    </footer>
                </body>
                </html>",
                    TextBody = $@"
                Hola {nombre},

                {detalle}

                Saludos,
                Tu Nombre"
                };

                //bodyBuilder.LinkedResources.Add(image);
                message.Body = bodyBuilder.ToMessageBody();

                // Configurar el cliente SMTP
                using (var client = new SmtpClient())
                {
                    // Conectar al servidor SMTP
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    // Autenticación
                    client.Authenticate("jorge.shigui123@gmail.com", "tegy zupw uhib guky");

                    // Enviar el mensaje
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
                requestSMTP = $"notificación enviada!";
            }
            catch (Exception ex)
            {
                requestSMTP = ex.Message + " ex: " + ex.ToString();
            }
            return requestSMTP;
        }
    }
}
