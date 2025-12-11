using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkDBApp
{
    public enum ReportParamType
    {
        Date,
        DateTime,
        Text,
        Int,
        Decimal,
        Bool,
        Combo
    }
    public class ReportParameter
    {
        public string Name { get; init; }
        public string Label { get; init; }
        public ReportParamType Type { get; init; }
        public object DefaultValue { get; init; } = null;
        public List<KeyValuePair<string, object>> Options { get; init; } = null;
        public bool AllowNull { get; init; }
    }
}
