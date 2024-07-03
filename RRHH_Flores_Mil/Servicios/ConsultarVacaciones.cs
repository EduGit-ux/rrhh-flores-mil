using RRHH_Flores_Mil.Interface;
using RRHH_Flores_Mil.Models;
using RRHH_Flores_Mil.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RRHH_Flores_Mil.Servicios
{
    public class ConsultarVacaciones : IConsultarVacaciones
    {
        PostgreSQLConnection postgreSQLConnection = new PostgreSQLConnection();
        public async Task<string> AprobarRechazar(int id, string decision, string observacion)
        {
            await postgreSQLConnection.AprobarRechazarSolicitud(id, decision, observacion);
            return string.Empty;
        }

        public async Task<List<AprobarVacaciones>> ConsultaVacaciones()
        {
            return  await postgreSQLConnection.DetallesEmpleadoVacaciones("pendiente");
        }
    }
}
