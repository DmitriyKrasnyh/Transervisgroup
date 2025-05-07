using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
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
            StylizeUsersAdminForm();
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                foreach (DataGridViewRow row in dataGridUsers.Rows)
                {
                    if (row.IsNewRow) continue;

                    int userId = Convert.ToInt32(row.Cells["Id"].Value);
                    string role = row.Cells["RoleCombo"].Value?.ToString();

                    SQLiteCommand cmd = new SQLiteCommand("UPDATE Users SET Role = @role WHERE Id = @id", conn);
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
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT Id, FullName AS ФИО, Login AS Логин, Role AS Роль FROM Users", conn);
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

            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Users WHERE Id = @id", conn);
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

        private void StylizeUsersAdminForm()
        {
            this.Text = "Управление пользователями";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1220, 620);

            lblTitle.Text = "Пользователи системы";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(12, 10);

            lblStatus.Text = "Вы можете редактировать роли пользователей и удалять их.";
            lblStatus.Font = new Font("Segoe UI", 10);
            lblStatus.Location = new Point(12, 50);

            dataGridUsers.Size = new Size(1160, 380);
            dataGridUsers.Location = new Point(12, 90);
            dataGridUsers.Font = new Font("Segoe UI", 10);
            dataGridUsers.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Button[] buttons = { btnSaveChanges, btnDeleteUser, btnBack };
            string[] btnText = { "Сохранить", "Удалить", "Назад" };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Text = btnText[i];
                buttons[i].Font = new Font("Segoe UI", 10, FontStyle.Bold);
                buttons[i].Size = new Size(180, 40);
                buttons[i].BackColor = Color.FromArgb(0, 120, 215);
                buttons[i].ForeColor = Color.White;
                buttons[i].FlatStyle = FlatStyle.Flat;
                buttons[i].Location = new Point(12 + i * 200, 500);
            }
        }
    }
}
