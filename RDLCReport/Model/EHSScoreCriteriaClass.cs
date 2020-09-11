using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDLCReport.Model
{
    public class EHSScoreCriteriaClass
    {
        public decimal? SRNo { get; set; }
        public string Area { get; set; }
        public string TotalWeightage { get; set; }
        public string Parameter { get; set; }
        public decimal? SubWeightage { get; set; }
        public string Applicable { get; set; }
        public decimal? ApplicableWeightage { get; set; }
        public string PercentageCompliances { get; set; }
        public decimal? Compliances { get; set; }
        public string Remark { get; set; }
        public int RequirementId { get; set; }
    }
}