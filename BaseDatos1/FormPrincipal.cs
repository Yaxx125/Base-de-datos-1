using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace BaseDatos1
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            SqlConnection cn = new SqlConnection(conexion);

            try
            {
                cn.Open();
                MessageBox.Show("Conexión exitosa a la base de datos");
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
