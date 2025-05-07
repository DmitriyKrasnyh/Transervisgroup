using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Transsevisgroup
{
    public partial class ServiceBookingForm : Form
    {
        private int userId;
        private string fullName;
        private DateTime? selectedTimeSlot = null;

        public ServiceBookingForm(int userId, string fullName)
        {
            InitializeComponent();
            this.userId = userId;
            this.fullName = fullName;
            StylizeForm();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (comboServices.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите услугу.");
                return;
            }

            if (selectedTimeSlot == null)
            {
                MessageBox.Show("Пожалуйста, выберите свободное время из доступных слотов.", "Время не выбрано", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Bookings
            ([ПользовательId], [УслугаId], [Телефон], [Почта], [Время], [Комментарий], [ТипКлиентаId])
            VALUES (@UserId, @ServiceId, @Phone, @Email, @DateTime, @Comment, @ClientType)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ServiceId", comboServices.SelectedValue);
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@DateTime", selectedTimeSlot.Value);
                    cmd.Parameters.AddWithValue("@Comment", txtComment.Text.Trim());
                    int clientTypeId = GetClientTypeId();
                    cmd.Parameters.AddWithValue("@ClientType", clientTypeId);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    

        private void ServiceBookingForm_Load(object sender, EventArgs e)
        {
            lblTitle.Text = "Запись на услугу";
            lblFullName.Text = $"ФИО: {fullName}";

            LoadServices();

            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = "dd.MM.yyyy HH:mm";
            datePicker.MinDate = DateTime.Now;
            datePicker.Value = DateTime.Now.AddHours(1);
        }

        private void LoadServices()
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT 
                si.Id, 
                si.Название || ' (' || IFNULL(sp.Цена, 'без цены') || '₽)' AS DisplayName
            FROM ServiceItems si
            LEFT JOIN ServicePrices sp ON si.Id = sp.УслугаId";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                comboServices.DataSource = dt;
                comboServices.DisplayMember = "DisplayName";
                comboServices.ValueMember = "Id";
            }
        }




        private bool IsTimeSlotTaken(DateTime dateTime)
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM Bookings WHERE [Время] = @datetime", conn);
                cmd.Parameters.AddWithValue("@datetime", dateTime);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private bool IsTimeAvailable(DateTime selectedDateTime)
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Bookings WHERE [Время] = @DateTime";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DateTime", selectedDateTime);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count == 0;
                }
            }
        }

        private int GetClientTypeId()
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();

                // Проверяем количество предыдущих записей
                SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM Bookings WHERE ПользовательId = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                // 1 — Обычный, 2 — Постоянный (если был хотя бы 1 визит)
                return (count > 0) ? 2 : 1;
            }
        }

        private void LoadAvailableTimeSlots()
        {
            flowTimeSlots.Controls.Clear();

            DateTime date = datePicker.Value.Date;
            DateTime startTime = date.AddHours(9);
            DateTime endTime = date.AddHours(18);

            for (DateTime time = startTime; time < endTime; time = time.AddMinutes(30))
            {
                if (IsTimeAvailable(time))
                {
                    DateTime slotTime = time; // 🟢 фикс: новая переменная для захвата

                    Button btn = new Button();
                    btn.Text = slotTime.ToString("HH:mm");
                    btn.Width = 60;
                    btn.Height = 30;
                    btn.Click += (s, e) =>
                    {
                        selectedTimeSlot = slotTime;
                        lblSelectedTime.Text = $"Вы выбрали: {slotTime:dd.MM.yyyy HH:mm}";
                    };
                    flowTimeSlots.Controls.Add(btn);
                }
            }

            if (flowTimeSlots.Controls.Count == 0)
            {
                Label noSlots = new Label();
                noSlots.Text = "Нет свободных слотов";
                noSlots.ForeColor = Color.Gray;
                flowTimeSlots.Controls.Add(noSlots);
            }
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            selectedTimeSlot = null;
            lblSelectedTime.Text = "";
            LoadAvailableTimeSlots();
        }

        private void StylizeForm()
        {
            this.Text = "Запись на услугу";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            Font labelFont = new Font("Segoe UI", 10);
            Font labelBold = new Font("Segoe UI", 12, FontStyle.Bold);
            Font inputFont = new Font("Segoe UI", 10);

            lblTitle.Font = labelBold;
            lblFullName.Font = labelFont;
            lblService.Font = labelFont;
            lblPhone.Font = labelFont;
            lblEmail.Font = labelFont;
            
            lblComment.Font = labelFont;
            lblDate.Font = labelFont;
            lblTime.Font = labelFont;
            lblSelectedTime.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblSelectedTime.ForeColor = Color.DarkGreen;
            lblService.Text = "Услуга:";
            lblPhone.Text = "Телефон:";
            lblEmail.Text = "Электронная почта:";
            lblDate.Text = "Дата:";
            lblTime.Text = "Выберите время:";
            lblComment.Text = "Комментарий:";
            lblSelectedTime.Text = ""; // будет отображать выбранный слот


            foreach (Control control in new Control[] { txtPhone, txtEmail, txtComment, comboServices, datePicker })
            {
                control.Font = inputFont;
                control.BackColor = Color.WhiteSmoke;
            }

            txtComment.BorderStyle = BorderStyle.FixedSingle;

            btnSubmit.Text = "Записаться";
            btnSubmit.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSubmit.BackColor = Color.FromArgb(0, 120, 215);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.FlatStyle = FlatStyle.Flat;

            flowTimeSlots.BackColor = Color.WhiteSmoke;
            flowTimeSlots.BorderStyle = BorderStyle.FixedSingle;
        }

    }
}
