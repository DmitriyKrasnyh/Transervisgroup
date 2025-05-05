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
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            InitializeAboutForm();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {

        }

        private void InitializeAboutForm()
        {
            this.Text = "О компании";
            this.Size = new Size(820, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label titleLabel = new Label
            {
                Text = "ООО «Транссервисгрупп»",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(titleLabel);

            PictureBox logo = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(700, 20),
                SizeMode = PictureBoxSizeMode.Zoom
                // logo.Image = Image.FromFile("logo.png"); // Добавь логотип при необходимости
            };
            this.Controls.Add(logo);

            // Общие сведения
            GroupBox gbMainInfo = new GroupBox
            {
                Text = "Общие сведения",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 110),
                Size = new Size(760, 130)
            };
            gbMainInfo.Controls.Add(CreateLabel("ИНН / КПП: 3306014359 / 330601001", 20));
            gbMainInfo.Controls.Add(CreateLabel("ОГРН: 1103326000315", 50));
            gbMainInfo.Controls.Add(CreateLabel("Дата регистрации: 29.03.2010", 80));
            gbMainInfo.Controls.Add(CreateLabel("Уставный капитал: 100 000 руб.", 110));
            this.Controls.Add(gbMainInfo);

            // Контакты
            GroupBox gbContact = new GroupBox
            {
                Text = "Контактная информация",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 250),
                Size = new Size(760, 120)
            };
            gbContact.Controls.Add(CreateLabel("Юр. адрес: 601784, Владимирская обл., г. Кольчугино, ул. Луговая, д.14б", 20));
            gbContact.Controls.Add(CreateLabel("Директор: Сороченков Евгений Александрович", 50));
            gbContact.Controls.Add(CreateLabel("Телефон: +7 (499) 490-77-43", 80));
            gbContact.Controls.Add(CreateLabel("Email: soro4enkovea@yandex.ru", 110));
            this.Controls.Add(gbContact);

            // Финансы
            GroupBox gbFinance = new GroupBox
            {
                Text = "Финансовые показатели (2024)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 380),
                Size = new Size(760, 90)
            };
            gbFinance.Controls.Add(CreateLabel("Выручка: 99 млн руб. (+18%)", 20));
            gbFinance.Controls.Add(CreateLabel("Прибыль: скрыта (+77%)", 50));
            gbFinance.Controls.Add(CreateLabel("Стоимость: скрыта (+24%)", 80));
            this.Controls.Add(gbFinance);

            // Арбитраж
            GroupBox gbCourt = new GroupBox
            {
                Text = "Арбитражные дела",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 480),
                Size = new Size(760, 90)
            };
            gbCourt.Controls.Add(CreateLabel("Всего дел: 6 на сумму 481 тыс. руб.", 20));
            gbCourt.Controls.Add(CreateLabel("Истец: 4 дела на 398 тыс. руб.", 50));
            gbCourt.Controls.Add(CreateLabel("Ответчик: 1 дело на 46 тыс. руб.", 80));
            this.Controls.Add(gbCourt);

            // Закупки
            GroupBox gbContracts = new GroupBox
            {
                Text = "Госзакупки",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 580),
                Size = new Size(760, 70)
            };
            gbContracts.Controls.Add(CreateLabel("84 контракта на сумму 38,98 млн руб.", 20));
            gbContracts.Controls.Add(CreateLabel("ТОП заказчик: ФКУ \"ЦХИСО УМВД\", 26,6 млн руб.", 50));
            this.Controls.Add(gbContracts);

            // Кнопка закрытия
            Button closeButton = new Button
            {
                Text = "Закрыть",
                Size = new Size(100, 30),
                Location = new Point(680, 630)
            };
            closeButton.Click += (sender, e) => this.Close();
            this.Controls.Add(closeButton);
        }

        private Label CreateLabel(string text, int top)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, top)
            };
        }
    }
}
