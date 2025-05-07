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
    public partial class MainForm : Form
    {
        private string role;
        private string fullName;
       
        public MainForm(string role, string fullName)
        {
            InitializeComponent();
            this.role = role;
            this.fullName = fullName;
        }

        private void btnDocuments_Click(object sender, EventArgs e)
        {
            PriceViewerForm priceViewer = new PriceViewerForm(role);
            priceViewer.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Добро пожаловать, {fullName}!";
            lblRole.Text = $"Роль: {role}";

            if (role == "admin")
                btnAdmin.Visible = true;
            else
                btnAdmin.Visible = false;

            StylizeMainForm();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutUs aboutUsForm = new AboutUs();
            aboutUsForm.ShowDialog();
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            AdminPanelForm adminForm = new AdminPanelForm();
            adminForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Возврат к форме логина
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void btnBookService_Click(object sender, EventArgs e)
        {
            // Открытие формы записи на услугу
            ServiceBookingForm bookingForm = new ServiceBookingForm(GetUserId(), fullName);
            bookingForm.ShowDialog();
        }

        private int GetUserId()
        {
            int id = -1;
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT Id FROM Users WHERE FullName = @name", conn);
                cmd.Parameters.AddWithValue("@name", fullName);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    id = Convert.ToInt32(result);
                }
            }
            return id;
        }

        private void StylizeMainForm()
        {
            this.Text = "Панель управления";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Заголовки
            lblWelcome.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblWelcome.Location = new Point(20, 15);

            lblRole.Font = new Font("Segoe UI", 10);
            lblRole.Location = new Point(20, 45);

            // Кнопки верхнего блока
            btnLogout.Text = "Выход";
            btnLogout.Font = new Font("Segoe UI", 10);
            btnLogout.Size = new Size(100, 35);
            btnLogout.BackColor = Color.LightGray;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Основные кнопки
            Button[] mainButtons = { btnDocuments, btnBookService, btnAbout };

            string[] btnNames = { "Документы", "Запись на услугу", "О компании" };

            for (int i = 0; i < mainButtons.Length; i++)
            {
                mainButtons[i].Text = btnNames[i];
                mainButtons[i].Font = new Font("Segoe UI", 12, FontStyle.Bold);
                mainButtons[i].BackColor = Color.FromArgb(0, 120, 215);
                mainButtons[i].ForeColor = Color.White;
                mainButtons[i].FlatStyle = FlatStyle.Flat;
                mainButtons[i].Size = new Size(300, 130);
                mainButtons[i].Location = new Point(20 + i * 320, 90);
            }

            // Админ-панель
            btnAdmin.Text = "Администрирование";
            btnAdmin.Font = new Font("Segoe UI", 11);
            btnAdmin.BackColor = Color.DimGray;
            btnAdmin.ForeColor = Color.White;
            btnAdmin.FlatStyle = FlatStyle.Flat;
            btnAdmin.Size = new Size(940, 50);
            btnAdmin.Location = new Point(30, 240);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
