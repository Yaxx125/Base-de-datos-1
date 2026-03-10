using BaseDatos1;
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
    public partial class NuevaCitaForm : Form
    {

        private Cita citaEditada;

        public NuevaCitaForm(Cita citaAEditar)
        {
            InitializeComponent();
            citaEditada = citaAEditar;
        }

        public NuevaCitaForm()
        {
            InitializeComponent();
        }

        /*private void NuevaCitaForm_Load(object sender, EventArgs e)
        {
            InicializarCombos();

        dtpFechaCita.Format = DateTimePickerFormat.Short;
            dtpFechaCita.Value = DateTime.Today;

            dtpHoraCita.Format = DateTimePickerFormat.Time;
            dtpHoraCita.ShowUpDown = true;

            if (citaEditada != null)
            {
                // Seleccionar paciente
                var pac = DatosGlobales.ListaPacientes
                    .FirstOrDefault(p => p.IDPaciente == citaEditada.IDPaciente);
                if (pac != null) cmbPacienteCita.SelectedItem = pac;

                // Seleccionar odontólogo
                var odon = DatosGlobales.ListaOdontologos
                    .FirstOrDefault(o => o.IDOdontologo == citaEditada.IDOdontologo);
                if (odon != null) cmbOdontologoCita.SelectedItem = odon;

                // Forzar filtrado de servicios según el odontólogo
                cmbOdontologoCita_SelectedIndexChanged(null, EventArgs.Empty);

        // Seleccionar servicio
        var serv = DatosGlobales.CatalogoServicios
            .FirstOrDefault(s => s.IDServicio == citaEditada.IDServicio);
                if (serv != null) cmbServicioCita.SelectedItem = serv;

                // Fecha y hora actuales de la cita
                dtpFechaCita.Value = citaEditada.Fecha;
                dtpHoraCita.Value = citaEditada.Fecha.Date + citaEditada.Hora;

                // Descripción
                txtDescripcionCita.Text = citaEditada.Descripcion;
            }
}


private void InicializarCombos()
{
    // Pacientes
    cmbPacienteCita.DataSource = DatosGlobales.ListaPacientes.ToList();
    cmbPacienteCita.DisplayMember = "Nombre";
    cmbPacienteCita.ValueMember = "IDPaciente";

    // Odontólogos
    cmbOdontologoCita.DataSource = DatosGlobales.ListaOdontologos.ToList();
    cmbOdontologoCita.DisplayMember = "Nombre";
    cmbOdontologoCita.ValueMember = "IDOdontologo";

    // Servicios iniciales (se ajustan al seleccionar odontólogo)
    cmbServicioCita.DataSource = DatosGlobales.CatalogoServicios.ToList();
    cmbServicioCita.DisplayMember = "Nombre";
    cmbServicioCita.ValueMember = "IDServicio";
}

private void cmbOdontologoCita_SelectedIndexChanged(object sender, EventArgs e)
{
    var odon = cmbOdontologoCita.SelectedItem as Odontologo;
    if (odon == null) return;

    var idsEsp = odon.IDEspecialidades ?? new System.Collections.Generic.List<int>();

    var idsServiciosPermitidos = DatosGlobales.ListaEspecialidadServicio
        .Where(rel => idsEsp.Contains(rel.IDEspecialidad))
        .Select(rel => rel.IDServicio)
        .Distinct()
        .ToList();

    var serviciosFiltrados = DatosGlobales.CatalogoServicios
        .Where(s => idsServiciosPermitidos.Contains(s.IDServicio))
        .ToList();

    cmbServicioCita.DataSource = serviciosFiltrados;
    cmbServicioCita.DisplayMember = "Nombre";
    cmbServicioCita.ValueMember = "IDServicio";
}

private void btnGuardarCita_Click(object sender, EventArgs e)
{
    // 1) Validar selección de entidades
    var paciente = cmbPacienteCita.SelectedItem as Paciente;
    if (paciente == null)
    {
        MessageBox.Show("Debes seleccionar un paciente.");
        return;
    }

    var odontologo = cmbOdontologoCita.SelectedItem as Odontologo;
    if (odontologo == null)
    {
        MessageBox.Show("Debes seleccionar un odontólogo.");
        return;
    }

    var servicio = cmbServicioCita.SelectedItem as Servicio;
    if (servicio == null)
    {
        MessageBox.Show("Debes seleccionar un servicio.");
        return;
    }

    // 2) Fecha/hora de la cita
    DateTime fecha = dtpFechaCita.Value.Date;
    TimeSpan hora = dtpHoraCita.Value.TimeOfDay;
    DateTime fechaHoraCita = fecha + hora;

    // 3) No pasada
    if (fechaHoraCita < DateTime.Now)
    {
        MessageBox.Show("La fecha y hora de la cita no pueden ser en el pasado.");
        dtpFechaCita.Focus();
        return;
    }

    // 3.1) Rango de horario de la clínica (8:30 a 19:30)
    TimeSpan horaApertura = new TimeSpan(8, 30, 0);
    TimeSpan horaCierre = new TimeSpan(18, 30, 0); // 7:30 pm

    if (hora < horaApertura || hora > horaCierre)
    {
        MessageBox.Show("La cita debe estar dentro del horario de atención: 8:30 am a 7:30 pm.");
        dtpHoraCita.Focus();
        return;
    }

    // 4) Citas existentes de ese odontólogo
    var citasOdontologo = DatosGlobales.ListaCitas
        .Where(c => c.IDOdontologo == odontologo.IDOdontologo
                    && (citaEditada == null || c.IDCita != citaEditada.IDCita)) // excluirse a sí misma si es edición
        .OrderBy(c => c.Fecha.Add(c.Hora))
        .ToList();

    if (citasOdontologo.Any())
    {
        var ultimaCitaAnterior = citasOdontologo
            .Where(c => c.Fecha.Add(c.Hora) <= fechaHoraCita)
            .LastOrDefault();

        if (ultimaCitaAnterior != null)
        {
            DateTime horaFinalUltima;
            if (ultimaCitaAnterior.HoraFinal.HasValue)
            {
                horaFinalUltima = ultimaCitaAnterior.HoraFinal.Value;
            }
            else
            {
                int minutosExtra = 30 + (ultimaCitaAnterior.IDCita % 36); // 30–65 min
                horaFinalUltima = ultimaCitaAnterior.Fecha.Date +
                                  ultimaCitaAnterior.Hora.Add(TimeSpan.FromMinutes(minutosExtra));
            }

            if (fechaHoraCita <= horaFinalUltima)
            {
                MessageBox.Show(
                    "La nueva cita debe iniciar después de la hora de finalización aproximada de la cita anterior de este odontólogo.");
                return;
            }
        }
    }

    // 5) Evitar misma fecha/hora exacta por seguridad
    bool hayConflicto = DatosGlobales.ListaCitas.Any(c =>
        c.IDOdontologo == odontologo.IDOdontologo &&
        c.Fecha.Date == fecha &&
        c.Hora == hora &&
        (citaEditada == null || c.IDCita != citaEditada.IDCita) && // excluirse a sí misma
        c.IDEstado != 3 && c.IDEstado != 4);                        // por ejemplo, 3=Realizado, 4=Cancelada

    if (hayConflicto)
    {
        MessageBox.Show("El odontólogo ya tiene una cita exactamente en ese horario.");
        return;
    }

    // 6) Crear o editar la cita (if citaEditada == null ... else ...)


    // 6) NUEVA o EDICIÓN según citaEditada
    if (citaEditada == null)
    {
        // NUEVA CITA
        int nuevoId = DatosGlobales.ListaCitas.Any()
            ? DatosGlobales.ListaCitas.Max(c => c.IDCita) + 1
            : 1;

        var cita = new Cita
        {
            IDCita = nuevoId,
            Fecha = fecha,
            Hora = hora,
            IDPaciente = paciente.IDPaciente,
            IDOdontologo = odontologo.IDOdontologo,
            IDServicio = servicio.IDServicio,
            IDEstado = 1, // Pendiente
            Descripcion = txtDescripcionCita.Text.Trim(),
            FechaRegistro = DateTime.Now,
            HoraFinal = null,
            IDUsuario = 0
        };

        DatosGlobales.ListaCitas.Add(cita);
        MessageBox.Show("Cita creada correctamente.");
    }
    else
    {
        // REPROGRAMAR / EDITAR
        citaEditada.Fecha = fecha;
        citaEditada.Hora = hora;
        citaEditada.IDPaciente = paciente.IDPaciente;
        citaEditada.IDOdontologo = odontologo.IDOdontologo;
        citaEditada.IDServicio = servicio.IDServicio;
        citaEditada.Descripcion = txtDescripcionCita.Text.Trim();
        citaEditada.IDEstado = 1;    // opcional: volver a Pendiente
        citaEditada.HoraFinal = null;

        MessageBox.Show("Cita reprogramada correctamente.");
    }

    this.DialogResult = DialogResult.OK;
    this.Close();
}


private void btnCancelarCita_Click(object sender, EventArgs e)
{
    this.DialogResult = DialogResult.Cancel;
    this.Close();
}/*/

    }
}
