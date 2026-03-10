using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace BaseDatos1
{
        public class Alergia
        {
            public int IDAlergia { get; set; }
            public string Nombre { get; set; }
            public override string ToString() => Nombre;
        }
}
