using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseDatos1
{
    public partial class NuevoOdontologoForm : Form
    {
        public NuevoOdontologoForm()
        {
            InitializeComponent();
        }

        /*private PacientesForm parentForm;
        private Odontologo odontologoEditado;

        // Constructor para NUEVO odontólogo
        public NuevoOdontologoForm(PacientesForm form)
        {
            InitializeComponent();
            parentForm = form;
        }

        // Constructor para EDICIÓN de odontólogo
        public NuevoOdontologoForm(PacientesForm form, Odontologo odontologo)
        {
            InitializeComponent();
            parentForm = form;
            odontologoEditado = odontologo;
        }

        public NuevoOdontologoForm()
        {
            InitializeComponent();

        }

        private void NuevoOdontologoForm_Load(object sender, EventArgs e)
        {
            clbEspecialidades.Items.Clear();
            foreach (var esp in DatosGlobales.CatalogoEspecialidades)
                clbEspecialidades.Items.Add(esp, false);

            if (odontologoEditado != null)
            {
                txtCedulaOdontologo.Text = odontologoEditado.Cedula;
                txtNombreOdontologo.Text = odontologoEditado.Nombre;
                txtTelefonoOdontologo.Text = odontologoEditado.Telefono;
                txtEmailOdontologo.Text = odontologoEditado.CorreoElectronico;

                // AQUI:
                for (int i = 0; i < clbEspecialidades.Items.Count; i++)
                {
                    var eobj = clbEspecialidades.Items[i] as Especialidad;
                    if (eobj != null && odontologoEditado.IDEspecialidades.Contains(eobj.IDEspecialidad))
                        clbEspecialidades.SetItemChecked(i, true);
                }
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombreOdontologo.Text))
            {
                MessageBox.Show("El campo Nombre es obligatorio."); txtNombreOdontologo.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(txtCedulaOdontologo.Text))
            {
                MessageBox.Show("El campo Cédula es obligatorio."); txtCedulaOdontologo.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(txtTelefonoOdontologo.Text) || txtTelefonoOdontologo.Text.Length < 7)
            {
                MessageBox.Show("Agrega un teléfono válido."); txtTelefonoOdontologo.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmailOdontologo.Text) ||
                !Regex.IsMatch(txtEmailOdontologo.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Ingresa un email válido."); txtEmailOdontologo.Focus(); return false;
            }
            if (dtpFechaRegistroOdontologo.Value > DateTime.Now)
            {
                MessageBox.Show("La fecha de registro no puede ser futura.");
                dtpFechaRegistroOdontologo.Focus();
                return false;
            }

            return true;
        }


        private void dtpFechaNacOdontólogo_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtCedulaOdontologo_TextChanged(object sender, EventArgs e)
        {

        }

        private void clbEspecialidades_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtEmailOdontologo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBuscarEspecialidad_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscarEspecialidad.Text.Trim().ToLower();
            clbEspecialidades.Items.Clear();
            var idsMarcados = new HashSet<int>(
                clbEspecialidades.CheckedItems.Cast<Especialidad>().Select(ei => ei.IDEspecialidad)
            );
            var filtradas = DatosGlobales.CatalogoEspecialidades
                .Where(esp => esp.Nombre.ToLower().Contains(filtro))
                .ToList();
            foreach (var esp in filtradas)
                clbEspecialidades.Items.Add(esp, idsMarcados.Contains(esp.IDEspecialidad));
        }

        private void btnAgregarEspecialidad_Click(object sender, EventArgs e)
        {

        }

        private void btnEliminarEspecialidad_Click(object sender, EventArgs e)
        {

        }

        private void txtNuevaEspecialidad_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTelefonoOdontologo_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCancelarOdont_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardarOdontologo_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            // Aquí va este fragmento:
            List<int> idsEspecialidades = clbEspecialidades.CheckedItems
             .Cast<Especialidad>().Select(esp => esp.IDEspecialidad).ToList();


            if (odontologoEditado == null) // NUEVO
            {
                int nuevoId = parentForm.listaOdontologos.Any() ? parentForm.listaOdontologos.Max(x => x.IDOdontologo) + 1 : 1;

                Odontologo nuevo = new Odontologo
                {
                    IDOdontologo = nuevoId,
                    Nombre = txtNombreOdontologo.Text.Trim(),
                    Cedula = txtCedulaOdontologo.Text.Trim(),
                    Telefono = txtTelefonoOdontologo.Text.Trim(),
                    CorreoElectronico = txtEmailOdontologo.Text.Trim(),
                    IDEspecialidades = idsEspecialidades,
                    HistorialLaboral = txtHistorialLaboral.Text.Trim(),
                    FechaRegistro = dtpFechaRegistroOdontologo.Value
                };

                parentForm.listaOdontologos.Add(nuevo);
                parentForm.RefrescarOdontologos();
            }
            else // EDICIÓN
            {
                odontologoEditado.Nombre = txtNombreOdontologo.Text.Trim();
                odontologoEditado.Cedula = txtCedulaOdontologo.Text.Trim();
                odontologoEditado.Telefono = txtTelefonoOdontologo.Text.Trim();
                odontologoEditado.CorreoElectronico = txtEmailOdontologo.Text.Trim();
                odontologoEditado.IDEspecialidades = idsEspecialidades;
                odontologoEditado.HistorialLaboral = txtHistorialLaboral.Text.Trim();
                odontologoEditado.FechaRegistro = dtpFechaRegistroOdontologo.Value;
                parentForm.RefrescarOdontologos();
            }

            MessageBox.Show("Datos guardados correctamente.");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtHistorialLaboral_TextChanged(object sender, EventArgs e)
        {

        }

        private void dtpFechaRegistroOdontologo_ValueChanged(object sender, EventArgs e)
        {

        }*/
    }
}
