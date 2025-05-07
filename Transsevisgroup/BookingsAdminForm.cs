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
    public partial class BookingsAdminForm : Form
    {
        public BookingsAdminForm()
        {
            InitializeComponent();
        }

        private void BookingsAdminForm_Load(object sender, EventArgs e)
        {
            lblTitle.Text = "Записи клиентов";

            StylizeForm();

            LoadServices();
            LoadUsers();

            dateFilter.ValueChanged += FilterChanged;
            comboServiceFilter.SelectedIndexChanged += FilterChanged;
            comboUserFilter.SelectedIndexChanged += FilterChanged;

            LoadBookings();
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            if (comboServiceFilter.Items.Count > 0)
                comboServiceFilter.SelectedIndex = 0;

            if (comboUserFilter.Items.Count > 0)
                comboUserFilter.SelectedIndex = 0;

            dateFilter.Checked = false;
            LoadBookings();
        }

        private void LoadServices()
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();

                DataTable dt = new DataTable();
                SQLiteCommand cmd = new SQLiteCommand("SELECT Id, Название FROM ServiceItems", conn);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dt);

                // Добавим "Все услуги"
                DataRow allRow = dt.NewRow();
                allRow["Id"] = 0;
                allRow["Название"] = "Все услуги";
                dt.Rows.InsertAt(allRow, 0);

                comboServiceFilter.DataSource = dt;
                comboServiceFilter.DisplayMember = "Название";
                comboServiceFilter.ValueMember = "Id";
            }
        }

        private void LoadUsers()
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();

                DataTable dt = new DataTable();
                SQLiteCommand cmd = new SQLiteCommand("SELECT Id, FullName FROM Users", conn);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dt);

                // Добавим "Все пользователи"
                DataRow allRow = dt.NewRow();
                allRow["Id"] = 0;
                allRow["FullName"] = "Все пользователи";
                dt.Rows.InsertAt(allRow, 0);

                comboUserFilter.DataSource = dt;
                comboUserFilter.DisplayMember = "FullName";
                comboUserFilter.ValueMember = "Id";
            }
        }

        private void LoadBookings()
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
                    SELECT b.Id, u.FullName AS Пользователь, s.Название AS Услуга, 
                           b.Телефон, b.Почта, b.Время, b.Комментарий
                    FROM Bookings b
                    LEFT JOIN Users u ON b.ПользовательId = u.Id
                    LEFT JOIN ServiceItems s ON b.УслугаId = s.Id
                    WHERE (@serviceId = 0 OR b.УслугаId = @serviceId)
                      AND (@userId = 0 OR b.ПользовательId = @userId)
                      AND (@date IS NULL OR DATE(b.Время) = DATE(@date))";

                SQLiteCommand cmd = new SQLiteCommand(query, conn);

                int serviceId = Convert.ToInt32(comboServiceFilter.SelectedValue ?? 0);
                int userId = Convert.ToInt32(comboUserFilter.SelectedValue ?? 0);
                object date = dateFilter.Checked ? (object)dateFilter.Value.Date : DBNull.Value;

                cmd.Parameters.AddWithValue("@serviceId", serviceId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@date", date);

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridBookings.DataSource = dt;
            }
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            LoadBookings();
        }

        private void StylizeForm()
        {
            this.Text = "Записи клиентов";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            lblTitle.Text = "Просмотр записей клиентов";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 30, 30);

            label1.Text = "Дата:";
            label2.Text = "Услуга:";
            label3.Text = "Пользователь:";

            label1.Font = label2.Font = label3.Font = new Font("Segoe UI", 10);
            dateFilter.Font = new Font("Segoe UI", 10);
            comboServiceFilter.Font = comboUserFilter.Font = new Font("Segoe UI", 10);

            btnClearFilters.Text = "Сбросить фильтры";
            btnClearFilters.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClearFilters.BackColor = Color.FromArgb(0, 120, 215);
            btnClearFilters.ForeColor = Color.White;
            btnClearFilters.FlatStyle = FlatStyle.Flat;

            dataGridBookings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridBookings.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridBookings.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridBookings.EnableHeadersVisualStyles = false;
            dataGridBookings.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
        }

    }
}
