using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RRHH_Flores_Mil.Interface
{
    public interface IEnviarEmail
    {
        Task<string> EnviarEmailSmtp(string nombre, string detalle, string email, string tipo, string fechaInicio, string fechaFin);
        Task<string> EnviarEmailJefe(string nombre, string nombreJefe, string detalle, string tipo, string emailJefe, string fechaInicio, string fechaFin, string url);
        Task<string> EnviarEmailSmtpDecision(string nombre, string detalle, string email, string decision, string fechaInicio, string fechaFin, string tipo);
        Task<string> EnviarEmailSmtpRRHH(string nombreEmp, string nombre, string detalle, string email, string decision, string fechaInicio, string fechaFin, string tipo);

    }
}
