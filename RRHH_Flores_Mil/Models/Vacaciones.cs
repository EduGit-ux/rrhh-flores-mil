using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RRHH_Flores_Mil.Models
{
    public class Vacaciones
    {
        public string Nombre { get; set; }
        public List<SelectListItem> Tipo { get; set; }
        public string SelectedOption { get; set; }
        public List<SelectListItem> JefeSucursal { get; set; }
        public string SelectJefeSucursal { get; set; }
        public string Sucursal { get; set; }
        public string FechaContrato { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string DiasPendiente { get; set; }
        public string Observación { get; set; }
        
    }    
}
