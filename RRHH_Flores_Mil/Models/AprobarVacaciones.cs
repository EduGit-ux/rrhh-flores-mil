using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RRHH_Flores_Mil.Models
{
    public class AprobarVacaciones
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Aprobado { get; set; }
        public string Observacion { get; set; }
    }
}
