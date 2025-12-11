using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace CourseworkDBApp
{
    public partial class ReportPanel : Form
    {
        private readonly ReportDefinition definition;
        private readonly Dictionary<string, Control> paramControls = new Dictionary<string, Control>();
        public ReportPanel(ReportDefinition definition)
        {
            InitializeComponent();
            this.definition = definition;
            Text = definition.Title;
            Load += ReportPanel_Load;
        }

        private void ReportPanel_Load(object sender, EventArgs e)
        {
            BuildUI();
            btnGenerate.Click += BtnGenerate_Click;
        }

        private void BuildUI()
        {
            pnlParams.Controls.Clear();

            int y = 6;
            int labelW = 160;
            int ctrlW = 200;
            int h = 26;

            foreach (var p in definition.Parameters)
            {
                var lbl = new Label
                {
                    Left = 6,
                    Top = y + 4,
                    Width = labelW,
                    Text = p.Label + ":"
                };
                pnlParams.Controls.Add(lbl);

                Control ctrl = p.Type switch
                {
                    ReportParamType.Date => new DateTimePicker
                    {
                        Format = DateTimePickerFormat.Short,
                        Width = ctrlW,
                        Left = 6 + labelW,
                        Top = y
                    },
                    ReportParamType.DateTime => new DateTimePicker
                    {
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "yyyy-MM-dd HH:mm:ss",
                        Width = ctrlW,
                        Left = 6 + labelW,
                        Top = y
                    },
                    ReportParamType.Text => new TextBox
                    {
                        Width = ctrlW,
                        Left = 6 + labelW,
                        Top = y
                    },
                    ReportParamType.Int => new NumericUpDown
                    {
                        Width = ctrlW,
                        Left = 6 + labelW,
                        Top = y,
                        Minimum = int.MinValue,
                        Maximum = int.MaxValue,
                        DecimalPlaces = 0
                    },
                    ReportParamType.Decimal => new NumericUpDown
                    {
                        Width = ctrlW,
                        Left = 6 + labelW,
                        Top = y,
                        Minimum = decimal.MinValue,
                        Maximum = decimal.MaxValue,
                        DecimalPlaces = 2,
                        Increment = 0.01M
                    },
                    ReportParamType.Bool => new CheckBox
                    {
                        Width = ctrlW,
                        Left = 6 + labelW,
                        Top = y + 2
                    },
                    ReportParamType.Combo => new ComboBox
                    {
                        Width = ctrlW,
                        Left = 6 + labelW,
                        Top = y,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    },
                    _ => new TextBox
                    {
                        Width = ctrlW,
                        Left = 6 + labelW,
                        Top = y
                    }
                };

                if (p.Type == ReportParamType.Combo && p.Options != null)
                {
                    var cb = (ComboBox)ctrl;
                    cb.Items.AddRange(p.Options.Select(kv => kv.Key).ToArray());
                    if (p.DefaultValue != null)
                    {
                        var idx = p.Options.FindIndex(kv => Equals(kv.Value, p.DefaultValue));
                        if (idx >= 0)
                            cb.SelectedIndex = idx;
                    }
                }
                else if ((p.Type == ReportParamType.Date || p.Type == ReportParamType.DateTime) && p.DefaultValue is DateTime dt)
                {
                    ((DateTimePicker)ctrl).Value = dt;
                }
                else if (p.Type == ReportParamType.Text && p.DefaultValue != null)
                {
                    ((TextBox)ctrl).Text = p.DefaultValue.ToString();
                }
                else if (p.Type == ReportParamType.Int && p.DefaultValue is int iv)
                {
                    ((NumericUpDown)ctrl).Value = iv;
                }
                else if (p.Type == ReportParamType.Decimal && p.DefaultValue is decimal dv)
                {
                    ((NumericUpDown)ctrl).Value = dv;
                }
                else if (p.Type == ReportParamType.Bool && p.DefaultValue is bool bv)
                {
                    ((CheckBox)ctrl).Checked = bv;
                }

                paramControls[p.Name] = ctrl;

                pnlParams.Controls.Add(ctrl);
                y += h + 8;
            }

            var lblSort = new Label
            {
                Left = 6,
                Top = y + 4,
                Width = labelW,
                Text = "Order by:"
            };
            var txtSort = new TextBox
            {
                Left = 6 + labelW,
                Top = y,
                Width = 360,
                Name = "txtOrderBy"
            };
            if (!string.IsNullOrEmpty(definition.DefaultOrderBy))
                txtSort.Text = definition.DefaultOrderBy;

            pnlParams.Controls.Add(lblSort);
            pnlParams.Controls.Add(txtSort);
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            RunReport();
        }

        private void RunReport()
        {
            try
            {
                var sql = definition.sql;
                string orderBy = "";
                var txt = pnlParams.Controls.Find("txtOrderBy", true).FirstOrDefault() as TextBox;
                if (txt != null && !string.IsNullOrWhiteSpace(txt.Text))
                {
                    orderBy = txt.Text.Trim();
                }

                if (sql.Contains("{ORDER_BY}"))
                {
                    sql = sql.Replace("{ORDER_BY}", string.IsNullOrWhiteSpace(orderBy) ? "" : " ORDER BY " + orderBy);
                }

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                foreach (var p in definition.Parameters)
                {
                    if (!paramControls.TryGetValue(p.Name, out var ctrl))
                        continue;

                    object value = GetControlValue(ctrl, p);
                    if (value == null && p.AllowNull)
                    {
                        parameters.Add(new NpgsqlParameter(p.Name, DBNull.Value));
                    }
                    else
                    {
                        parameters.Add(new NpgsqlParameter(p.Name, value ?? DBNull.Value));
                    }
                }

                var dt = DbUtils.GetDataTable(sql, parameters.ToArray());
                dgvTable.DataSource = dt;
                CalculateSums(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        private object GetControlValue(Control ctrl, ReportParameter p)
        {
            return p.Type switch
            {
                ReportParamType.Date => ((DateTimePicker)ctrl).Value.Date,
                ReportParamType.DateTime => ((DateTimePicker)ctrl).Value,
                ReportParamType.Text => string.IsNullOrWhiteSpace(((TextBox)ctrl).Text) ? (p.AllowNull ? null : "") : ((TextBox)ctrl).Text,
                ReportParamType.Int => Convert.ToInt32(((NumericUpDown)ctrl).Value),
                ReportParamType.Decimal => Convert.ToDecimal(((NumericUpDown)ctrl).Value),
                ReportParamType.Bool => ((CheckBox)ctrl).Checked,
                ReportParamType.Combo => GetComboValue((ComboBox)ctrl, p),
                _ => null
            };
        }

        private object GetComboValue(ComboBox cb, ReportParameter p)
        {
            if (cb.SelectedIndex < 0)
                return p.AllowNull ? null : (p.Options?.FirstOrDefault().Value);

            var key = cb.SelectedItem.ToString();
            var kv = p.Options?.FirstOrDefault(x => x.Key == key);

            return kv.Equals(default(KeyValuePair<string, object>)) ? (object)key : kv.Value.Value;
        }

        private void CalculateSums(DataTable dt)
        {
            if (definition.SumColumns == null || definition.SumColumns.Length == 0)
            {
                lblTotals.Text = "";
                return;
            }

            var sums = new Dictionary<string, decimal>();
            foreach (var col in definition.SumColumns)
            {
                if (!dt.Columns.Contains(col))
                    continue;
                decimal s = 0;
                foreach (DataRow r in dt.Rows)
                {
                    if (r.IsNull(col))
                        continue;
                    try
                    {
                        s += Convert.ToDecimal(r[col]);
                    }
                    catch { }
                }
                sums[col] = s;
            }

            var parts = sums.Select(kv => $"{kv.Key}: {kv.Value:N2}");
            lblTotals.Text = "Total - " + string.Join(" | ", parts);
        }
    }
}
