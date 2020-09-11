using Microsoft.Reporting.WebForms;
using RDLCReport.Data;
using RDLCReport.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RDLCReport
{
    public partial class CorpAssessorCriteria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int EhsAssessmentId = Convert.ToInt32(Request.QueryString["EhsAssesmentID"]);
            if (!Page.IsPostBack)
            {
                EhsReport(EhsAssessmentId);
            }
        }

        public void EhsReport(int EhsAssesmentID)
        {
            SafetyAppDBEntities db = new SafetyAppDBEntities();
            if (EhsAssesmentID != 0)
            {

                var EhsTransaction = db.tb_ehs_assessment_score_transaction.Where(x => x.EhsAssesmentID == EhsAssesmentID).Select(x =>
                     new EHSScoreCriteriaClass
                     {
                         SRNo = x.tb_ehs_req_area.SrNo,
                         Area = x.tb_ehs_req_area.AreaName,
                         TotalWeightage = x.tb_ehs_req_area.TotalWeightagePerCriteria.ToString(),
                         Parameter = x.tb_ehs_req_area_praram.ParameterName,
                         SubWeightage = x.tb_ehs_req_area_praram.SubWeightagePerCriteria,
                         Applicable = x.IsApplicable.ToString(),
                         ApplicableWeightage = x.ApplicableWeightage,
                         PercentageCompliances = x.Assessor1PercentageCompliance,
                         Compliances = x.Accessor1Compliance,
                         Remark = x.Accessor1Remark,
                         RequirementId = x.tb_ehs_req_area.RequirementId,
                     }).ToList();


                if (EhsTransaction.Any() && EhsTransaction != null)
                {
                    var data = EhsTransaction.GroupBy(x => x.Area).ToList();

                    EhsTransaction = new List<EHSScoreCriteriaClass>();

                    foreach (var item in data)
                    {
                        decimal? subWeightage = 0;
                        decimal? applicableWeightage = 0;
                        decimal? compliances = 0;

                        foreach (var itemdata in item.ToList())
                        {
                            var model = new EHSScoreCriteriaClass
                            {
                                SRNo = itemdata.SRNo,
                                Area = itemdata.Area,
                                TotalWeightage = itemdata.TotalWeightage,
                                Parameter = itemdata.Parameter,
                                SubWeightage = itemdata.SubWeightage,
                                Applicable = itemdata.Applicable,
                                ApplicableWeightage = itemdata.ApplicableWeightage,
                                PercentageCompliances = itemdata.PercentageCompliances,
                                Compliances = itemdata.Compliances,
                                Remark = itemdata.Remark,
                                RequirementId = itemdata.RequirementId
                            };
                            EhsTransaction.Add(model);
                            subWeightage += model.SubWeightage;
                            applicableWeightage += model.ApplicableWeightage;
                            compliances += model.Compliances;
                        }
                        var Total = new EHSScoreCriteriaClass
                        {
                            SRNo = EhsTransaction.Last().SRNo,
                            Area = EhsTransaction.Last().Area,
                            TotalWeightage = EhsTransaction.Last().TotalWeightage.ToString(),
                            Applicable = EhsTransaction.Last().Applicable,
                            SubWeightage = subWeightage,
                            ApplicableWeightage = applicableWeightage,
                            Compliances = compliances,
                            RequirementId = EhsTransaction.Last().RequirementId
                        };
                        EhsTransaction.Add(Total);
                    }
                }

                var EhsTransaction1 = EhsTransaction.FindAll(x => x.RequirementId == 1);
                var EhsTransaction2 = EhsTransaction.FindAll(x => x.RequirementId == 2);
                var EhsTransaction3 = EhsTransaction.FindAll(x => x.RequirementId == 3);
                var EhsTransaction4 = EhsTransaction.FindAll(x => x.RequirementId == 4);


                var SubWeightage1 = EhsTransaction1.Sum(x => x.SubWeightage) / 2;
                var ApplicableWeightage1 = EhsTransaction1.Sum(x => x.ApplicableWeightage) / 2;
                var Compliances1 = EhsTransaction1.Sum(x => x.Compliances) / 2;

                var SubWeightage2 = EhsTransaction2.Sum(x => x.SubWeightage) / 2;
                var ApplicableWeightage2 = EhsTransaction2.Sum(x => x.ApplicableWeightage) / 2;
                var Compliances2 = EhsTransaction2.Sum(x => x.Compliances) / 2;

                var SubWeightage3 = EhsTransaction3.Sum(x => x.SubWeightage) / 2;
                var ApplicableWeightage3 = EhsTransaction3.Sum(x => x.ApplicableWeightage) / 2;
                var Compliances3 = EhsTransaction3.Sum(x => x.Compliances) / 2;

                var SubWeightage4 = EhsTransaction4.Sum(x => x.SubWeightage) / 2;
                var ApplicableWeightage4 = EhsTransaction4.Sum(x => x.ApplicableWeightage) / 2;
                var Compliances4 = EhsTransaction4.Sum(x => x.Compliances) / 2;



                var TotalSubWeightage = SubWeightage1 + SubWeightage2 + SubWeightage3 + SubWeightage4;
                var TotalApplicableWeightage = ApplicableWeightage1 + ApplicableWeightage2 + ApplicableWeightage3 + ApplicableWeightage4;
                var TotalCompliances = Compliances1 + Compliances2 + Compliances3 + Compliances4;


                ReportParameter rp1 = new ReportParameter("SubWeightage1", SubWeightage1.ToString());
                ReportParameter rp2 = new ReportParameter("ApplicableWeightage1", ApplicableWeightage1.ToString());
                ReportParameter rp3 = new ReportParameter("Compliances1", Compliances1.ToString());
                ReportParameter rp4 = new ReportParameter("SubWeightage2", SubWeightage2.ToString());
                ReportParameter rp5 = new ReportParameter("ApplicableWeightage2", ApplicableWeightage2.ToString());
                ReportParameter rp6 = new ReportParameter("Compliances2", Compliances2.ToString());
                ReportParameter rp7 = new ReportParameter("SubWeightage3", SubWeightage3.ToString());
                ReportParameter rp8 = new ReportParameter("ApplicableWeightage3", ApplicableWeightage3.ToString());
                ReportParameter rp9 = new ReportParameter("Compliances3", Compliances3.ToString());
                ReportParameter rp10 = new ReportParameter("SubWeightage4", SubWeightage4.ToString());
                ReportParameter rp11 = new ReportParameter("ApplicableWeightage4", ApplicableWeightage4.ToString());
                ReportParameter rp12 = new ReportParameter("Compliances4", Compliances4.ToString());
                ReportParameter rp13 = new ReportParameter("TotalSubWeightage", TotalSubWeightage.ToString());
                ReportParameter rp14 = new ReportParameter("TotalApplicableWeightage", TotalApplicableWeightage.ToString());
                ReportParameter rp15 = new ReportParameter("TotalCompliances", TotalCompliances.ToString());




                ReportViewer1.Visible = true;
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCriteria.rdlc");
                var scoreCardId = db.tb_ehs_assessment_score.FirstOrDefault(x => x.EhsAssesmentId == EhsAssesmentID).ScorecardID;
                var scorecardName = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == scoreCardId).ScoreCardName.Substring(21);
                ReportParameter ScoreCardName = new ReportParameter("ScoreCardName", scorecardName);

                ReportDataSource rds = new ReportDataSource("EHSSCriteria1", EhsTransaction1);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria2", EhsTransaction2));
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria3", EhsTransaction3));
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria4", EhsTransaction4));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp2, rp3, rp4, rp5, rp6, rp7, rp8, rp10, rp11, rp12, rp13, rp14, rp15, ScoreCardName });
                ReportViewer1.Width = 1000;
                ReportViewer1.Height = 800;
                ReportViewer1.LocalReport.DataSources.Add(rds);
            }
        }
    }
}