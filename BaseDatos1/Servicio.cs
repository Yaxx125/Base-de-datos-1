using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDatos1
{
    public class Servicio
    {
        public int IDServicio { get; set; }
        public string Nombre { get; set; }
        public string Procedimiento { get; set; }
        public override string ToString() => Nombre;
    }
}
