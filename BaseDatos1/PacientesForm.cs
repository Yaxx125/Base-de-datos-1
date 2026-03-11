using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseDatos1
{
    public partial class PacientesForm : Form
    {
        private Usuario usuarioActual;
        public PacientesForm(Usuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;

            AplicarPermisosPorRol();
        }
        private void AplicarPermisosPorRol()
        {
            string rol = usuarioActual.Rol;

            // Ejemplo de nombres: tabPacientes, tabCitas, tabServicios, tabDoctores, tabOpciones
            // y botones: btnNuevoPaciente, btnNuevaCita, btnNuevoOdontologo, btnOpciones

            if (rol == "Admin")
            {
                // Admin: todo visible
                return;
            }

            if (rol == "Recepcionista")
            {
                // Puede: agendar citas, añadir pacientes
                // No puede: opciones avanzadas, gestión de odontólogos
                tabPage1.Parent = null;   // oculta pestaña Doctores
                tabOpciones.Parent = null;   // oculta pestaña Opciones

                btnNuevoOdontologo.Visible = false;
                // deja visibles btnNuevoPaciente, btnNuevaCita
            }
            else if (rol == "Odontologo")
            {
                // Odontólogo: solo ver sus citas/servicios (por ahora, todas) y DOCTORES si quieres
                tabPage2.Parent = null;  // oculta pestaña Pacientes
                tabOpciones.Parent = null;   // oculta pestaña Opciones

                btnNuevoPaciente.Visible = false;
                btnNuevoOdontologo.Visible = false;
                btnNuevaCita.Visible = false;   // si no quieres que cree citas

                // Opcional: dejar solo pestañas Citas y Servicios
                // tabDoctores.Parent = null; // si tampoco debe ver Doctores
            }
        }


        public List<Paciente> listaPacientes = new List<Paciente>();
        private BindingList<Paciente> pacientesBinding;
        public List<Odontologo> listaOdontologos = new List<Odontologo>();
        private BindingList<Odontologo> odontologosBinding;

        public PacientesForm()
        {
            InitializeComponent();
        }

        private void PacientesForm_Load(object sender, EventArgs e)
        {
            InicializarDatos();
            CargarEspecialidades();
            CargarServicios();
            CargarEspecialidadesEnCheckedList();
            CargarServiciosEnCheckedList();
            ConfigurarDataGridView();
            InicializarOdontologos();
            ConfigurarDataGridViewOdontologos();
            ConfigurarDataGridViewCitas();
            RefrescarPacientes();
            RefrescarCitas();
            RefrescarServicios();
            ActualizarCitasProgramadas();
            ActualizarServiciosDelDia();
        }

        private void ConfigurarDataGridViewOdontologos()
        {
            dgvOdontologos.AutoGenerateColumns = false;
            dgvOdontologos.Columns.Clear();

            dgvOdontologos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IDOdontologo", HeaderText = "ID" });
            dgvOdontologos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre" });
            dgvOdontologos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cedula", HeaderText = "Cédula" });
            dgvOdontologos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Disposicion", HeaderText = "Disposición" });
            dgvOdontologos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefono", HeaderText = "Teléfono" });
            dgvOdontologos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CorreoElectronico", HeaderText = "Email" });

            dgvOdontologos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Especialidad",
                HeaderText = "Especialidades",
                DataPropertyName = "IDESpecialidades", // mapea a la lista
                ReadOnly = true
            });

            dgvOdontologos.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Editar",
                HeaderText = "Editar",
                Text = "Editar",
                UseColumnTextForButtonValue = true
            });

            dgvOdontologos.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Eliminar",
                HeaderText = "Eliminar",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true
            });

            dgvOdontologos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOdontologos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOdontologos.RowHeadersVisible = false;

            this.dgvOdontologos.CellFormatting += dgvOdontologos_CellFormatting;
        }

        private void dgvOdontologos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvOdontologos.Columns[e.ColumnIndex].Name == "Especialidad"
                && e.Value is List<int> ids)
            {
                var nombres = ids
                    .Select(id => DatosGlobales.CatalogoEspecialidades.FirstOrDefault(x => x.IDEspecialidad == id)?.Nombre)
                    .Where(n => !string.IsNullOrEmpty(n));
                e.Value = string.Join(", ", nombres);
            }
        }

        private void InicializarOdontologos()
        {
            listaOdontologos = new List<Odontologo>
            {
            new Odontologo
                {
                    IDOdontologo = 1,
                    Nombre = "Andrea López",
                    Cedula = "OD-123456",
                    Telefono = "+52 55 8765 4321",
                    CorreoElectronico = "andrea.lopez@example.com",
                    IDEspecialidades = new List<int> { 1, 2 },
                    HistorialLaboral = "Egresada de UNAM. 15 años de experiencia. Certificado en ortodoncia avanzada.",
                    FechaRegistro = DateTime.Now
                }
            };
            // Sincronizar con DatosGlobales
            DatosGlobales.ListaOdontologos = listaOdontologos;

            odontologosBinding = new BindingList<Odontologo>(listaOdontologos);
            dgvOdontologos.DataSource = odontologosBinding;

            lblTotalOdontologos.Text = odontologosBinding.Count.ToString();
            RefrescarOdontologos();
        }


        public void RefrescarOdontologos()
        {
            listaOdontologos = DatosGlobales.ListaOdontologos;
            odontologosBinding = new BindingList<Odontologo>(listaOdontologos);
            dgvOdontologos.DataSource = odontologosBinding;
            odontologosBinding = new BindingList<Odontologo>(listaOdontologos);
            dgvOdontologos.DataSource = odontologosBinding;
            lblTotalOdontologos.Text = odontologosBinding.Count.ToString();

            // -- Cálculo odontólogos nuevos en el último mes --
            DateTime hace30dias = DateTime.Now.AddDays(-30);
            int nuevosMes = listaOdontologos.Count(o => o.FechaRegistro >= hace30dias);
            lblCambioMensual.Text = $"+{nuevosMes} en el último mes";
        }


        public void GuardarPacientesAJson(string path)
        {
            var opciones = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(listaPacientes, opciones);
            File.WriteAllText(path, json);
        }
        public void ExportarDatosJSON(string carpeta)
        {
            var opciones = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            string jsonPacientes = System.Text.Json.JsonSerializer.Serialize(listaPacientes, opciones);
            System.IO.File.WriteAllText(System.IO.Path.Combine(carpeta, "pacientes.json"), jsonPacientes);

            string jsonAlergias = System.Text.Json.JsonSerializer.Serialize(DatosGlobales.CatalogoAlergias, opciones);
            System.IO.File.WriteAllText(System.IO.Path.Combine(carpeta, "alergias.json"), jsonAlergias);
        }

        public void ImportarDatosJSON(string carpeta)
        {
            var opciones = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string pathPacientes = System.IO.Path.Combine(carpeta, "pacientes.json");
            string pathAlergias = System.IO.Path.Combine(carpeta, "alergias.json");

            bool huboPacientes = false, huboAlergias = false;

            if (System.IO.File.Exists(pathAlergias))
            {
                string jsonAlergias = System.IO.File.ReadAllText(pathAlergias);
                var alergias = System.Text.Json.JsonSerializer.Deserialize<List<Alergia>>(jsonAlergias, opciones);
                if (alergias != null)
                {
                    DatosGlobales.CatalogoAlergias = alergias;
                    huboAlergias = true;
                }
            }

            if (System.IO.File.Exists(pathPacientes))
            {
                string jsonPacientes = System.IO.File.ReadAllText(pathPacientes);
                var pacientes = System.Text.Json.JsonSerializer.Deserialize<List<Paciente>>(jsonPacientes, opciones);
                if (pacientes != null)
                {
                    listaPacientes = pacientes;
                    RefrescarPacientes();
                    huboPacientes = true;
                }
            }

            if (!huboPacientes && !huboAlergias)
                MessageBox.Show("No se encontraron archivos válidos para importar en esa carpeta.", "Importar JSON", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (huboPacientes && huboAlergias)
                MessageBox.Show("Pacientes y alergias importados correctamente.", "Importar JSON");
            else if (huboPacientes)
                MessageBox.Show("Sólo pacientes importados correctamente (no se encontró alergias.json).", "Importar JSON");
            else if (huboAlergias)
                MessageBox.Show("Sólo alergias importadas correctamente (no se encontró pacientes.json).", "Importar JSON");
        }

        private void InicializarDatos()
        {
            listaPacientes = new List<Paciente>
            {
                new Paciente
                {
                    IDPaciente = 1,
                    Cedula = "123456789",
                    FechaNacimiento = new DateTime(1985, 6, 14),
                    Nombre = "Juan Pérez",
                    Telefono = "+52 55 1234 5678",
                    CorreoElectronico = "juan.perez@example.com",
                    HistorialMedico = "Posee diábetes tipo 2",
                    FechaRegistro = DateTime.Now,
                    AlergiasDelPaciente = new List<AlergiaPaciente>
                    {
                        new AlergiaPaciente { IDAlergia = 2 }
                    }
                },
            };
            // Sincronizar con DatosGlobales
            DatosGlobales.ListaPacientes = listaPacientes;

            pacientesBinding = new BindingList<Paciente>(listaPacientes);
            dgvPacientes.DataSource = pacientesBinding;

            lblTotalPacientes.Text = pacientesBinding.Count.ToString();
            lblCambioSemanal.Text = "+1 desde la semana pasada";
            RefrescarPacientes();
        }

        private void ConfigurarDataGridView()
        {
            dgvPacientes.AutoGenerateColumns = false;
            dgvPacientes.Columns.Clear();

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IDPaciente", HeaderText = "ID" });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cedula", HeaderText = "Cédula" });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FechaNacimiento", HeaderText = "Fecha Nac." });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre" });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefono", HeaderText = "Teléfono" });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CorreoElectronico", HeaderText = "Email" });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Alergias",
                HeaderText = "Alergias",
                DataPropertyName = "AlergiasDelPaciente",
                ReadOnly = true
            });

            dgvPacientes.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Editar",
                HeaderText = "Editar",
                Text = "Editar",
                UseColumnTextForButtonValue = true
            });
            dgvPacientes.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Eliminar",
                HeaderText = "Eliminar",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true
            });

            dgvPacientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPacientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPacientes.RowHeadersVisible = false;
            this.dgvPacientes.CellFormatting += dgvPacientes_CellFormatting;
        }


        // Para mostrar los nombres de alergias en el grid
        private void dgvPacientes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPacientes.Columns[e.ColumnIndex].Name == "Alergias" && e.Value is List<AlergiaPaciente> relaciones)
            {
                e.Value = string.Join(", ",
                    relaciones.Select(rel =>
                    {
                        int idAlergia = Convert.ToInt32(rel.IDAlergia);
                        var nombre = DatosGlobales.CatalogoAlergias.FirstOrDefault(a => a.IDAlergia == idAlergia)?.Nombre;
                        return nombre;
                    })
                    .Where(n => !string.IsNullOrEmpty(n))
                );
            }
        }
        private void dgvPacientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPacientes.Columns[e.ColumnIndex].Name == "Editar")
            {
                Paciente paciente = pacientesBinding[e.RowIndex];
                NuevoPacienteForm editarForm = new NuevoPacienteForm(this, paciente);
                editarForm.ShowDialog();
            }
            else if (dgvPacientes.Columns[e.ColumnIndex].Name == "Eliminar")
            {
                Paciente paciente = pacientesBinding[e.RowIndex];
                var confirm = MessageBox.Show("¿Deseas eliminar a " + paciente.Nombre + "?", "Confirmar", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    listaPacientes.Remove(paciente);
                    RefrescarPacientes();
                }
            }

        }

        public void AgregarPaciente(Paciente paciente)
        {
            listaPacientes.Add(paciente);
            RefrescarPacientes();
        }

        public void RefrescarPacientes()
        {
            listaPacientes = DatosGlobales.ListaPacientes;
            pacientesBinding = new BindingList<Paciente>(listaPacientes);
            dgvPacientes.DataSource = pacientesBinding;
            // Actualizar binding y grid
            pacientesBinding = new BindingList<Paciente>(listaPacientes);
            dgvPacientes.DataSource = pacientesBinding;

            int total = pacientesBinding.Count;

            DateTime hace7dias = DateTime.Now.AddDays(-7);
            int nuevosSemana = listaPacientes.Count(p => p.FechaRegistro >= hace7dias);

            string textoTotal = total.ToString();
            string textoCambio = $"+{nuevosSemana} desde la semana pasada";

            // Panel de la pestaña Pacientes
            lblTotalPacientes.Text = textoTotal;
            lblCambioSemanal.Text = textoCambio;

            // Panel equivalente en pestaña Citas
            lblTotalPacientesCitas.Text = textoTotal;
            lblCambioSemanalCitas.Text = textoCambio;

            lblTotalPacientesServicios.Text = textoTotal;
            lblCambioSemanalServicios.Text = textoCambio;
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void btnNuevoPaciente_Click(object sender, EventArgs e)
        {
            NuevoPacienteForm nuevoForm = new NuevoPacienteForm(this);
            nuevoForm.ShowDialog();
        }

        private void lblNumTratamientos_Click(object sender, EventArgs e)
        {

        }

        private void lblTratamientosActivos_Click(object sender, EventArgs e)
        {

        }

        private void lblNumCitas_Click(object sender, EventArgs e)
        {

        }

        private void lblCitasDía_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalPacientes_Click(object sender, EventArgs e)
        {

        }

        private void lblCambioSemanal_Click(object sender, EventArgs e)
        {

        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string criterio = txtBuscar.Text.ToLower();
            var filtrados = listaPacientes.Where(p =>
                p.Nombre.ToLower().Contains(criterio) ||
                p.Cedula.Contains(criterio) ||
                p.CorreoElectronico.ToLower().Contains(criterio) ||
                p.Telefono.Contains(criterio)
            ).ToList();
            pacientesBinding = new BindingList<Paciente>(filtrados);
            dgvPacientes.DataSource = pacientesBinding;
            lblTotalPacientes.Text = pacientesBinding.Count.ToString();
        }

        private void gbxPacientes_Enter(object sender, EventArgs e)
        {

        }

        private void btnExportarJson_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Exportar datos a JSON";
                sfd.Filter = "Archivos JSON (*.json)|*.json";
                sfd.FileName = "datos_clinica.json";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DatosGlobales.GuardarEnJson(sfd.FileName);
                    MessageBox.Show("Datos exportados correctamente a JSON.");
                }
            }
        }

        private void btnImportarJson_Click_1(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Importar datos desde JSON";
                ofd.Filter = "Archivos JSON (*.json)|*.json";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DatosGlobales.CargarDesdeJson(ofd.FileName);

                    listaPacientes = DatosGlobales.ListaPacientes;
                    listaOdontologos = DatosGlobales.ListaOdontologos;

                    RefrescarPacientes();
                    RefrescarOdontologos();
                    RefrescarCitas();
                    RefrescarServicios();
                    ActualizarCitasProgramadas();
                    ActualizarServiciosDelDia();

                    MessageBox.Show("Datos importados correctamente desde JSON.");
                }
            }
        }


        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void gbxCitasProg_Enter(object sender, EventArgs e)
        {

        }

        private void lblTotalOdontologos_Click(object sender, EventArgs e)
        {

        }

        private void btnNuevoOdontologo_Click(object sender, EventArgs e)
        {
            NuevoOdontologoForm nuevoForm = new NuevoOdontologoForm(this);
            nuevoForm.ShowDialog();
        }

        private void dgvOdontologos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvOdontologos.Columns[e.ColumnIndex].Name == "Editar")
            {
                Odontologo odontologo = odontologosBinding[e.RowIndex];
                NuevoOdontologoForm editarForm = new NuevoOdontologoForm(this, odontologo);
                editarForm.ShowDialog();
            }
            else if (dgvOdontologos.Columns[e.ColumnIndex].Name == "Eliminar")
            {
                Odontologo odontologo = odontologosBinding[e.RowIndex];
                var confirm = MessageBox.Show(
                    $"¿Deseas eliminar al odontólogo {odontologo.Nombre}?",
                    "Confirmar", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.Yes)
                {
                    listaOdontologos.Remove(odontologo);
                    RefrescarOdontologos();
                }
            }
        }

        private void lblCambioMensual_Click(object sender, EventArgs e)
        {

        }

        private void dgvOdontologos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Odontologo odontologo = odontologosBinding[e.RowIndex];

            string esp = string.Join(", ", odontologo.IDEspecialidades
                .Select(id => DatosGlobales.CatalogoEspecialidades.FirstOrDefault(es => es.IDEspecialidad == id)?.Nombre)
                .Where(n => !string.IsNullOrEmpty(n))
            );

            string mensaje =
            $"ID: {odontologo.IDOdontologo}\n" +
            $"Nombre: {odontologo.Nombre}\n" +
            $"Cédula: {odontologo.Cedula}\n" +
            $"Fecha de Registro: {odontologo.FechaRegistro:dd/MM/yyyy}\n" +
            $"Teléfono: {odontologo.Telefono}\n" +
            $"Email: {odontologo.CorreoElectronico}\n" +
            $"Especialidades: {esp}\n" +
            $"Historial Laboral: {odontologo.HistorialLaboral}";


            MessageBox.Show(mensaje, "Detalle del Odontólogo");
        }


        private void dgvPacientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Ignora header

            Paciente paciente = pacientesBinding[e.RowIndex];

            string alergias = string.Join(", ", paciente.AlergiasDelPaciente
                .Select(ap => DatosGlobales.CatalogoAlergias
                    .FirstOrDefault(a => a.IDAlergia == Convert.ToInt32(ap.IDAlergia))?.Nombre)
                .Where(n => !string.IsNullOrEmpty(n))
            );

            string mensaje =
                $"ID: {paciente.IDPaciente}\n" +
                $"Nombre: {paciente.Nombre}\n" +
                $"Cédula: {paciente.Cedula}\n" +
                $"Fecha de Nac.: {paciente.FechaNacimiento:dd/MM/yyyy}\n" +
                $"Teléfono: {paciente.Telefono}\n" +
                $"Email: {paciente.CorreoElectronico}\n" +
                $"Alergias: {alergias}\n" +
                $"Historial Médico: {paciente.HistorialMedico}";

            MessageBox.Show(mensaje, "Detalle del Paciente");
        }

        private void CargarEspecialidades()
        {
            dgvEspecialidades.DataSource = new BindingList<Especialidad>(DatosGlobales.CatalogoEspecialidades);
        }

        private void CargarServiciosEnCheckedList()
        {
            clbServiciosDisponibles.Items.Clear();
            foreach (var serv in DatosGlobales.CatalogoServicios)
                clbServiciosDisponibles.Items.Add(serv, false);
        }
        private void dgvEspecialidades_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEspecialidades.CurrentRow == null) return;
            var esp = (Especialidad)dgvEspecialidades.CurrentRow.DataBoundItem;
            // Marcar relacionados:
            CargarServiciosEnCheckedList();
            var idServicios = DatosGlobales.ListaEspecialidadServicio
                .Where(r => r.IDEspecialidad == esp.IDEspecialidad)
                .Select(r => r.IDServicio).ToHashSet();
            for (int i = 0; i < clbServiciosDisponibles.Items.Count; i++)
            {
                var srv = clbServiciosDisponibles.Items[i] as Servicio;
                if (srv != null && idServicios.Contains(srv.IDServicio))
                    clbServiciosDisponibles.SetItemChecked(i, true);
            }
            txrAgBusElimEspecialidad.Text = esp.Nombre; // si tienes un textbox de nombre
        }

        private void txrAgBusElimEspecialidad_TextChanged(object sender, EventArgs e)
        {
            string criterio = txrAgBusElimEspecialidad.Text.Trim().ToLower();

            var filtradas = DatosGlobales.CatalogoEspecialidades
                .Where(esp => esp.Nombre.ToLower().Contains(criterio)
                           || esp.IDEspecialidad.ToString().Contains(criterio))
                .ToList();

            dgvEspecialidades.DataSource = new BindingList<Especialidad>(filtradas);
        }


        private void btnAgregarEspecialidad_Click(object sender, EventArgs e)
        {
            string nombreEsp = txrAgBusElimEspecialidad.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreEsp))
            {
                MessageBox.Show("El nombre de la especialidad es obligatorio.");
                return;
            }
            if (clbServiciosDisponibles.CheckedItems.Count == 0)
            {
                MessageBox.Show("Debes asociar al menos UN servicio a la especialidad.");
                return;
            }
            // Verifica duplicados:
            if (DatosGlobales.CatalogoEspecialidades.Any(x => x.Nombre.Equals(nombreEsp, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe una especialidad con ese nombre. Usa Editar si deseas modificarla.");
                return;
            }
            var nueva = new Especialidad
            {
                IDEspecialidad = DatosGlobales.CatalogoEspecialidades.Count == 0 ? 1 :
                                 DatosGlobales.CatalogoEspecialidades.Max(x => x.IDEspecialidad) + 1,
                Nombre = nombreEsp
            };
            DatosGlobales.CatalogoEspecialidades.Add(nueva);

            foreach (var serv in clbServiciosDisponibles.CheckedItems.Cast<Servicio>())
            {
                DatosGlobales.ListaEspecialidadServicio.Add(new EspecialidadServicio
                {
                    IDEspecialidad = nueva.IDEspecialidad,
                    IDServicio = serv.IDServicio
                });
            }
            CargarEspecialidades();
            CargarServiciosEnCheckedList();
            LimpiarEspecialidadUI();
        }

        private void clbServiciosDisponibles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnEditarEspecialidad_Click(object sender, EventArgs e)
        {
            if (dgvEspecialidades.CurrentRow == null) return;
            string nombreEsp = txrAgBusElimEspecialidad.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombreEsp))
            {
                MessageBox.Show("El nombre de la especialidad es obligatorio.");
                return;
            }
            if (clbServiciosDisponibles.CheckedItems.Count == 0)
            {
                MessageBox.Show("Debes asociar al menos UN servicio a la especialidad.");
                return;
            }
            var esp = (Especialidad)dgvEspecialidades.CurrentRow.DataBoundItem;
            // Verifica conflicto de duplicados (no te compares contigo mismo)
            if (DatosGlobales.CatalogoEspecialidades.Any(x => x.Nombre.Equals(nombreEsp, StringComparison.OrdinalIgnoreCase) && x.IDEspecialidad != esp.IDEspecialidad))
            {
                MessageBox.Show("Ya existe otra especialidad con ese nombre. Corrige el nombre o edita la otra.");
                return;
            }

            esp.Nombre = nombreEsp;
            DatosGlobales.ListaEspecialidadServicio.RemoveAll(r => r.IDEspecialidad == esp.IDEspecialidad);
            foreach (var serv in clbServiciosDisponibles.CheckedItems.Cast<Servicio>())
            {
                DatosGlobales.ListaEspecialidadServicio.Add(new EspecialidadServicio
                {
                    IDEspecialidad = esp.IDEspecialidad,
                    IDServicio = serv.IDServicio
                });
            }
            CargarEspecialidades();
            CargarServiciosEnCheckedList();
            LimpiarEspecialidadUI();
        }
        private void LimpiarEspecialidadUI()
        {
            txrAgBusElimEspecialidad.Clear();
            for (int i = 0; i < clbServiciosDisponibles.Items.Count; i++)
                clbServiciosDisponibles.SetItemChecked(i, false);
            dgvEspecialidades.ClearSelection();
        }


        private void btnEliminarEspecialidad_Click(object sender, EventArgs e)
        {
            if (dgvEspecialidades.CurrentRow == null) return;
            var esp = (Especialidad)dgvEspecialidades.CurrentRow.DataBoundItem;

            var confirm = MessageBox.Show(
                $"¿Seguro que deseas eliminar la especialidad '{esp.Nombre}'?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            // Elimina todas las relaciones con servicios
            DatosGlobales.ListaEspecialidadServicio.RemoveAll(r => r.IDEspecialidad == esp.IDEspecialidad);
            // Elimina la especialidad del catálogo
            DatosGlobales.CatalogoEspecialidades.Remove(esp);

            // Refresca el grid y los controles relacionados
            CargarEspecialidades();
            CargarServiciosEnCheckedList();

            // Limpia campos de la UI después de eliminar
            LimpiarEspecialidadUI(); // Este método limpia el textbox, los checks y la selección, como abajo
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void CargarServicios()
        {
            dgvServicios.AutoGenerateColumns = false;
            dgvServicios.Columns.Clear();

            // Columna ID
            dgvServicios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IDServicio",
                HeaderText = "ID",
                Name = "IDServicio"
            });

            // Columna Nombre
            dgvServicios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Nombre",
                HeaderText = "Servicio",
                Name = "NombreServicio"
            });

            dgvServicios.DataSource = new BindingList<Servicio>(DatosGlobales.CatalogoServicios);
        }


        private void tabOpciones_Enter(object sender, EventArgs e)
        {
            RefrescarOpciones();
        }

        private void CargarEspecialidadesEnCheckedList()
        {
            clbEspecialidadesRelacionadas.Items.Clear();
            foreach (var esp in DatosGlobales.CatalogoEspecialidades)
                clbEspecialidadesRelacionadas.Items.Add(esp, false);
        }

        private void dgvServicios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvServicios.CurrentRow == null) return;
            var srv = (Servicio)dgvServicios.CurrentRow.DataBoundItem;
            txrAgBusElimServicioS.Text = srv.Nombre;  // Solo nombre
            CargarEspecialidadesEnCheckedList(); // Limpia y carga todas desmarcadas

            var idEspecialidades = DatosGlobales.ListaEspecialidadServicio
                .Where(r => r.IDServicio == srv.IDServicio)
                .Select(r => r.IDEspecialidad).ToHashSet();

            for (int i = 0; i < clbEspecialidadesRelacionadas.Items.Count; i++)
            {
                var esp = clbEspecialidadesRelacionadas.Items[i] as Especialidad;
                if (esp != null && idEspecialidades.Contains(esp.IDEspecialidad))
                    clbEspecialidadesRelacionadas.SetItemChecked(i, true);
            }
        }


        private void btnAgregarServicioS_Click(object sender, EventArgs e)
        {
            string nombreServicio = txrAgBusElimServicioS.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombreServicio))
            {
                MessageBox.Show("El nombre del servicio es obligatorio.");
                return;
            }
            if (clbEspecialidadesRelacionadas.CheckedItems.Count == 0)
            {
                MessageBox.Show("Debes asociar al menos UNA especialidad al servicio.");
                return;
            }
            if (DatosGlobales.CatalogoServicios.Any(x => x.Nombre.Equals(nombreServicio, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe un servicio con ese nombre. Usa Editar para modificarlo.");
                return;
            }

            var nuevo = new Servicio
            {
                IDServicio = DatosGlobales.CatalogoServicios.Count == 0 ? 1 :
                             DatosGlobales.CatalogoServicios.Max(x => x.IDServicio) + 1,
                Nombre = nombreServicio
            };
            DatosGlobales.CatalogoServicios.Add(nuevo);

            foreach (var esp in clbEspecialidadesRelacionadas.CheckedItems.Cast<Especialidad>())
            {
                DatosGlobales.ListaEspecialidadServicio.Add(new EspecialidadServicio
                {
                    IDEspecialidad = esp.IDEspecialidad,
                    IDServicio = nuevo.IDServicio
                });
            }
            CargarServicios();
            CargarEspecialidadesEnCheckedList();
            LimpiarServicioUI();
        }

        private void btnEditarServicioS_Click(object sender, EventArgs e)
        {
            if (dgvServicios.CurrentRow == null) return;
            string nombreServicio = txrAgBusElimServicioS.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombreServicio))
            {
                MessageBox.Show("El nombre del servicio es obligatorio.");
                return;
            }
            if (clbEspecialidadesRelacionadas.CheckedItems.Count == 0)
            {
                MessageBox.Show("Debes asociar al menos UNA especialidad al servicio.");
                return;
            }
            var srv = (Servicio)dgvServicios.CurrentRow.DataBoundItem;
            // Previene duplicados (no te compares contigo mismo)
            if (DatosGlobales.CatalogoServicios.Any(x => x.Nombre.Equals(nombreServicio, StringComparison.OrdinalIgnoreCase) && x.IDServicio != srv.IDServicio))
            {
                MessageBox.Show("Ya existe otro servicio con ese nombre. Cambia el nombre o edita el servicio correspondiente.");
                return;
            }

            srv.Nombre = nombreServicio;
            DatosGlobales.ListaEspecialidadServicio.RemoveAll(r => r.IDServicio == srv.IDServicio);

            foreach (var esp in clbEspecialidadesRelacionadas.CheckedItems.Cast<Especialidad>())
            {
                DatosGlobales.ListaEspecialidadServicio.Add(new EspecialidadServicio
                {
                    IDEspecialidad = esp.IDEspecialidad,
                    IDServicio = srv.IDServicio
                });
            }
            CargarServicios();
            CargarEspecialidadesEnCheckedList();
            LimpiarServicioUI();
        }
        private void LimpiarServicioUI()
        {
            txrAgBusElimServicioS.Clear();
            for (int i = 0; i < clbEspecialidadesRelacionadas.Items.Count; i++)
                clbEspecialidadesRelacionadas.SetItemChecked(i, false);
            dgvServicios.ClearSelection();
        }


        private void btnEliminarServicioS_Click(object sender, EventArgs e)
        {
            if (dgvServicios.CurrentRow == null) return;
            var srv = (Servicio)dgvServicios.CurrentRow.DataBoundItem;
            var confirm = MessageBox.Show($"¿Seguro que desea eliminar el servicio '{srv.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;
            DatosGlobales.ListaEspecialidadServicio.RemoveAll(r => r.IDServicio == srv.IDServicio);
            DatosGlobales.CatalogoServicios.Remove(srv);
            CargarServicios();
            clbEspecialidadesRelacionadas.ClearSelected();
            txrAgBusElimServicioS.Clear();
        }

        private void clbEspecialidadesRelacionadas_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabOpciones_Click(object sender, EventArgs e)
        {

        }

        private void dgvEspecialidades_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txrAgBusElimServicioS_TextChanged(object sender, EventArgs e)
        {
            string criterio = txrAgBusElimServicioS.Text.Trim().ToLower();

            var filtradas = DatosGlobales.CatalogoServicios
                .Where(srv => srv.Nombre.ToLower().Contains(criterio)
                           || srv.IDServicio.ToString().Contains(criterio))
                .ToList();

            dgvServicios.DataSource = new BindingList<Servicio>(filtradas);
        }

        private void dgvServicios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEspecialidades_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEspecialidades.CurrentRow == null) return;
            var esp = (Especialidad)dgvEspecialidades.CurrentRow.DataBoundItem;
            txrAgBusElimEspecialidad.Text = esp.Nombre;

            // Marcar servicios compatibles
            var idServicios = DatosGlobales.ListaEspecialidadServicio
                .Where(r => r.IDEspecialidad == esp.IDEspecialidad)
                .Select(r => r.IDServicio).ToHashSet();
            for (int i = 0; i < clbServiciosDisponibles.Items.Count; i++)
            {
                var srv = clbServiciosDisponibles.Items[i] as Servicio;
                clbServiciosDisponibles.SetItemChecked(i, srv != null && idServicios.Contains(srv.IDServicio));
            }
        }

        private void dgvServicios_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvServicios.CurrentRow == null) return;
            var srv = (Servicio)dgvServicios.CurrentRow.DataBoundItem;
            txrAgBusElimServicioS.Text = srv.Nombre;

            var idEspecialidades = DatosGlobales.ListaEspecialidadServicio
                .Where(r => r.IDServicio == srv.IDServicio)
                .Select(r => r.IDEspecialidad).ToHashSet();

            for (int i = 0; i < clbEspecialidadesRelacionadas.Items.Count; i++)
            {
                var esp = clbEspecialidadesRelacionadas.Items[i] as Especialidad;
                clbEspecialidadesRelacionadas.SetItemChecked(i, esp != null && idEspecialidades.Contains(esp.IDEspecialidad));
            }
        }
        private void RefrescarOpciones()
        {
            // Refrescar grids
            CargarEspecialidades();
            CargarServicios();

            // Refrescar CheckedListBox base (sin checks)
            CargarServiciosEnCheckedList();       // Servicios para especialidades
            CargarEspecialidadesEnCheckedList();  // Especialidades para servicios

            // Limpiar UI de edición
            LimpiarEspecialidadUI();
            LimpiarServicioUI();
        }

        private void btnRefrescarOpciones_Click(object sender, EventArgs e)
        {
            RefrescarOpciones();
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
        }
        private void RefrescarCitas()
        {
            var visibles = DatosGlobales.ListaCitas
                .Where(c => c.IDEstado != 4) // no mostrar canceladas
                .ToList();

            dgvCitas.DataSource = new BindingList<Cita>(visibles);
        }


        private void ConfigurarDataGridViewCitas()
        {
            dgvCitas.AutoGenerateColumns = false;
            dgvCitas.Columns.Clear();

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IDCita",
                HeaderText = "ID Cita"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreOdontologo",
                HeaderText = "Odontólogo"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombrePaciente",
                HeaderText = "Paciente"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreServicio",
                HeaderText = "Servicio"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Fecha",
                HeaderText = "Fecha"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Hora",
                HeaderText = "Hora"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreEstado",
                HeaderText = "Estado"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Descripcion",
                HeaderText = "Descripción"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HoraFinalAproximada",
                HeaderText = "Hora Final (aprox.)"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FechaRegistro",
                HeaderText = "Fecha Registro"
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IDUsuario",
                HeaderText = "ID Usuario"
            });

            dgvCitas.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Reprogramar",
                HeaderText = "Reprogramar",
                Text = "Reprogramar",
                UseColumnTextForButtonValue = true
            });

            dgvCitas.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Cancelar",
                HeaderText = "Cancelar",
                Text = "Cancelar",
                UseColumnTextForButtonValue = true
            });


            dgvCitas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCitas.RowHeadersVisible = false;
            dgvCitas.CellContentClick += dgvCitas_CellContentClick;
        }

        private void ActualizarCitasProgramadas()
        {
            DateTime hoy = DateTime.Today;
            DateTime manana = hoy.AddDays(1);

            int citasHoy = DatosGlobales.ListaCitas
                .Count(c => c.Fecha.Date == hoy);

            int citasManiana = DatosGlobales.ListaCitas
                .Count(c => c.Fecha.Date == manana);

            string textoHoy = $"{citasHoy}";
            string textoManiana = $"{citasManiana} cita(s) para mañana";

            // Panel principal
            lblCitasHoy.Text = textoHoy;
            lblCitasManiana.Text = textoManiana;

            // Pestaña Pacientes
            lblCitasHoy_Pacientes.Text = textoHoy;
            lblCitasManiana_Pacientes.Text = textoManiana;

            // Pestaña Citas
            lblCitasHoyC.Text = textoHoy;
            lblCitasManianaC.Text = textoManiana;

            lblCitasHoy_Servicios.Text = textoHoy;
            lblCitasManiana_Servicios.Text = textoManiana;
        }

        private void btnNuevaCita_Click(object sender, EventArgs e)
        {
            using (var frm = new NuevaCitaForm())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    RefrescarCitas();
                    ActualizarCitasProgramadas();
                    ActualizarServiciosDelDia();
                }
            }
        }

        private void CancelarCita(Cita cita)
        {
            var resp = MessageBox.Show(
                "¿Seguro que deseas cancelar esta cita?",
                "Cancelar cita",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (resp != DialogResult.Yes) return;

            cita.IDEstado = 4;           // Cancelada
            cita.HoraFinal = DateTime.Now;

            RefrescarCitas();
            ActualizarCitasProgramadas();
            DatosGlobales.ListaCitas.Remove(cita);
            RefrescarCitas();
            ActualizarCitasProgramadas();
            ActualizarServiciosDelDia();
        }

        private void ReprogramarCita(Cita cita)
        {
            using (var frm = new NuevaCitaForm(cita))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    RefrescarCitas();
                    ActualizarCitasProgramadas();
                    ActualizarServiciosDelDia();
                }
            }
        }

        private void dgvCitas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvCitas.Columns[e.ColumnIndex].Name;
            Cita cita = dgvCitas.Rows[e.RowIndex].DataBoundItem as Cita;
            if (cita == null) return;

            if (colName == "Reprogramar")
            {
                ReprogramarCita(cita);
            }
            else if (colName == "Cancelar")
            {
                CancelarCita(cita);
            }
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalPacientesCitas_Click(object sender, EventArgs e)
        {

        }

        private void lblCambioSemanalCitas_Click(object sender, EventArgs e)
        {

        }

        private void dgvCitas_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0) return;

            var cita = dgvCitas.Rows[e.RowIndex].DataBoundItem as Cita;
            if (cita == null) return;

            string nombrePaciente = cita.NombrePaciente;
            string nombreOdontologo = cita.NombreOdontologo;
            string nombreServicio = cita.NombreServicio;
            string nombreEstado = cita.NombreEstado;

            // Hora (TimeSpan) → usar ToString(@"hh\:mm")
            string horaInicio = cita.Hora.ToString(@"hh\:mm");

            // Hora final aprox. según tu definición:
            // si HoraFinalAproximada es TimeSpan:
            string horaFinalAprox = cita.HoraFinalAproximada.ToString(@"hh\:mm");
            // si la tienes como DateTime, usa:
            // string horaFinalAprox = cita.HoraFinalAproximada.ToString("HH:mm");

            string mensaje =
                $"ID Cita: {cita.IDCita}\n" +
                $"Paciente: {nombrePaciente}\n" +
                $"Odontólogo: {nombreOdontologo}\n" +
                $"Servicio: {nombreServicio}\n" +
                $"Fecha: {cita.Fecha:dd/MM/yyyy}\n" +
                $"Hora: {horaInicio}\n" +
                $"Estado: {nombreEstado}\n" +
                $"Descripción: {cita.Descripcion}\n" +
                $"Fecha de registro: {cita.FechaRegistro:dd/MM/yyyy HH:mm}\n" +
                $"ID Usuario: {cita.IDUsuario}\n" +
                $"Hora final aprox.: {horaFinalAprox}";

            MessageBox.Show(mensaje, "Detalle de la Cita");
        }

        private void ConfigurarDataGridViewServicios()
        {
            dgvServicio.AutoGenerateColumns = false;
            dgvServicio.Columns.Clear();

            dgvServicio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IDCita",
                HeaderText = "ID Cita"
            });

            dgvServicio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreOdontologo",
                HeaderText = "Odontólogo"
            });

            dgvServicio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombrePaciente",
                HeaderText = "Paciente"
            });

            dgvServicio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreServicio",
                HeaderText = "Servicio"
            });

            dgvServicio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProcedimientoServicio",
                HeaderText = "Procedimiento"
            });

            dgvServicio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Fecha",
                HeaderText = "Fecha"
            });

            dgvServicio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Hora",
                HeaderText = "Hora inicio"
            });

            dgvServicio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreEstado",
                HeaderText = "Estado"
            });

            dgvServicio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HoraFinalAproximada",
                HeaderText = "Hora final"
            });

            dgvServicio.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvServicio.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvServicio.RowHeadersVisible = false;
        }
        private void ActualizarServiciosDelDia()
        {
            DateTime hoy = DateTime.Today;

            // Servicios del día = citas de hoy
            int serviciosHoy = DatosGlobales.ListaCitas
                .Count(c => c.Fecha.Date == hoy);

            // Servicios activos = por ejemplo estados Pendiente o En proceso
            int serviciosActivos = DatosGlobales.ListaCitas
                .Count(c => c.Fecha.Date == hoy &&
                            (c.IDEstado == 1 || c.IDEstado == 2)); // ajusta según tus estados

            string textoHoy = $"Servicios hoy: {serviciosHoy}";
            string textoActivos = $"Activos hoy: {serviciosActivos}";

            // Pestaña Pacientes
            lblServiciosHoy_Pacientes.Text = textoHoy;
            lblServiciosActivos_Pacientes.Text = textoActivos;

            // Pestaña Citas
            lblServiciosHoy_Citas.Text = textoHoy;
            lblServiciosActivos_Citas.Text = textoActivos;

            // Pestaña Servicios
            lblServiciosHoy_Servicios.Text = textoHoy;
            lblServiciosActivos_Servicios.Text = textoActivos;

            // Doctores
            lblServiciosHoy_Doctores.Text = textoHoy;
            lblServiciosActivos_Doctores.Text = textoActivos;

        }

        private void RefrescarServicios()
        {
            DateTime hoy = DateTime.Today;

            var listaHoy = DatosGlobales.ListaCitas
                .Where(c => c.Fecha.Date == hoy)
                .ToList();

            dgvServicio.DataSource = new BindingList<Cita>(listaHoy);
        }

        private void tabServicios_Enter(object sender, EventArgs e)
        {
            ConfigurarDataGridViewServicios();
            RefrescarServicios();
        }

        private void dgvServicio_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            // Cerrar solo este formulario
            this.Close();
        }

        private void btnCerrarTodo_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
