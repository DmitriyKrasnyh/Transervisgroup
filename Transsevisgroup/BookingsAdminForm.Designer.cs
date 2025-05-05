namespace Transsevisgroup
{
    partial class BookingsAdminForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClearFilters = new System.Windows.Forms.Button();
            this.dataGridBookings = new System.Windows.Forms.DataGridView();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dateFilter = new System.Windows.Forms.DateTimePicker();
            this.comboServiceFilter = new System.Windows.Forms.ComboBox();
            this.comboUserFilter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBookings)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClearFilters
            // 
            this.btnClearFilters.Location = new System.Drawing.Point(128, 237);
            this.btnClearFilters.Name = "btnClearFilters";
            this.btnClearFilters.Size = new System.Drawing.Size(200, 53);
            this.btnClearFilters.TabIndex = 28;
            this.btnClearFilters.Text = "button1";
            this.btnClearFilters.UseVisualStyleBackColor = true;
            this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
            // 
            // dataGridBookings
            // 
            this.dataGridBookings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridBookings.Location = new System.Drawing.Point(12, 296);
            this.dataGridBookings.Name = "dataGridBookings";
            this.dataGridBookings.RowHeadersWidth = 62;
            this.dataGridBookings.RowTemplate.Height = 28;
            this.dataGridBookings.Size = new System.Drawing.Size(1392, 435);
            this.dataGridBookings.TabIndex = 27;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(51, 20);
            this.lblTitle.TabIndex = 25;
            this.lblTitle.Text = "label1";
            // 
            // dateFilter
            // 
            this.dateFilter.Location = new System.Drawing.Point(240, 66);
            this.dateFilter.Name = "dateFilter";
            this.dateFilter.Size = new System.Drawing.Size(406, 26);
            this.dateFilter.TabIndex = 29;
            // 
            // comboServiceFilter
            // 
            this.comboServiceFilter.FormattingEnabled = true;
            this.comboServiceFilter.Location = new System.Drawing.Point(240, 124);
            this.comboServiceFilter.Name = "comboServiceFilter";
            this.comboServiceFilter.Size = new System.Drawing.Size(406, 28);
            this.comboServiceFilter.TabIndex = 30;
            // 
            // comboUserFilter
            // 
            this.comboUserFilter.FormattingEnabled = true;
            this.comboUserFilter.Location = new System.Drawing.Point(240, 178);
            this.comboUserFilter.Name = "comboUserFilter";
            this.comboUserFilter.Size = new System.Drawing.Size(406, 28);
            this.comboUserFilter.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 32;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 33;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 20);
            this.label3.TabIndex = 34;
            this.label3.Text = "label3";
            // 
            // BookingsAdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1416, 783);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboUserFilter);
            this.Controls.Add(this.comboServiceFilter);
            this.Controls.Add(this.dateFilter);
            this.Controls.Add(this.btnClearFilters);
            this.Controls.Add(this.dataGridBookings);
            this.Controls.Add(this.lblTitle);
            this.Name = "BookingsAdminForm";
            this.Text = "BookingsAdminForm";
            this.Load += new System.EventHandler(this.BookingsAdminForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBookings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClearFilters;
        private System.Windows.Forms.DataGridView dataGridBookings;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DateTimePicker dateFilter;
        private System.Windows.Forms.ComboBox comboServiceFilter;
        private System.Windows.Forms.ComboBox comboUserFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}