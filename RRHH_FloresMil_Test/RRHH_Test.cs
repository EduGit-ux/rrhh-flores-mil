using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using RRHH_Flores_Mil.Controllers;
using RRHH_Flores_Mil.Interface;
using RRHH_Flores_Mil.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using RRHH_Flores_Mil.Repositorio;

namespace RRHH_Flores_Mil.Tests
{
    [TestFixture]
    public class VacacionesControllerTests
    {
        private Mock<IConsultarVacaciones> _mockConsultarVacaciones;
        private Mock<IEnviarEmail> _mockEnviarEmail;
        private Mock<ISession> _mockSession;
        private VacacionesController _controller;
        private DefaultHttpContext _httpContext;        

        [SetUp]
        public void SetUp()
        {            
            _mockConsultarVacaciones = new Mock<IConsultarVacaciones>();
            _mockEnviarEmail = new Mock<IEnviarEmail>();
            _mockConsultarVacaciones.Setup(x => x.ConsultaVacaciones()).ReturnsAsync(new List<AprobarVacaciones>() { 
                new AprobarVacaciones(){ Nombre = "Carlos", Aprobado = false },
                new AprobarVacaciones(){ Nombre = "Luis", Aprobado = false }
            });
            _mockEnviarEmail.Setup(x => x.EnviarEmailSmtpDecision(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)).ReturnsAsync("Ok");
            _controller = new VacacionesController(_mockConsultarVacaciones.Object, _mockEnviarEmail.Object);

            _httpContext = new DefaultHttpContext();
            _mockSession = new Mock<ISession>();
            _httpContext.Session = _mockSession.Object;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };
        }

        [Test]
        public async Task Request_Get_Vacaciones()
        {
            // Arrange
            var reps = _controller.Response(string.Empty);

            Assert.IsNotNull(reps);
        }

        [Test]
        public async Task Request_Post_Fechas_Invalidas()
        {
            // Arrange
            var request = new Vacaciones
            {
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(-1) // FechaFin es anterior a FechaInicio
            };
            _controller.ModelState.AddModelError("FechaFin", "FechaFin debe ser posterior a FechaInicio");

            // Act
            var result = await _controller.Request(request) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Rango de fechas incorrectas.", result.ViewData["Message"]);
        }

        [Test]
        public async Task Request_Send_Email()
        {

            // ACt
            var rpsEmail = _controller.enviarNotificacionesFinal(string.Empty, 1, "ACEPTADO");
            // Assert
            Assert.IsNotNull(rpsEmail);
        }
    }
}
