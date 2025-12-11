namespace CourseworkDBApp
{
    partial class ReportPanel
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
            pnlParams = new Panel();
            btnGenerate = new Button();
            lblTotals = new Label();
            dgvTable = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvTable).BeginInit();
            SuspendLayout();
            // 
            // pnlParams
            // 
            pnlParams.AutoScroll = true;
            pnlParams.Location = new Point(713, 12);
            pnlParams.Name = "pnlParams";
            pnlParams.Size = new Size(484, 426);
            pnlParams.TabIndex = 0;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(12, 405);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(123, 33);
            btnGenerate.TabIndex = 1;
            btnGenerate.Text = "Generate";
            btnGenerate.UseVisualStyleBackColor = true;
            // 
            // lblTotals
            // 
            lblTotals.AutoSize = true;
            lblTotals.Location = new Point(141, 414);
            lblTotals.Name = "lblTotals";
            lblTotals.Size = new Size(38, 15);
            lblTotals.TabIndex = 2;
            lblTotals.Text = "Totals";
            // 
            // dgvTable
            // 
            dgvTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTable.Location = new Point(12, 12);
            dgvTable.Name = "dgvTable";
            dgvTable.Size = new Size(695, 387);
            dgvTable.TabIndex = 3;
            // 
            // ReportPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1209, 450);
            Controls.Add(dgvTable);
            Controls.Add(lblTotals);
            Controls.Add(btnGenerate);
            Controls.Add(pnlParams);
            Name = "ReportPanel";
            Text = "ReportPanel";
            ((System.ComponentModel.ISupportInitialize)dgvTable).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlParams;
        private Button btnGenerate;
        private Label lblTotals;
        private DataGridView dgvTable;
    }
}