namespace CourseworkDBApp
{
    partial class HouseApartmentsForm
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
            lblHouse = new Label();
            dgvApts = new DataGridView();
            btnAdd = new Button();
            btnDelete = new Button();
            btnSave = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvApts).BeginInit();
            SuspendLayout();
            // 
            // lblHouse
            // 
            lblHouse.AutoSize = true;
            lblHouse.Location = new Point(12, 9);
            lblHouse.Name = "lblHouse";
            lblHouse.Size = new Size(54, 15);
            lblHouse.TabIndex = 0;
            lblHouse.Text = "lblHouse";
            // 
            // dgvApts
            // 
            dgvApts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvApts.Location = new Point(12, 27);
            dgvApts.Name = "dgvApts";
            dgvApts.Size = new Size(776, 259);
            dgvApts.TabIndex = 1;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(12, 292);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(12, 321);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 23);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(12, 350);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // HouseApartmentsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSave);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(dgvApts);
            Controls.Add(lblHouse);
            Name = "HouseApartmentsForm";
            Text = "HouseApartmentsForm";
            ((System.ComponentModel.ISupportInitialize)dgvApts).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblHouse;
        private DataGridView dgvApts;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnSave;
    }
}