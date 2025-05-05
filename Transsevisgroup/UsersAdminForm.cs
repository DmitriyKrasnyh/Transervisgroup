using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Transsevisgroup
{
    public partial class UsersAdminForm : Form
    {
        private DataTable usersTable;
        public UsersAdminForm()
        {
            InitializeComponent();
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                foreach (DataGridViewRow row in dataGridUsers.Rows)
                {
                    if (row.IsNewRow) continue;

                    int userId = Convert.ToInt32(row.Cells["Id"].Value);
                    string role = row.Cells["RoleCombo"].Value?.ToString();

                    SqlCommand cmd = new SqlCommand("UPDATE Users SET Role = @role WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Изменения сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadUsers();
        }
        private void LoadUsers()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Id, FullName AS ФИО, Login AS Логин, Role AS Роль FROM Users", conn);
                usersTable = new DataTable();
                adapter.Fill(usersTable);
                dataGridUsers.DataSource = usersTable;

                // Настройка DataGridView
                if (!dataGridUsers.Columns.Contains("Роль")) return;

                DataGridViewComboBoxColumn roleColumn = new DataGridViewComboBoxColumn();
                roleColumn.DataPropertyName = "Роль";
                roleColumn.HeaderText = "Роль";
                roleColumn.Name = "RoleCombo";
                roleColumn.Items.AddRange("user", "admin");

                int roleIndex = dataGridUsers.Columns["Роль"].Index;
                dataGridUsers.Columns.RemoveAt(roleIndex);
                dataGridUsers.Columns.Insert(roleIndex, roleColumn);

                dataGridUsers.Columns["Id"].Visible = false;
                dataGridUsers.Columns["ФИО"].ReadOnly = true;
                dataGridUsers.Columns["Логин"].ReadOnly = true;
            }
        }

        private void UsersAdminForm_Load(object sender, EventArgs e)
        {
            lblTitle.Text = "Управление пользователями";
            LoadUsers();
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dataGridUsers.CurrentRow == null) return;

            int userId = Convert.ToInt32(dataGridUsers.CurrentRow.Cells["Id"].Value);
            string name = dataGridUsers.CurrentRow.Cells["ФИО"].Value.ToString();

            var confirm = MessageBox.Show($"Удалить пользователя «{name}»?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Пользователь удалён.", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadUsers();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
