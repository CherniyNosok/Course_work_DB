namespace CourseworkDBApp
{
    partial class MainForm
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
            gridTable = new DataGridView();
            comboTables = new ComboBox();
            btnAdd = new Button();
            btnDelete = new Button();
            btnSave = new Button();
            btnApplyFilter = new Button();
            btnOpenRelation = new Button();
            txtFilterField = new ComboBox();
            txtFilterValue = new TextBox();
            btnReports = new Button();
            cbReports = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)gridTable).BeginInit();
            SuspendLayout();
            // 
            // gridTable
            // 
            gridTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridTable.Location = new Point(12, 41);
            gridTable.Name = "gridTable";
            gridTable.Size = new Size(938, 256);
            gridTable.TabIndex = 0;
            // 
            // comboTables
            // 
            comboTables.FormattingEnabled = true;
            comboTables.Location = new Point(12, 12);
            comboTables.Name = "comboTables";
            comboTables.Size = new Size(351, 23);
            comboTables.TabIndex = 1;
            comboTables.Text = "Таблицы";
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(841, 303);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(109, 35);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(841, 344);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(109, 35);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(841, 385);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(109, 35);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Location = new Point(369, 303);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(109, 23);
            btnApplyFilter.TabIndex = 5;
            btnApplyFilter.Text = "Filter";
            btnApplyFilter.UseVisualStyleBackColor = true;
            // 
            // btnOpenRelation
            // 
            btnOpenRelation.Location = new Point(12, 332);
            btnOpenRelation.Name = "btnOpenRelation";
            btnOpenRelation.Size = new Size(109, 35);
            btnOpenRelation.TabIndex = 6;
            btnOpenRelation.Text = "Open Relation";
            btnOpenRelation.UseVisualStyleBackColor = true;
            // 
            // txtFilterField
            // 
            txtFilterField.FormattingEnabled = true;
            txtFilterField.Location = new Point(12, 303);
            txtFilterField.Name = "txtFilterField";
            txtFilterField.Size = new Size(182, 23);
            txtFilterField.TabIndex = 7;
            // 
            // txtFilterValue
            // 
            txtFilterValue.Location = new Point(200, 303);
            txtFilterValue.Name = "txtFilterValue";
            txtFilterValue.Size = new Size(163, 23);
            txtFilterValue.TabIndex = 8;
            // 
            // btnReports
            // 
            btnReports.Location = new Point(200, 373);
            btnReports.Name = "btnReports";
            btnReports.Size = new Size(109, 23);
            btnReports.TabIndex = 10;
            btnReports.Text = "Open report";
            btnReports.UseVisualStyleBackColor = true;
            // 
            // cbReports
            // 
            cbReports.FormattingEnabled = true;
            cbReports.Location = new Point(12, 373);
            cbReports.Name = "cbReports";
            cbReports.Size = new Size(182, 23);
            cbReports.TabIndex = 11;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(962, 434);
            Controls.Add(cbReports);
            Controls.Add(btnReports);
            Controls.Add(txtFilterValue);
            Controls.Add(txtFilterField);
            Controls.Add(btnOpenRelation);
            Controls.Add(btnApplyFilter);
            Controls.Add(btnSave);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(comboTables);
            Controls.Add(gridTable);
            Name = "MainForm";
            Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)gridTable).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView gridTable;
        private ComboBox comboTables;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnSave;
        private Button btnApplyFilter;
        private Button btnOpenRelation;
        private ComboBox txtFilterField;
        private TextBox txtFilterValue;
        private Button btnReports;
        private ComboBox cbReports;
    }
}