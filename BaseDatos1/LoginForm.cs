using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseDatos1
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        /*/public Usuario UsuarioAutenticado { get; private set; }
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            cmbUsuario.DataSource = DatosGlobales.ListaUsuarios.ToList();
            cmbUsuario.DisplayMember = "Nombre";
            cmbUsuario.ValueMember = "IDUsuario";
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            var seleccionado = cmbUsuario.SelectedItem as Usuario;
            string pass = txtContrasena.Text.Trim();

            if (seleccionado == null)
            {
                MessageBox.Show("Selecciona un usuario.");
                return;
            }

            if (seleccionado.Password != pass)
            {
                MessageBox.Show("Contraseña incorrecta.");
                return;
            }

            UsuarioAutenticado = seleccionado;
            this.DialogResult = DialogResult.OK;
            this.Close();

        }
        //*/
    }
}
