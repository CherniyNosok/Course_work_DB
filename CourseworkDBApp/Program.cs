namespace CourseworkDBApp
{
    internal static class Program
    {
        public static List<ReportDefinition> reports = new();

        [STAThread]
        static void Main()
        {
            LoadReports();
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        private static void LoadReports()
        {
            reports.Add(new ReportDefinition
            {
                Title = "Payments on apartments",
                sql = @"
                    SELECT h.street, h.house_number, a.apt_number,
                        SUM(p.amount) AS total_paid,
                        COUNT(p.payment_code) AS payments_count,
                        MAX(p.pay_date) AS last_payment
                    FROM apartments a
                    JOIN houses h ON a.house_code = h.house_code
                    LEFT JOIN payments p ON p.apartment_code = a.apartment_code AND (p.pay_date BETWEEN @from AND @to)
                    GROUP BY h.street, h.house_number, a.apt_number
                    {ORDER_BY}
                ",
                Parameters = new List<ReportParameter>
                {
                    new ReportParameter
                    {
                        Name = "from", Label = "Date from",
                        Type = ReportParamType.Date,
                        DefaultValue = DateTime.Today.AddMonths(-1), AllowNull = false
                    },
                    new ReportParameter
                    {
                        Name = "to", Label = "Date to",
                        Type = ReportParamType.Date,
                        DefaultValue = DateTime.Today, AllowNull = false
                    },
                    new ReportParameter
                    {
                        Name = "onlyResponsible", Label = "Only Responsible",
                        Type = ReportParamType.Bool,
                        DefaultValue = false, AllowNull = true
                    }
                    },
                SumColumns = new[] { "total_paid" },
                DefaultOrderBy = "total_paid DESC"
            });

            reports.Add(new ReportDefinition
            {
                Title = "Residents by sections",
                sql = @"
                    SELECT s.service_code, s.dept_code, s.section_code, s.name AS section_name,
                           h.street, h.house_number,
                           COUNT(r.resident_code) AS residents_count,
                           AVG(a.living_area) AS avg_area
                    FROM sections s
                    JOIN houses h ON h.service_code = s.service_code AND h.dept_code = s.dept_code AND h.section_code = s.section_code
                    JOIN apartments a ON a.house_code = h.house_code
                    LEFT JOIN residents r ON r.apartment_code = a.apartment_code
                    WHERE (@section IS NULL OR s.section_code = @section)
                    GROUP BY s.service_code, s.dept_code, s.section_code, s.name, h.street, h.house_number
                    HAVING COUNT(r.resident_code) >= @minResidents
                    {ORDER_BY}
                    ",
                Parameters = new List<ReportParameter> 
                {
                    new ReportParameter 
                    { 
                        Name = "section", Label = "section code", 
                        Type = ReportParamType.Int, 
                        DefaultValue = null, AllowNull = true 
                    },
                    new ReportParameter 
                    { 
                        Name = "minResidents", Label = "min residents", 
                        Type = ReportParamType.Int, 
                        DefaultValue = 1, AllowNull = false 
                    }
                },
                SumColumns = new[] { "residents_count" },
                DefaultOrderBy = "residents_count DESC"
            });

            reports.Add(new ReportDefinition
            {
                Title = "Using tariffs",
                sql = @"
                    SELECT t.tariff_code, t.rate, t.hot_water, t.elevator, h.street, h.house_number,
                           COUNT(a.apartment_code) AS apt_count,
                           SUM(a.total_area) AS total_area_sum
                    FROM tariffs t
                    JOIN apartments a ON a.tariff_id = t.tariff_code
                    JOIN houses h ON a.house_code = h.house_code
                    WHERE (t.hot_water = @hotWater OR @hotWater IS NULL)
                      AND (t.elevator = @elevator OR @elevator IS NULL)
                      AND (t.rate BETWEEN @rateFrom AND @rateTo)
                    GROUP BY t.tariff_code, t.rate, t.hot_water, t.elevator, h.street, h.house_number
                    {ORDER_BY}
                    ",
                Parameters = new List<ReportParameter> {
                    new ReportParameter { Name = "hotWater", Label = "Hot water (yes/no/all)",
                        Type = ReportParamType.Combo,
                        Options = new List<KeyValuePair<string, object>> {
                            new("all", null),
                            new("yes", true),
                            new("no", false)
                        },
                        DefaultValue = null, AllowNull = true },
                    new ReportParameter { Name = "elevator", Label = "Elevator (yes/no/all)",
                        Type = ReportParamType.Combo,
                        Options = new List<KeyValuePair<string, object>> {
                            new("all", null), 
                            new("yes", true),
                            new("no", false)
                        }, DefaultValue = null, AllowNull = true
                    },
                    new ReportParameter
                    { 
                        Name = "rateFrom", Label = "tariff from", 
                        Type = ReportParamType.Decimal, 
                        DefaultValue = 0m, AllowNull = false 
                    },
                    new ReportParameter 
                    { 
                        Name = "rateTo", Label = "tariff to", 
                        Type = ReportParamType.Decimal, 
                        DefaultValue = 99999m, AllowNull = false 
                    }
                },
                SumColumns = new[] { "apt_count", "total_area_sum" },
                DefaultOrderBy = "apt_count DESC"
            });
        }
    }
}