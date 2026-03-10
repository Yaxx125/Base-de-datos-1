using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDatos1
{
    public class Odontologo
    {
        public int IDOdontologo { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }
        public string HistorialLaboral { get; set; }
        public List<int> IDEspecialidades { get; set; } = new List<int>();
        public DateTime FechaRegistro { get; set; }
    }
}
