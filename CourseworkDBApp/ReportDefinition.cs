using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkDBApp
{
    public class ReportDefinition
    {
        public string Title { get; init; }
        public string sql { get; init; }
        public List<ReportParameter> Parameters { get; init; } = new List<ReportParameter>();
        public string[] SumColumns { get; init; } = new string[0];
        public string DefaultOrderBy { get; init; } = null;
    }
}
