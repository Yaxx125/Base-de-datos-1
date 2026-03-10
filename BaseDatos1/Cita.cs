using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDatos1
{
    public class Cita
    {
        public int IDCita { get; set; }
        public DateTime Fecha { get; set; }           // Solo día
        public TimeSpan Hora { get; set; }           // Hora de inicio
        public int IDPaciente { get; set; }
        public int IDOdontologo { get; set; }
        public int IDServicio { get; set; }

        public int IDEstado { get; set; }            // 1=Pendiente, 2=En proceso, 3=Realizado
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? HoraFinal { get; set; }     // null hasta que se marque Realizado
        public int IDUsuario { get; set; }           // lo llenaré cuando tenga gestión de usuarios

        public string NombrePaciente
        {
            get
            {
                var p = DatosGlobales.ListaPacientes.FirstOrDefault(x => x.IDPaciente == IDPaciente);
                return p != null ? p.Nombre : "";
            }
        }

        public string NombreOdontologo
        {
            get
            {
                var o = DatosGlobales.ListaOdontologos.FirstOrDefault(x => x.IDOdontologo == IDOdontologo);
                return o != null ? o.Nombre : "";
            }
        }

        public string NombreServicio
        {
            get
            {
                var s = DatosGlobales.CatalogoServicios.FirstOrDefault(x => x.IDServicio == IDServicio);
                return s != null ? s.Nombre : "";
            }
        }

        public string NombreEstado
        {
            get
            {
                string nombre;
                if (DatosGlobales.EstadosCita != null &&
                    DatosGlobales.EstadosCita.TryGetValue(IDEstado, out nombre))
                    return nombre;
                return "";
            }
        }

        // Hora final aproximada (30–65 min después)
        public TimeSpan HoraFinalAproximada
        {
            get
            {
                // Si ya tienes HoraFinal real, usa su hora
                if (HoraFinal.HasValue)
                    return HoraFinal.Value.TimeOfDay;

                int minutosExtra = 60 + (IDCita % 36); // 30–65 min aprox.
                var horaFin = Hora.Add(TimeSpan.FromMinutes(minutosExtra));
                return horaFin;
            }
        }
        public string ProcedimientoServicio
        {
            get { return Descripcion; }
        }
    }
}
