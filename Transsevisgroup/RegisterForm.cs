using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Transsevisgroup
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            StylizeRegisterForm();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();

            if (login == "" || password == "" || fullName == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            string passwordHash = ComputeSha256Hash(password);

            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=HOME-PC\\SQLEXPRESS;Initial Catalog=TranssevisgroupDB;Integrated Security=True"))
                {
                    conn.Open();

                    // Проверка на уникальность логина
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Login = @login", conn);
                    checkCmd.Parameters.AddWithValue("@login", login);

                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.");
                        return;
                    }

                    // Добавление пользователя
                    SqlCommand insertCmd = new SqlCommand(
                        "INSERT INTO Users (Login, PasswordHash, FullName, Role) VALUES (@login, @pass, @name, 'user')", conn);

                    insertCmd.Parameters.AddWithValue("@login", login);
                    insertCmd.Parameters.AddWithValue("@pass", passwordHash);
                    insertCmd.Parameters.AddWithValue("@name", fullName);
                    insertCmd.ExecuteNonQuery();

                    MessageBox.Show("Регистрация прошла успешно!");
                    this.Close(); // Закрываем форму регистрации
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка регистрации: " + ex.Message);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private void StylizeRegisterForm()
        {
            this.Text = "Регистрация нового пользователя";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Метки
            lblLogin.Text = "Логин:";
            lblPassword.Text = "Пароль:";
            lblFullName.Text = "ФИО:";

            Font labelFont = new Font("Segoe UI", 10);
            lblLogin.Font = labelFont;
            lblPassword.Font = labelFont;
            lblFullName.Font = labelFont;

            // Текстовые поля
            Font textBoxFont = new Font("Segoe UI", 10);
            txtLogin.Font = textBoxFont;
            txtPassword.Font = textBoxFont;
            txtFullName.Font = textBoxFont;

            // Кнопка регистрации
            btnRegister.Text = "Зарегистрироваться";
            btnRegister.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRegister.BackColor = Color.FromArgb(0, 120, 215);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;

            // Кнопка назад
            btnBack.Text = "Назад";
            btnBack.Font = new Font("Segoe UI", 9);
            btnBack.BackColor = Color.LightGray;
            btnBack.FlatStyle = FlatStyle.Flat;
        }

    }
}
