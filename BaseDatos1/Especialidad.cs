using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDatos1
{
    public class Especialidad
    {
        public int IDEspecialidad { get; set; }
        public string Nombre { get; set; }
        public override string ToString() => Nombre;
    }
}
