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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        /* private PacientesForm parentForm;
        private Paciente pacienteEditado;

        // Constructor para nuevo paciente
        public NuevoPacienteForm(PacientesForm form)
        {
            InitializeComponent();
            parentForm = form;
            clbAlergias.ItemCheck += clbAlergias_ItemCheck;
        }

        // Constructor para edición de paciente
        public NuevoPacienteForm(PacientesForm form, Paciente paciente)
        {
            InitializeComponent();
            parentForm = form;
            pacienteEditado = paciente;
            clbAlergias.ItemCheck += clbAlergias_ItemCheck;
        }

        public NuevoPacienteForm()
        {
            InitializeComponent();
        }

        private void NuevoPacienteForm_Load(object sender, EventArgs e)
        {
            clbAlergias.Items.Clear();
            foreach (var alergia in DatosGlobales.CatalogoAlergias)
                clbAlergias.Items.Add(alergia, false);

            if (pacienteEditado != null)
            {
                txtCedula.Text = pacienteEditado.Cedula;
                txtNombrePaciente.Text = pacienteEditado.Nombre;
                dtpFechaNac.Value = pacienteEditado.FechaNacimiento;
                txtEmailPaciente.Text = pacienteEditado.CorreoElectronico;
                txtTelefonoPaciente.Text = pacienteEditado.Telefono;
                var ids = pacienteEditado.AlergiasDelPaciente.Select(a => a.IDAlergia).ToList();
                for (int i = 0; i < clbAlergias.Items.Count; i++)
                    if (ids.Contains(((Alergia)clbAlergias.Items[i]).IDAlergia))
                        clbAlergias.SetItemChecked(i, true);
            }
        }


        private void txtAlergias_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
        private void clbAlergias_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Referencia al CheckedListBox
            var clb = sender as CheckedListBox;
            if (clb == null) return;

            // Consigue el nombre de la alergia que se está marcando/desmarcando
            var seleccionada = clb.Items[e.Index] as Alergia;
            if (seleccionada == null) return;

            // Si se marca "Ninguna", desmarcar todas las demás
            if (seleccionada.Nombre.Equals("Ninguna", StringComparison.OrdinalIgnoreCase) &&
                e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < clb.Items.Count; i++)
                {
                    if (i != e.Index)
                        clb.SetItemChecked(i, false);
                }
            }
            // Si se va a marcar cualquier otra y "Ninguna" está marcada, desmarcar "Ninguna"
            else if (!seleccionada.Nombre.Equals("Ninguna", StringComparison.OrdinalIgnoreCase) &&
                     e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < clb.Items.Count; i++)
                {
                    var item = clb.Items[i] as Alergia;
                    if (item != null && item.Nombre.Equals("Ninguna", StringComparison.OrdinalIgnoreCase))
                        clb.SetItemChecked(i, false);
                }
            }
        }
        private void btnAgregarAlergia_Click(object sender, EventArgs e)
        {
            string nuevaAlergia = txtNuevaAlergia.Text.Trim();
            if (!string.IsNullOrEmpty(nuevaAlergia))
            {
                if (!DatosGlobales.CatalogoAlergias.Any(al => al.Nombre.Equals(nuevaAlergia, StringComparison.OrdinalIgnoreCase)))
                {
                    int nuevoId = DatosGlobales.CatalogoAlergias.Any() ?
                        DatosGlobales.CatalogoAlergias.Max(al => al.IDAlergia) + 1 : 1;
                    var nueva = new Alergia { IDAlergia = nuevoId, Nombre = nuevaAlergia };
                    DatosGlobales.CatalogoAlergias.Add(nueva);
                    clbAlergias.Items.Add(nueva, true);
                    MessageBox.Show("Alergia añadida correctamente.");
                    txtNuevaAlergia.Clear();
                }
                else
                {
                    MessageBox.Show("Esa alergia ya existe en la lista.");
                }
            }
            else
            {
                MessageBox.Show("Escribe el nombre de la alergia.");
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombrePaciente.Text))
            {
                MessageBox.Show("El campo Nombre es obligatorio."); txtNombrePaciente.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(txtTelefonoPaciente.Text) || txtTelefonoPaciente.Text.Length < 7)
            {
                MessageBox.Show("Agrega un teléfono válido."); txtTelefonoPaciente.Focus(); return false;
            }
            DateTime fechaNac = dtpFechaNac.Value;
            int edad = DateTime.Now.Year - fechaNac.Year;
            if (DateTime.Now < fechaNac.AddYears(edad)) edad--;
            if (fechaNac > DateTime.Now)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser futura."); dtpFechaNac.Focus(); return false;
            }
            if (edad < 5)
            {
                MessageBox.Show("El paciente debe tener al menos 5 años."); dtpFechaNac.Focus(); return false;
            }
            if (edad >= 18)
            {
                if (string.IsNullOrWhiteSpace(txtEmailPaciente.Text) ||
                    !Regex.IsMatch(txtEmailPaciente.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Debes ingresar un email válido para mayores de edad."); txtEmailPaciente.Focus(); return false;
                }
                if (string.IsNullOrWhiteSpace(txtCedula.Text))
                {
                    MessageBox.Show("Debes ingresar la cédula si es mayor de edad."); txtCedula.Focus(); return false;
                }
            }
            if (clbAlergias.CheckedItems.Count == 0)
            {
                MessageBox.Show("Selecciona al menos una alergia."); clbAlergias.Focus(); return false;
            }
            return true;
        }


        private void clbAlergias_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nombres = string.Join(", ", clbAlergias.CheckedItems.Cast<Alergia>().Select(a => a.Nombre));
        }
 

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            DateTime fechaNac = dtpFechaNac.Value;
            int edad = DateTime.Now.Year - fechaNac.Year;
            if (DateTime.Now < fechaNac.AddYears(edad)) edad--;

            string cedula = edad < 18 ? "Menor de edad" : txtCedula.Text.Trim();
            string email = edad < 18 ? "Menor de edad" : txtEmailPaciente.Text.Trim();

            List<AlergiaPaciente> relaciones = clbAlergias.CheckedItems.Cast<Alergia>()
                .Select(al => new AlergiaPaciente { IDAlergia = al.IDAlergia, Alergia = al }).ToList();

            if (pacienteEditado == null)
            {
                int nuevoId = parentForm.listaPacientes.Any() ? parentForm.listaPacientes.Max(p => p.IDPaciente) + 1 : 1;
                Paciente nuevoPaciente = new Paciente
                {
                    IDPaciente = nuevoId,
                    Cedula = cedula,
                    FechaNacimiento = fechaNac,
                    Nombre = txtNombrePaciente.Text.Trim(),
                    Telefono = txtTelefonoPaciente.Text.Trim(),
                    CorreoElectronico = email,
                    AlergiasDelPaciente = relaciones,
                    HistorialMedico = txtHistorialMed.Text.Trim(),  // <-- ESTA LÍNEA
                    FechaRegistro = DateTime.Now
                };
                parentForm.AgregarPaciente(nuevoPaciente);
            }
            else
            {
                pacienteEditado.Cedula = cedula;
                pacienteEditado.FechaNacimiento = fechaNac;
                pacienteEditado.Nombre = txtNombrePaciente.Text.Trim();
                pacienteEditado.Telefono = txtTelefonoPaciente.Text.Trim();
                pacienteEditado.CorreoElectronico = email;
                pacienteEditado.AlergiasDelPaciente = relaciones;
                pacienteEditado.HistorialMedico = txtHistorialMed.Text.Trim();
                parentForm.RefrescarPacientes();
            }

            MessageBox.Show("Datos guardados correctamente.");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtHistorialMed_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTelefonoPaciente_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtEmailPaciente_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNombrePaciente_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCedula_TextChanged(object sender, EventArgs e)
        {

        }

        private void dtpFechaNac_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtNuevaAlergia_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnBuscarAlergia_Click(object sender, EventArgs e)
        {

        }

        private void txtBuscarAlergia_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscarAlergia.Text.Trim().ToLower();

            clbAlergias.Items.Clear();
            // Filtra, pero mantiene los checkeos previos si no fue eliminado
            var idsChecked = new HashSet<int>(
                clbAlergias.CheckedItems.Cast<Alergia>().Select(a => a.IDAlergia)
            );

            // Filtra las alergias por búsqueda
            var filtradas = DatosGlobales.CatalogoAlergias
                .Where(al => al.Nombre.ToLower().Contains(filtro))
                .ToList();

            foreach (var alergia in filtradas)
                clbAlergias.Items.Add(alergia, idsChecked.Contains(alergia.IDAlergia));
        }

        private void btnEliminarAlergia_Click(object sender, EventArgs e)
        {
            if (clbAlergias.CheckedItems.Count == 0)
            {
                MessageBox.Show("Selecciona al menos una alergia para eliminar.");
                return;
            }

            var seleccionadas = clbAlergias.CheckedItems.Cast<Alergia>().ToList();
            List<string> noEliminables = new List<string>();

            // Revisa si alguna alergia seleccionada la tiene algún paciente
            foreach (var alergia in seleccionadas)
            {
                bool enUso = parentForm.listaPacientes.Any(p =>
                p.AlergiasDelPaciente.Any(rel => Convert.ToInt32(rel.IDAlergia) == alergia.IDAlergia));
                if (enUso)
                    noEliminables.Add(alergia.Nombre);
            }

            if (noEliminables.Count > 0)
            {
                MessageBox.Show("No se puede eliminar la(s) alergia(s): "
                    + string.Join(", ", noEliminables)
                    + ".\nActualmente asignada(s) a uno o más pacientes.",
                    "Eliminación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si llegaste aquí es que todas pueden eliminarse con seguridad
            string nombres = string.Join(", ", seleccionadas.Select(a => a.Nombre));
            var confirm = MessageBox.Show(
                $"¿Desea eliminar la(s) siguiente(s) alergia(s): {nombres}?\nEsta acción no se puede deshacer.",
                "Eliminar alergía(s)",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                foreach (var alergia in seleccionadas)
                    DatosGlobales.CatalogoAlergias.Remove(alergia);

                txtBuscarAlergia_TextChanged(txtBuscarAlergia, EventArgs.Empty);

                MessageBox.Show("Alergía(s) eliminada(s) correctamente.");
            }*/
    }
}
