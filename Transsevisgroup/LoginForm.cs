using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Transsevisgroup
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            StylizeForm();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            string passwordHash = ComputeSha256Hash(password);

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                   
                    // ⬇️ Основная логика входа
                    using (var cmd = new SQLiteCommand("SELECT Role, FullName FROM Users WHERE Login=@login AND PasswordHash=@pass", conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@pass", passwordHash);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string role = reader["Role"].ToString();
                                string fullName = reader["FullName"].ToString();

                                MessageBox.Show($"Добро пожаловать, {fullName} ({role})!");

                                MainForm main = new MainForm(role, fullName);
                                main.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm regForm = new RegisterForm();
            regForm.ShowDialog();
        }

        private void StylizeForm()
        {
            this.Text = "Вход в систему";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Лейблы
            lblLogin.Text = "Логин:";
            lblLogin.Font = new Font("Segoe UI", 11);
            lblLogin.Location = new Point(40, 50);

            lblPassword.Text = "Пароль:";
            lblPassword.Font = new Font("Segoe UI", 11);
            lblPassword.Location = new Point(40, 95);

            // Текстбоксы
            txtLogin.Font = new Font("Segoe UI", 11);
            txtLogin.Size = new Size(300, 30);
            txtLogin.Location = new Point(120, 47);

            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.Size = new Size(300, 30);
            txtPassword.Location = new Point(120, 92);
            txtPassword.PasswordChar = '*';

            // Кнопка входа
            btnLogin.Text = "Войти";
            btnLogin.Font = new Font("Segoe UI", 11);
            btnLogin.Size = new Size(380, 40);
            btnLogin.Location = new Point(40, 145);
            btnLogin.BackColor = Color.FromArgb(0, 120, 215);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;

            // Кнопка регистрации
            btnRegister.Text = "Регистрация";
            btnRegister.Font = new Font("Segoe UI", 10);
            btnRegister.Size = new Size(380, 35);
            btnRegister.Location = new Point(40, 195);
            btnRegister.BackColor = Color.LightGray;
            btnRegister.FlatStyle = FlatStyle.Flat;

            // Кнопка выхода
            btnExit.Text = "Выход";
            btnExit.Font = new Font("Segoe UI", 9);
            btnExit.Size = new Size(60, 30);
            btnExit.Location = new Point(this.ClientSize.Width - 70, 10);
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }


        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtLogin_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblPassword_Click(object sender, EventArgs e)
        {

        }

        private void lblLogin_Click(object sender, EventArgs e)
        {

        }
    }
}
