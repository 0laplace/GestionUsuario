using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GestionUsuario
{
    public class FormRoles : Form
    {
        TextBox txtNombreRol;
        Button btnGuardarRol, btnEliminarRol;
        DataGridView dataGridViewRoles;
        int rolSeleccionadoId = -1;

        public FormRoles()
        {
            InicializarComponentes();
            CargarRoles();
        }

        private void InicializarComponentes()
        {
            this.Text = "Gestión de Roles";
            this.Size = new System.Drawing.Size(500, 400);

            Label lblRol = new Label { Text = "Nombre del Rol:", Top = 20, Left = 20, Width = 120 };
            txtNombreRol = new TextBox { Top = 20, Left = 150, Width = 200 };

            btnGuardarRol = new Button { Text = "Guardar Rol", Top = 60, Left = 150, Width = 100 };
            btnGuardarRol.Click += BtnGuardarRol_Click;

            btnEliminarRol = new Button { Text = "Eliminar Rol", Top = 60, Left = 260, Width = 100 };
            btnEliminarRol.Click += BtnEliminarRol_Click;

            dataGridViewRoles = new DataGridView
            {
                Top = 110,
                Left = 20,
                Width = 440,
                Height = 200,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dataGridViewRoles.CellClick += DataGridViewRoles_CellClick;

            this.Controls.AddRange(new Control[]
            {
                lblRol, txtNombreRol,
                btnGuardarRol, btnEliminarRol,
                dataGridViewRoles
            });
        }

        private void CargarRoles()
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Nombre FROM Roles", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridViewRoles.DataSource = dt;
            }
        }

        private void BtnGuardarRol_Click(object sender, EventArgs e)
        {
            string nombreRol = txtNombreRol.Text.Trim();

            if (string.IsNullOrEmpty(nombreRol))
            {
                MessageBox.Show("Ingresa un nombre de rol válido.");
                return;
            }

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd;

                if (rolSeleccionadoId == -1)
                {
                    cmd = new SqlCommand("INSERT INTO Roles (Nombre) VALUES (@Nombre)", con);
                }
                else
                {
                    cmd = new SqlCommand("UPDATE Roles SET Nombre = @Nombre WHERE Id = @Id", con);
                    cmd.Parameters.AddWithValue("@Id", rolSeleccionadoId);
                }

                cmd.Parameters.AddWithValue("@Nombre", nombreRol);
                cmd.ExecuteNonQuery();
                con.Close();
            }

            LimpiarCampos();
            CargarRoles();
        }

        private void BtnEliminarRol_Click(object sender, EventArgs e)
        {
            if (dataGridViewRoles.SelectedRows.Count == 0)
                return;

            int id = Convert.ToInt32(dataGridViewRoles.SelectedRows[0].Cells["Id"].Value);
            DialogResult result = MessageBox.Show("¿Deseas eliminar este rol?", "Confirmar", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Roles WHERE Id = @Id", con);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                CargarRoles();
                LimpiarCampos();
            }
        }

        private void DataGridViewRoles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridViewRoles.Rows[e.RowIndex].Selected = true;
                rolSeleccionadoId = Convert.ToInt32(dataGridViewRoles.SelectedRows[0].Cells["Id"].Value);
                txtNombreRol.Text = dataGridViewRoles.SelectedRows[0].Cells["Nombre"].Value.ToString();
            }
        }

        private void LimpiarCampos()
        {
            rolSeleccionadoId = -1;
            txtNombreRol.Text = "";
        }
    }
}
