using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RRHH_Flores_Mil.Models
{
    public class User
    {
        public int Codigo { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaContrato { get; set; }
        public string Sucursal { get; set; }
        public int DiasPendientes { get; set; }
    }
}
