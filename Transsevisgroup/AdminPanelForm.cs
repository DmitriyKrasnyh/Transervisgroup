using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Transsevisgroup
{
    public partial class AdminPanelForm : Form
    {
        public AdminPanelForm()
        {
            InitializeComponent();
            StylizeAdminPanel();
        }

        private void btnServices_Click(object sender, EventArgs e)
        {
            ServicesAdminForm servicesForm = new ServicesAdminForm();
            servicesForm.ShowDialog();
        }

        private void AdminPanelForm_Load(object sender, EventArgs e)
        {
            lblTitle.Text = "Админ-панель";
        }

        private void btnLocations_Click(object sender, EventArgs e)
        {
            LocationsAdminForm locationForm = new LocationsAdminForm();
            locationForm.ShowDialog();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            UsersAdminForm userForm = new UsersAdminForm();
            userForm.ShowDialog();
        }

        private void btnBookings_Click(object sender, EventArgs e)
        {
            BookingsAdminForm bookingForm = new BookingsAdminForm();
            bookingForm.ShowDialog();
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StylizeAdminPanel()
        {
            this.Text = "Панель администратора";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            lblTitle.Text = "Добро пожаловать в панель администратора";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 30, 30);

            // Общий стиль кнопок
            Button[] buttons = { btnServices, btnLocations, btnUsers, btnBookings, btnStatistics, btnBack };
            string[] buttonNames = {
        "Управление услугами",
        "Управление локациями",
        "Пользователи",
        "Записи (брони)",
        "Статистика",
        "Назад"
    };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Text = buttonNames[i];
                buttons[i].BackColor = Color.FromArgb(0, 120, 215);
                buttons[i].ForeColor = Color.White;
                buttons[i].FlatStyle = FlatStyle.Flat;
                buttons[i].Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }
        }

    }
}
