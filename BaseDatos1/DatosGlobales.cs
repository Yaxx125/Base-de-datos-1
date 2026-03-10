
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;


namespace BaseDatos1
{
    public static class DatosGlobales
    {
        public static List<Paciente> ListaPacientes = new List<Paciente>();
        public static List<Odontologo> ListaOdontologos = new List<Odontologo>();

        public static List<Alergia> CatalogoAlergias = new List<Alergia>
        {
            new Alergia { IDAlergia = 1, Nombre = "Ninguna" },
            new Alergia { IDAlergia = 2, Nombre = "Penicilina" },
            new Alergia { IDAlergia = 3, Nombre = "Aspirina" },
            new Alergia { IDAlergia = 4, Nombre = "Látex" },
            new Alergia { IDAlergia = 5, Nombre = "Sulfa" }
        };

        public static List<Especialidad> CatalogoEspecialidades = new List<Especialidad>
        {
            new Especialidad { IDEspecialidad = 1, Nombre = "Ortodoncia" },
            new Especialidad { IDEspecialidad = 2, Nombre = "Periodoncia" },
            new Especialidad { IDEspecialidad = 3, Nombre = "Endodoncia" }
        };

        public static List<Servicio> CatalogoServicios = new List<Servicio>
        {
            new Servicio { IDServicio = 1, Nombre = "Limpieza dental" },
            new Servicio { IDServicio = 2, Nombre = "Resina" },
            new Servicio { IDServicio = 3, Nombre = "Extracción de muela" }
        };
        public static List<EspecialidadServicio> ListaEspecialidadServicio = new List<EspecialidadServicio>
    {
            // Ortodoncia
            new EspecialidadServicio { IDEspecialidad = 1, IDServicio = 1 }, // Limpieza dental
            new EspecialidadServicio { IDEspecialidad = 1, IDServicio = 2 }, // Resina

            // Periodoncia
            new EspecialidadServicio { IDEspecialidad = 2, IDServicio = 1 }, // Limpieza dental
            new EspecialidadServicio { IDEspecialidad = 2, IDServicio = 3 }, // Extracción de muela

            // Endodoncia
            new EspecialidadServicio { IDEspecialidad = 3, IDServicio = 2 }, // Resina
            new EspecialidadServicio { IDEspecialidad = 3, IDServicio = 3 }  // Extracción de muela
        };
        public static List<Cita> ListaCitas = new List<Cita>();

        // Catálogo simple de estados de cita
        public static Dictionary<int, string> EstadosCita = new Dictionary<int, string>
        {
            { 1, "Pendiente" },
            { 2, "En proceso" },
            { 3, "Realizado" },
            { 4, "Cancelada" }
        };
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public static string RutaDatosJson = "datos_clinica.json";

        public static void GuardarEnJson(string ruta = null)
        {
            if (ruta == null) ruta = RutaDatosJson;

            var datos = new DatosPersistentes
            {
                Pacientes = ListaPacientes,
                Odontologos = ListaOdontologos,
                Citas = ListaCitas,
                Alergias = CatalogoAlergias,
                Especialidades = CatalogoEspecialidades,
                Servicios = CatalogoServicios,
                EspecialidadServicio = ListaEspecialidadServicio
            };

            string json = JsonSerializer.Serialize(datos, JsonOptions);
            File.WriteAllText(ruta, json, Encoding.UTF8);
        }

        public static void CargarDesdeJson(string ruta = null)
        {
            if (ruta == null) ruta = RutaDatosJson;
            if (!File.Exists(ruta)) return;

            string json = File.ReadAllText(ruta, Encoding.UTF8);
            var datos = JsonSerializer.Deserialize<DatosPersistentes>(json, JsonOptions);
            if (datos == null) return;

            ListaPacientes = datos.Pacientes ?? new List<Paciente>();
            ListaOdontologos = datos.Odontologos ?? new List<Odontologo>();
            ListaCitas = datos.Citas ?? new List<Cita>();
            CatalogoAlergias = datos.Alergias ?? CatalogoAlergias;
            CatalogoEspecialidades = datos.Especialidades ?? CatalogoEspecialidades;
            CatalogoServicios = datos.Servicios ?? CatalogoServicios;
            ListaEspecialidadServicio = datos.EspecialidadServicio ?? ListaEspecialidadServicio;
        }

        public static List<Usuario> ListaUsuarios = new List<Usuario>
        {
            new Usuario { IDUsuario = 1, Nombre = "Admin", Password = "123", Rol = "Admin" },
            new Usuario { IDUsuario = 2, Nombre = "Recepcionista", Password = "123", Rol = "Recepcionista" },
            new Usuario { IDUsuario = 3, Nombre = "Odontólogo",  Password = "123", Rol = "Odontologo" }
        };
    }
}
