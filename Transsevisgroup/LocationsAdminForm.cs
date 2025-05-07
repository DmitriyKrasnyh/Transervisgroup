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
    public partial class LocationsAdminForm : Form
    {
        private DataTable locationsTable;
        private SQLiteDataAdapter adapter; // Сделать полем
        private SQLiteConnection conn;
        public LocationsAdminForm()
        {
            InitializeComponent();
            StylizeForm();
        }

        private void LocationsAdminForm_Load(object sender, EventArgs e)
        {
            lblTitle.Text = "Управление локациями";
            lblHint.Text = "Вы можете редактировать ячейки, добавлять новые строки или удалять выбранные.";

            LoadLocations();
        }

        private void LoadLocations()
        {
            conn = Database.GetConnection();
            conn.Open();

            adapter = new SQLiteDataAdapter("SELECT Id, Название, Адрес FROM ServiceLocations", conn);
            SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);

            locationsTable = new DataTable();
            adapter.Fill(locationsTable);
            dataGridLocations.DataSource = locationsTable;

            dataGridLocations.RowValidated += (s, ev) =>
            {
                try
                {
                    adapter.Update(locationsTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                }
            };
        }


        private void btnReload_Click(object sender, EventArgs e)
        {
            conn?.Close();
            LoadLocations();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            conn?.Close();
            this.Close();
        }

        private void dataGridLocations_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Удалить выбранную локацию?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridLocations.CurrentRow != null)
            {
                var result = MessageBox.Show("Удалить выбранную локацию?", "Подтверждение", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    dataGridLocations.Rows.Remove(dataGridLocations.CurrentRow);
                    adapter.Update(locationsTable);
                }
            }
        }

        private void StylizeForm()
        {
            this.Text = "Управление локациями";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            lblTitle.Text = "Управление локациями";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 30, 30);

            lblHint.Text = "Вы можете редактировать ячейки, добавлять строки и удалять локации.";
            lblHint.Font = new Font("Segoe UI", 10);
            lblHint.ForeColor = Color.Gray;

            btnReload.Text = "Обновить";
            btnBack.Text = "Назад";
            btnDelete.Text = "Удалить";

            foreach (var btn in new[] { btnReload, btnBack, btnDelete })
            {
                btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btn.BackColor = Color.FromArgb(0, 120, 215);
                btn.ForeColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
            }

            dataGridLocations.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridLocations.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridLocations.EnableHeadersVisualStyles = false;
            dataGridLocations.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dataGridLocations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

    }
}
