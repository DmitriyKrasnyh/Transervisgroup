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
    public partial class AboutUs : Form
    {
        public AboutUs()
        {
            InitializeComponent();
            InitializeContactsForm();
        }

        private void AboutUs_Load(object sender, EventArgs e)
        {

        }
        private void InitializeContactsForm()
        {
            this.Text = "Контакты";
            this.Size = new Size(500, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label titleLabel = new Label
            {
                Text = "Связь с нами",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(titleLabel);

            Label contactLabel = new Label
            {
                Text = "Вы можете связаться с нами по следующим каналам:",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(20, 60)
            };
            this.Controls.Add(contactLabel);

            Label phoneLabel = new Label
            {
                Text = "Телефон: +7 (499) 490-77-43",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(20, 90)
            };
            this.Controls.Add(phoneLabel);

            Label emailLabel = new Label
            {
                Text = "Email: soro4enkovea@yandex.ru",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(20, 120)
            };
            this.Controls.Add(emailLabel);

            LinkLabel vkLink = new LinkLabel
            {
                Text = "Мы ВКонтакте: https://vk.com/tsg_service",
                Font = new Font("Segoe UI", 10, FontStyle.Underline),
                AutoSize = true,
                Location = new Point(20, 150),
                LinkColor = Color.Blue
            };
            vkLink.Click += (sender, e) =>
            {
                System.Diagnostics.Process.Start("https://vk.com/tsg_service");
            };
            this.Controls.Add(vkLink);

            Button closeButton = new Button
            {
                Text = "Закрыть",
                Size = new Size(100, 30),
                Location = new Point(360, 210)
            };
            closeButton.Click += (sender, e) => this.Close();
            this.Controls.Add(closeButton);
        }
    }
}
