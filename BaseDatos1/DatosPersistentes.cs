using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDatos1
{
    public class DatosPersistentes
    {
        public List<Paciente> Pacientes { get; set; } = new List<Paciente>();
        public List<Odontologo> Odontologos { get; set; } = new List<Odontologo>();
        public List<Cita> Citas { get; set; } = new List<Cita>();

        public List<Alergia> Alergias { get; set; } = new List<Alergia>();
        public List<Especialidad> Especialidades { get; set; } = new List<Especialidad>();
        public List<Servicio> Servicios { get; set; } = new List<Servicio>();
        public List<EspecialidadServicio> EspecialidadServicio { get; set; } = new List<EspecialidadServicio>();
    }
}
