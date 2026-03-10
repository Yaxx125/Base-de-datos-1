using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDatos1
{
    public class Paciente
    {
        public int IDPaciente { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }
        public string HistorialMedico { get; set; }
        public List<AlergiaPaciente> AlergiasDelPaciente { get; set; } = new List<AlergiaPaciente>();
        public DateTime FechaRegistro { get; set; }
    }
}
