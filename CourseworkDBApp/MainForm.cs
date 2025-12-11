using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseworkDBApp
{
    public partial class MainForm : Form
    {
        private BindingSource bindingSource = new BindingSource();
        private NpgsqlDataAdapter currentAdapter;
        private DataTable currentTable;
        private string currentTableName;
        public MainForm()
        {
            InitializeComponent();
            Load += FormLoad;
        }

        private void FormLoad(object sender, EventArgs e)
        {
            comboTables.Items.Clear();
            comboTables.Items.AddRange(DbUtils.GetTableNames().ToArray());
            comboTables.Items.AddRange(DbUtils.GetViewNames().ToArray());

            cbReports.Items.Clear();
            cbReports.Items.AddRange(Program.reports.Select(x => x.Title).ToArray());

            comboTables.SelectedIndex = 0;
            comboTables.SelectedIndexChanged += (s, ev) => LoadLogicalTable(comboTables.SelectedItem.ToString());
            gridTable.DataSource = bindingSource;

            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSave.Click += BtnSave_Click;
            btnApplyFilter.Click += BtnApplyFilter_Click;
            btnReports.Click += BtnReports_Click;
            btnOpenRelation.Click += BtnOpenRelation_Click;

            LoadLogicalTable(comboTables.SelectedItem.ToString());
        }

        private void LoadLogicalTable(string tableName)
        {
            currentTableName = tableName;

            string sql = $"SELECT * FROM {currentTableName};";

            //currentTable = DbUtils.GetDataTable(sql);
            //bindingSource.DataSource = currentTable;

            //var baseSelect = $"SELECT * FROM {tableName}";
            //currentAdapter = DbUtils.GetDataAdapter(baseSelect);
            //var dt = new DataTable();
            //currentAdapter.Fill(dt);
            //currentTable = dt;
            //bindingSource.DataSource = currentTable;

            
            currentTable = DbUtils.GetDataTable(sql);
            currentAdapter = DbUtils.GetDataAdapter(sql);
            bindingSource.DataSource = currentTable;

            txtFilterField.Items.Clear();
            txtFilterValue.Clear();

            txtFilterField.Items.AddRange(DbUtils.GetColumns(currentTableName).Select(x => x.name).ToArray());

            txtFilterField.SelectedIndex = 0;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            bindingSource.AddNew();
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bindingSource.RemoveCurrent();
                SaveIfAdapter();
            }
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveIfAdapter();
        }
        private void SaveIfAdapter()
        {
            if (currentAdapter != null && currentTable != null)
            {
                try
                {
                    currentAdapter.Update(currentTable);
                    MessageBox.Show("Saved");
                    LoadLogicalTable(currentTableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Don't allow");
            }
        }
        private void BtnApplyFilter_Click(object sender, EventArgs e)
        {
            var field = txtFilterField.Text.Trim();
            var value = txtFilterValue.Text.Trim();

            if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(value))
            {
                LoadLogicalTable(currentTableName);
                return;
            }

            var where = $"{field} {value}";
            string sql = $"SELECT * FROM {currentTableName} WHERE {where}";

            var dt = DbUtils.GetDataTable(sql);
            bindingSource.DataSource = dt;
        }
        private void BtnReports_Click(object sender, EventArgs e)
        {
            var def = Program.reports.Find(x => x.Title == cbReports.Text);
            using var rp = new ReportPanel(def);
            rp.ShowDialog();
        }
        private void BtnOpenRelation_Click(object sender, EventArgs e)
        {
            if (currentTableName == "houses")
            {
                var drv = bindingSource.Current as DataRowView;
                if (drv == null)
                {
                    MessageBox.Show("Choose house");
                    return;
                }
                int houseCode = Convert.ToInt32(drv["house_code"]);
                var f = new HouseApartmentsForm(houseCode);
                f.ShowDialog();
                LoadLogicalTable("houses");
            }
            else
            {
                MessageBox.Show("Only for houses");
            }
        }
    }
}
