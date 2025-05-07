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
    public partial class ServicesAdminForm : Form
    {
        public ServicesAdminForm()
        {
            InitializeComponent();
            StylizeServicesAdminForm();
        }

        private void ServicesAdminForm_Load(object sender, EventArgs e)
        {
            LoadServiceTypes();
            LoadLocations();
            comboServiceType.SelectedIndexChanged += comboServiceType_SelectedIndexChanged;
            comboLocation.SelectedIndexChanged += comboLocation_SelectedIndexChanged;
            RefreshServiceTable();
        }

        private void LoadServiceTypes()
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT Id, Название FROM ServiceTypes", conn);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                comboServiceType.DataSource = dt;
                comboServiceType.DisplayMember = "Название";
                comboServiceType.ValueMember = "Id";
            }
        }

        private void LoadLocations()
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Название", typeof(string));
                dt.Rows.Add(0, "Все локации");
                SQLiteCommand cmd = new SQLiteCommand("SELECT Id, Название FROM ServiceLocations", conn);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dt);
                comboLocation.DataSource = dt;
                comboLocation.DisplayMember = "Название";
                comboLocation.ValueMember = "Id";
            }
        }

        private void LoadServices(int typeId, object locationId)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT si.Id, si.Название, si.Описание, sp.Цена, st.Название AS Тип, sl.Название AS Локация
                    FROM ServiceItems si
                    LEFT JOIN ServicePrices sp ON si.Id = sp.УслугаId
                    LEFT JOIN ServiceTypes st ON si.ТипУслугиId = st.Id
                    LEFT JOIN ServiceLocations sl ON si.ЛокацияId = sl.Id
                    WHERE si.ТипУслугиId = @typeId
                      AND (@locId IS NULL OR si.ЛокацияId IS NULL OR si.ЛокацияId = @locId)";

                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@typeId", typeId);
                cmd.Parameters.AddWithValue("@locId", locationId ?? DBNull.Value);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridServices.DataSource = dt;
            }
        }

        private void RefreshServiceTable()
        {
            if (comboServiceType.SelectedValue == null || comboLocation.SelectedValue == null)
                return;

            var typeRow = comboServiceType.SelectedItem as DataRowView;
            var locRow = comboLocation.SelectedItem as DataRowView;

            if (typeRow == null || locRow == null)
                return;

            // Безопасное приведение через Convert
            int typeId = Convert.ToInt32(typeRow["Id"]);
            int locId = Convert.ToInt32(locRow["Id"]);

            object locationParam = (locId == 0) ? null : (object)locId;

            LoadServices(typeId, locationParam);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new ServiceEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshServiceTable();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridServices.CurrentRow == null) return;

            // Безопасное приведение
            int id = Convert.ToInt32(dataGridServices.CurrentRow.Cells["Id"].Value);

            var form = new ServiceEditForm(id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshServiceTable();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridServices.CurrentRow == null) return;

            int id = Convert.ToInt32(dataGridServices.CurrentRow.Cells["Id"].Value);
            var result = MessageBox.Show("Удалить эту услугу?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

            using (var conn = Database.GetConnection())
            {
                conn.Open();
                var cmd = new SQLiteCommand("DELETE FROM ServiceItems WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            RefreshServiceTable();
        }

        private void comboServiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshServiceTable();
        }

        private void comboLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshServiceTable();
        }

        private void StylizeServicesAdminForm()
        {
            this.Text = "Управление услугами";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1220, 680);

            lblTitle.Text = "Панель управления услугами";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(12, 10);

            lblCategory.Text = "Категория:";
            lblCategory.Font = new Font("Segoe UI", 10);
            lblCategory.Location = new Point(12, 50);

            comboServiceType.Font = new Font("Segoe UI", 10);
            comboServiceType.Size = new Size(420, 28);
            comboServiceType.Location = new Point(110, 47);

            label1.Text = "Локация:";
            label1.Font = new Font("Segoe UI", 10);
            label1.Location = new Point(12, 90);

            comboLocation.Font = new Font("Segoe UI", 10);
            comboLocation.Size = new Size(420, 28);
            comboLocation.Location = new Point(110, 87);

            dataGridServices.Size = new Size(1160, 400);
            dataGridServices.Location = new Point(12, 130);
            dataGridServices.Font = new Font("Segoe UI", 10);
            dataGridServices.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridServices.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Button[] buttons = { btnAdd, btnEdit, btnDelete };
            string[] btnText = { "Добавить", "Редактировать", "Удалить" };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Text = btnText[i];
                buttons[i].Font = new Font("Segoe UI", 10, FontStyle.Bold);
                buttons[i].Size = new Size(360, 50);
                buttons[i].BackColor = Color.FromArgb(0, 120, 215);
                buttons[i].ForeColor = Color.White;
                buttons[i].FlatStyle = FlatStyle.Flat;
                buttons[i].Location = new Point(20 + i * 400, 550);
            }
        }
    }
}
