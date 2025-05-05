using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                    SELECT si.Название, si.Описание, sp.Цена, si.ТипУслугиId, si.ЛокацияId 
                    FROM ServiceItems si
                    LEFT JOIN ServicePrices sp ON si.Id = sp.УслугаId
                    WHERE si.Id = @id", conn);

                cmd.Parameters.AddWithValue("@id", serviceId);

                SqlDataReader reader = cmd.ExecuteReader();
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

            int typeId = (int)comboServiceType.SelectedValue;
            object locationId = comboLocation.SelectedValue ?? DBNull.Value;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    int itemId;

                    if (serviceId.HasValue)
                    {
                        SqlCommand updateCmd = new SqlCommand(@"
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
                        SqlCommand insertCmd = new SqlCommand(@"
                    INSERT INTO ServiceItems (Название, Описание, ТипУслугиId, ЛокацияId) 
                    OUTPUT INSERTED.Id
                    VALUES (@name, @desc, @typeId, @locId)", conn, transaction);
                        insertCmd.Parameters.AddWithValue("@name", name);
                        insertCmd.Parameters.AddWithValue("@desc", description);
                        insertCmd.Parameters.AddWithValue("@typeId", typeId);
                        insertCmd.Parameters.AddWithValue("@locId", locationId ?? DBNull.Value);
                        itemId = (int)insertCmd.ExecuteScalar();
                    }

                    SqlCommand priceCheck = new SqlCommand(@"
                SELECT COUNT(*) FROM ServicePrices WHERE УслугаId = @itemId", conn, transaction);
                    priceCheck.Parameters.AddWithValue("@itemId", itemId);
                    int count = (int)priceCheck.ExecuteScalar();

                    if (count > 0)
                    {
                        SqlCommand updatePrice = new SqlCommand(@"
                    UPDATE ServicePrices SET Цена = @price 
                    WHERE УслугаId = @itemId", conn, transaction);
                        updatePrice.Parameters.AddWithValue("@price", price);
                        updatePrice.Parameters.AddWithValue("@itemId", itemId);
                        updatePrice.ExecuteNonQuery();
                    }
                    else
                    {
                        SqlCommand insertPrice = new SqlCommand(@"
                    INSERT INTO ServicePrices (УслугаId, Цена) 
                    VALUES (@itemId, @price)", conn, transaction);
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
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, Название FROM ServiceTypes", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                comboServiceType.DataSource = dt;
                comboServiceType.DisplayMember = "Название";
                comboServiceType.ValueMember = "Id";
            }
        }

        private void LoadLocations()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, Название FROM ServiceLocations", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                comboLocation.DataSource = dt;
                comboLocation.DisplayMember = "Название";
                comboLocation.ValueMember = "Id";
            }
        }
    }
}
