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
    public partial class PriceViewerForm : Form
    {
        private string role;
        public PriceViewerForm(string userRole)
        {
            InitializeComponent();
            role = userRole;
        }

        private void PriceViewerForm_Load(object sender, EventArgs e)
        {
            // Заголовок формы
            lblTitle.Text = "Прайс-лист услуг";

            // Загрузка категорий из базы
            LoadServiceTypes();

            // Загрузка локаций
            LoadLocations();

            // Назначение событий
            comboCategory.SelectedIndexChanged += ComboCategory_SelectedIndexChanged;
            comboLocation.SelectedIndexChanged += comboLocation_SelectedIndexChanged;

            // Проверка роли
            if (role != "admin")
            {
                btnAddService.Visible = false;
                btnEditService.Visible = false;
                btnDeleteService.Visible = false;
            }

            // Установка начального выбора
            if (comboCategory.Items.Count > 0)
                comboCategory.SelectedIndex = 0;

            if (comboLocation.Items.Count > 0)
                comboLocation.SelectedIndex = 0;
        }

        private void ComboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshServiceTable();
        }
        private void LoadServices(int typeId, object locationId)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
            SELECT si.Id, si.Название, si.Описание, sp.Цена, sl.Название AS Локация
            FROM ServiceItems si
            LEFT JOIN ServicePrices sp ON si.Id = sp.УслугаId
            LEFT JOIN ServiceLocations sl ON si.ЛокацияId = sl.Id
            WHERE si.ТипУслугиId = @typeId
              AND (@locId IS NULL OR si.ЛокацияId IS NULL OR si.ЛокацияId = @locId)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@typeId", typeId);
                cmd.Parameters.AddWithValue("@locId", locationId ?? DBNull.Value);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridServices.DataSource = dt;
            }
        }

        private void LoadLocations()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Название", typeof(string));

                // Добавим вручную строку "Все локации"
                dt.Rows.Add(0, "Все локации");

                SqlCommand cmd = new SqlCommand("SELECT Id, Название FROM ServiceLocations", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                comboLocation.DataSource = dt;
                comboLocation.DisplayMember = "Название";
                comboLocation.ValueMember = "Id";
            }
        }

        private void btnAddService_Click(object sender, EventArgs e)
        {
            var addForm = new ServiceEditForm(null); // null = новый
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                int typeId = (int)comboCategory.SelectedValue;
                int locId = (int)comboLocation.SelectedValue;
                object locationId = (locId == 0) ? null : (object)locId;
                LoadServices(typeId, locationId);
            }
        }

        private void btnEditService_Click(object sender, EventArgs e)
        {
            if (dataGridServices.CurrentRow == null) return;
            int id = (int)dataGridServices.CurrentRow.Cells["Id"].Value;

            var editForm = new ServiceEditForm(id);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                int typeId = (int)comboCategory.SelectedValue;
                int locId = (int)comboLocation.SelectedValue;
                object locationId = (locId == 0) ? null : (object)locId;
                LoadServices(typeId, locationId);
            }
        }

        private void btnDeleteService_Click(object sender, EventArgs e)
        {
            if (dataGridServices.CurrentRow == null) return;

            int id = (int)dataGridServices.CurrentRow.Cells["Id"].Value;
            string name = dataGridServices.CurrentRow.Cells["Название"].Value.ToString();

            var result = MessageBox.Show($"Удалить услугу «{name}»?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM ServiceItems WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            int typeId = (int)comboCategory.SelectedValue;
            int locId = (int)comboLocation.SelectedValue;
            object locationId = (locId == 0) ? null : (object)locId;
            LoadServices(typeId, locationId);
        }

        private void comboCategory_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            RefreshServiceTable();
        }

        private void comboLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshServiceTable();
        }
        private void RefreshServiceTable()
        {
            if (comboCategory.SelectedValue == null || comboLocation.SelectedValue == null)
                return;

            var categoryRow = comboCategory.SelectedItem as DataRowView;
            var locationRow = comboLocation.SelectedItem as DataRowView;

            if (categoryRow == null || locationRow == null)
                return;

            int typeId = (int)categoryRow["Id"];
            int locId = (int)locationRow["Id"];

            object locationParam = (locId == 0) ? null : (object)locId;

            LoadServices(typeId, locationParam);
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

                comboCategory.DataSource = dt;
                comboCategory.DisplayMember = "Название";
                comboCategory.ValueMember = "Id";
            }
        }

    }

}
