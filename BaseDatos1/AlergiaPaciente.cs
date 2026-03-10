using BaseDatos1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDatos1
{

        public class AlergiaPaciente
        {
            public int IDPaciente { get; set; }
            public Alergia Alergia { get; set; }
            public object IDAlergia { get; internal set; }
        }
}
