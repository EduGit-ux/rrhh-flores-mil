using RRHH_Flores_Mil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RRHH_Flores_Mil.Interface
{
    public interface IConsultarVacaciones
    {
        Task<List<AprobarVacaciones>> ConsultaVacaciones();
        Task<string> AprobarRechazar(int id, string decision, string observacion);
    }
}
