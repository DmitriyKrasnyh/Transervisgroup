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
    public partial class ServiceEditForm : Form
    {
        private int? serviceId; // null = добавление, иначе — редактирование
        public ServiceEditForm(int? id = null)
        {
            InitializeComponent();
            StylizeServiceEditForm();
            serviceId = id;
        }

        private void ServiceEditForm_Load(object sender, EventArgs e)
        {
            if (serviceId.HasValue)
            {
                this.Text = "Редактирование услуги";
                LoadServiceData();
            }
            else
            {
                this.Text = "Добавление новой услуги";
            }

            LoadServiceTypes();
            LoadLocations();
        }

        private void LoadServiceData()
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(@"
            SELECT si.Название, si.Описание, sp.Цена, si.ТипУслугиId, si.ЛокацияId 
            FROM ServiceItems si
            LEFT JOIN ServicePrices sp ON si.Id = sp.УслугаId
            WHERE si.Id = @id", conn);

                cmd.Parameters.AddWithValue("@id", serviceId);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtName.Text = reader["Название"].ToString();
                        txtDescription.Text = reader["Описание"].ToString();
                        numPrice.Value = reader["Цена"] != DBNull.Value ? Convert.ToDecimal(reader["Цена"]) : 0;

                        if (reader["ТипУслугиId"] != DBNull.Value)
                            comboServiceType.SelectedValue = Convert.ToInt32(reader["ТипУслугиId"]);

                        if (reader["ЛокацияId"] != DBNull.Value)
                            comboLocation.SelectedValue = Convert.ToInt32(reader["ЛокацияId"]);
                    }
                }
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();
            decimal price = numPrice.Value;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название услуги.");
                return;
            }

            int typeId = Convert.ToInt32(comboServiceType.SelectedValue);
            object locationId = comboLocation.SelectedValue ?? DBNull.Value;

            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                SQLiteTransaction transaction = conn.BeginTransaction();

                try
                {
                    int itemId;

                    if (serviceId.HasValue)
                    {
                        SQLiteCommand updateCmd = new SQLiteCommand(@"
                    UPDATE ServiceItems 
                    SET Название = @name, Описание = @desc, ТипУслугиId = @typeId, ЛокацияId = @locId
                    WHERE Id = @id", conn, transaction);
                        updateCmd.Parameters.AddWithValue("@name", name);
                        updateCmd.Parameters.AddWithValue("@desc", description);
                        updateCmd.Parameters.AddWithValue("@typeId", typeId);
                        updateCmd.Parameters.AddWithValue("@locId", locationId ?? DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@id", serviceId.Value);
                        updateCmd.ExecuteNonQuery();

                        itemId = serviceId.Value;
                    }
                    else
                    {
                        SQLiteCommand insertCmd = new SQLiteCommand(@"
                    INSERT INTO ServiceItems (Название, Описание, ТипУслугиId, ЛокацияId) 
                    VALUES (@name, @desc, @typeId, @locId); SELECT last_insert_rowid();", conn, transaction);
                        insertCmd.Parameters.AddWithValue("@name", name);
                        insertCmd.Parameters.AddWithValue("@desc", description);
                        insertCmd.Parameters.AddWithValue("@typeId", typeId);
                        insertCmd.Parameters.AddWithValue("@locId", locationId ?? DBNull.Value);

                        itemId = Convert.ToInt32(insertCmd.ExecuteScalar());
                    }

                    SQLiteCommand priceCheck = new SQLiteCommand("SELECT COUNT(*) FROM ServicePrices WHERE УслугаId = @itemId", conn, transaction);
                    priceCheck.Parameters.AddWithValue("@itemId", itemId);
                    int count = Convert.ToInt32(priceCheck.ExecuteScalar());

                    if (count > 0)
                    {
                        SQLiteCommand updatePrice = new SQLiteCommand("UPDATE ServicePrices SET Цена = @price WHERE УслугаId = @itemId", conn, transaction);
                        updatePrice.Parameters.AddWithValue("@price", price);
                        updatePrice.Parameters.AddWithValue("@itemId", itemId);
                        updatePrice.ExecuteNonQuery();
                    }
                    else
                    {
                        SQLiteCommand insertPrice = new SQLiteCommand("INSERT INTO ServicePrices (УслугаId, Цена) VALUES (@itemId, @price)", conn, transaction);
                        insertPrice.Parameters.AddWithValue("@itemId", itemId);
                        insertPrice.Parameters.AddWithValue("@price", price);
                        insertPrice.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Услуга успешно сохранена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadServiceTypes()
        {
            using (SQLiteConnection conn = Database.GetConnection())
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
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT Id, Название FROM ServiceLocations", conn);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                comboLocation.DataSource = dt;
                comboLocation.DisplayMember = "Название";
                comboLocation.ValueMember = "Id";
            }
        }

        private void StylizeServiceEditForm()
        {
            this.Text = serviceId.HasValue ? "Редактирование услуги" : "Добавление новой услуги";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(550, 360);

            lblTitle.Text = this.Text;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 10);

            Label[] labels = { label1, lblName, lblDescription, lblPrice };
            string[] texts = { "Локация:", "Название:", "Описание:", "Цена, ₽:" };

            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Text = texts[i];
                labels[i].Font = new Font("Segoe UI", 10);
            }

            // Комбобоксы
            comboServiceType.Font = new Font("Segoe UI", 10);
            comboServiceType.Size = new Size(400, 28);
            comboServiceType.Location = new Point(120, 40);

            comboLocation.Font = new Font("Segoe UI", 10);
            comboLocation.Size = new Size(400, 28);
            comboLocation.Location = new Point(120, 75);

            // Текстовые поля
            txtName.Font = new Font("Segoe UI", 10);
            txtName.Size = new Size(400, 26);
            txtName.Location = new Point(120, 110);

            txtDescription.Font = new Font("Segoe UI", 10);
            txtDescription.Size = new Size(400, 80);
            txtDescription.Location = new Point(120, 145);

            numPrice.Font = new Font("Segoe UI", 10);
            numPrice.Size = new Size(400, 26);
            numPrice.Location = new Point(120, 235);
            numPrice.DecimalPlaces = 2;
            numPrice.Maximum = 1000000;

            // Кнопки
            Button[] buttons = { btnSave, btnCancel };
            string[] btnText = { "Сохранить", "Отмена" };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Text = btnText[i];
                buttons[i].Font = new Font("Segoe UI", 10, FontStyle.Bold);
                buttons[i].Size = new Size(180, 40);
                buttons[i].BackColor = Color.FromArgb(0, 120, 215);
                buttons[i].ForeColor = Color.White;
                buttons[i].FlatStyle = FlatStyle.Flat;
                buttons[i].Location = new Point(120 + i * 220, 280);
            }
        }
    }
}
