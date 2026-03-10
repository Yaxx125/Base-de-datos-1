using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace BaseDatos1
{
    public class Usuario
    {
        public int IDUsuario { get; set; }
        public string Nombre { get; set; }      // nombre de usuario / login
        public string Password { get; set; }    // Se encripta
        public string Rol { get; set; }         // "Admin", "Recepcionista", "Odontologo"

        public static List<Usuario> ListaUsuarios = new List<Usuario>
        {
            new Usuario { IDUsuario = 1, Nombre = "Adin",    Password = "123", Rol = "Admin" },
            new Usuario { IDUsuario = 2, Nombre = "Recepcionista",    Password = "123", Rol = "Recepcionista" },
            new Usuario { IDUsuario = 3, Nombre = "Odontólogo",     Password = "123", Rol = "Odontologo" },
        };
    }
}
