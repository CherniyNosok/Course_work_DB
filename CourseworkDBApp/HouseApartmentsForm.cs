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
    public partial class HouseApartmentsForm : Form
    {
        private int houseCode;
        private BindingSource bs = new BindingSource();
        private NpgsqlDataAdapter adapter;
        private DataTable dt;
        public HouseApartmentsForm(int houseCode)
        {
            InitializeComponent();
            this.houseCode = houseCode;
            Load += HouseApartmentsForm_Load;
        }

        private void HouseApartmentsForm_Load(object sender, EventArgs e)
        {
            var sql = $"SELECT street, house_number, building FROM houses WHERE house_code = {houseCode};";
            var house = DbUtils.GetDataTable(sql);

            if (house.Rows.Count > 0)
            {
                var r = house.Rows[0];
                lblHouse.Text = $"{r["street"]} {r["house_number"]} {(r["building"] == DBNull.Value ? "" : "корп. " + r["building"])}";
            }

            sql = $"SELECT * FROM apartments WHERE house_code = {houseCode} ORDER BY apt_number;";
            dt = DbUtils.GetDataTable(sql);
            adapter = new NpgsqlDataAdapter();
            adapter.SelectCommand = new NpgsqlCommand("SELECT * FROM apartments WHERE house_code = @h", new NpgsqlConnection(DbUtils.connStr));
            adapter.SelectCommand.Parameters.AddWithValue("h", houseCode);
            var cb = new NpgsqlCommandBuilder(adapter);

            bs.DataSource = dt;
            dgvApts.DataSource = bs;

            btnAdd.Click += BtnAdd_Click;
            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var newRow = dt.NewRow();
            newRow["house_code"] = houseCode;

            dt.Rows.Add(newRow);
            bs.MoveLast();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                adapter.UpdateCommand = (new NpgsqlCommandBuilder(adapter)).GetUpdateCommand();
                adapter.InsertCommand = (new NpgsqlCommandBuilder(adapter)).GetInsertCommand();
                adapter.DeleteCommand = (new NpgsqlCommandBuilder(adapter)).GetDeleteCommand();

                if (!adapter.InsertCommand.Parameters.Contains("house_code"))
                {
                    adapter.InsertCommand.Parameters.Add(new NpgsqlParameter("house_code", houseCode));
                }

                adapter.Update(dt);
                MessageBox.Show("Saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: "+ ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete apartment?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bs.RemoveCurrent();
                try
                {
                    adapter.Update(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " +  ex.Message);
                }
            }
        }
    }
}
