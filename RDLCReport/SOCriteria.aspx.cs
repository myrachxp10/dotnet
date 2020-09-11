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
    public partial class SOCriteria : System.Web.UI.Page
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
                var EhsScore = db.tb_ehs_assessment_score.Where(z => z.EhsAssesmentId == EhsAssesmentID).Where(z => z.EhsAssesmentId == EhsAssesmentID).Select(x => new
                {
                    ScoreCardId = x.ScorecardID
                }).FirstOrDefault();
                var EhsTransaction = db.tb_ehs_assessment_score_transaction.Where(x => x.EhsAssesmentID == EhsAssesmentID).Select(x =>
                     new EHSScoreCriteriaClass
                     {
                         SRNo = x.tb_ehs_req_area.SrNo,
                         Area = x.tb_ehs_req_area.AreaName,
                         //TotalWeightage = x.AreaTotalWeightagePerCriteria.ToString(),
                         //Parameter = x.ParameterName,
                         //SubWeightage = x.SubWeightagePerCriteria,
                         TotalWeightage = x.tb_ehs_req_area.TotalWeightagePerCriteria.ToString(),
                         Parameter = x.tb_ehs_req_area_praram.ParameterName,
                         SubWeightage = x.tb_ehs_req_area_praram.SubWeightagePerCriteria,
                         Applicable = x.IsApplicable.ToString(),
                         ApplicableWeightage = x.ApplicableWeightage,
                         PercentageCompliances = x.PercentageCompliance,
                         Compliances = x.Compliance,
                         Remark = x.Remark,
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

                switch (EhsScore.ScoreCardId)
                {
                    case 1:
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
                        var scoreCardId1 = db.tb_ehs_assessment_score.FirstOrDefault(x => x.EhsAssesmentId == EhsAssesmentID).ScorecardID;
                        var scorecardName1 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == scoreCardId1).ScoreCardName.Substring(21);
                        ReportParameter ScoreCardName1 = new ReportParameter("ScoreCardName", scorecardName1);

                        ReportDataSource rds = new ReportDataSource("EHSSCriteria1", EhsTransaction1);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria2", EhsTransaction2));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria3", EhsTransaction3));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria4", EhsTransaction4));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp2, rp3, rp4, rp5, rp6, rp7, rp8, rp10, rp11, rp12, rp13, rp14, rp15, ScoreCardName1 });
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        break;
                    case 2:
                        var EhsTransaction5 = EhsTransaction.FindAll(x => x.RequirementId == 5);
                        var EhsTransaction6 = EhsTransaction.FindAll(x => x.RequirementId == 6);
                        var EhsTransaction7 = EhsTransaction.FindAll(x => x.RequirementId == 7);
                        var EhsTransaction8 = EhsTransaction.FindAll(x => x.RequirementId == 8);


                        var SubWeightage5 = EhsTransaction5.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage5 = EhsTransaction5.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances5 = EhsTransaction5.Sum(x => x.Compliances) / 2;

                        var SubWeightage6 = EhsTransaction6.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage6 = EhsTransaction6.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances6 = EhsTransaction6.Sum(x => x.Compliances) / 2;

                        var SubWeightage7 = EhsTransaction7.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage7 = EhsTransaction7.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances7 = EhsTransaction7.Sum(x => x.Compliances) / 2;

                        var SubWeightage8 = EhsTransaction8.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage8 = EhsTransaction8.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances8 = EhsTransaction8.Sum(x => x.Compliances) / 2;

                        var TotalSubWeightage2 = SubWeightage5 + SubWeightage6 + SubWeightage7 + SubWeightage8;
                        var TotalApplicableWeightage2 = ApplicableWeightage5 + ApplicableWeightage6 + ApplicableWeightage7 + ApplicableWeightage8;
                        var TotalCompliances2 = Compliances5 + Compliances6 + Compliances7 + Compliances8;

                        ReportParameter r2p1 = new ReportParameter("SubWeightage1", SubWeightage5.ToString());
                        ReportParameter r2p2 = new ReportParameter("ApplicableWeightage1", ApplicableWeightage5.ToString());
                        ReportParameter r2p3 = new ReportParameter("Compliances1", Compliances5.ToString());
                        ReportParameter r2p4 = new ReportParameter("SubWeightage2", SubWeightage6.ToString());
                        ReportParameter r2p5 = new ReportParameter("ApplicableWeightage2", ApplicableWeightage6.ToString());
                        ReportParameter r2p6 = new ReportParameter("Compliances2", Compliances6.ToString());
                        ReportParameter r2p7 = new ReportParameter("SubWeightage3", SubWeightage7.ToString());
                        ReportParameter r2p8 = new ReportParameter("ApplicableWeightage3", ApplicableWeightage7.ToString());
                        ReportParameter r2p9 = new ReportParameter("Compliances3", Compliances7.ToString());
                        ReportParameter r2p10 = new ReportParameter("SubWeightage4", SubWeightage8.ToString());
                        ReportParameter r2p11 = new ReportParameter("ApplicableWeightage4", ApplicableWeightage8.ToString());
                        ReportParameter r2p12 = new ReportParameter("Compliances4", Compliances8.ToString());
                        ReportParameter r2p13 = new ReportParameter("TotalSubWeightage", TotalSubWeightage2.ToString());
                        ReportParameter r2p14 = new ReportParameter("TotalApplicableWeightage", TotalApplicableWeightage2.ToString());
                        ReportParameter r2p15 = new ReportParameter("TotalCompliances", TotalCompliances2.ToString());

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCriteria.rdlc");

                        var scoreCardId2 = db.tb_ehs_assessment_score.FirstOrDefault(x => x.EhsAssesmentId == EhsAssesmentID).ScorecardID;
                        var scorecardName2 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == scoreCardId2).ScoreCardName.Substring(21);
                        ReportParameter ScoreCardName2 = new ReportParameter("ScoreCardName", scorecardName2);

                        ReportDataSource rds2 = new ReportDataSource("EHSSCriteria1", EhsTransaction5);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria2", EhsTransaction6));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria3", EhsTransaction7));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria4", EhsTransaction8));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { r2p1, r2p2, r2p3, r2p4, r2p5, r2p6, r2p7, r2p8, r2p10, r2p11, r2p12, r2p13, r2p14, r2p15, ScoreCardName2 });
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds2);

                        break;
                    case 3:
                        var EhsTransaction9 = EhsTransaction.FindAll(x => x.RequirementId == 9);
                        var EhsTransaction10 = EhsTransaction.FindAll(x => x.RequirementId == 10);
                        var EhsTransaction11 = EhsTransaction.FindAll(x => x.RequirementId == 11);
                        var EhsTransaction12 = EhsTransaction.FindAll(x => x.RequirementId == 12);


                        var SubWeightage9 = EhsTransaction9.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage9 = EhsTransaction9.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances9 = EhsTransaction9.Sum(x => x.Compliances) / 2;

                        var SubWeightage10 = EhsTransaction10.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage10 = EhsTransaction10.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances10 = EhsTransaction10.Sum(x => x.Compliances) / 2;

                        var SubWeightage11 = EhsTransaction11.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage11 = EhsTransaction11.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances11 = EhsTransaction11.Sum(x => x.Compliances) / 2;

                        var SubWeightage12 = EhsTransaction12.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage12 = EhsTransaction12.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances12 = EhsTransaction12.Sum(x => x.Compliances) / 2;


                        var TotalSubWeightage3 = SubWeightage9 + SubWeightage10 + SubWeightage11 + SubWeightage12;
                        var TotalApplicableWeightage3 = ApplicableWeightage9 + ApplicableWeightage10 + ApplicableWeightage11 + ApplicableWeightage12;
                        var TotalCompliances3 = Compliances9 + Compliances10+ Compliances11 + Compliances12;

                        ReportParameter r3p1 = new ReportParameter("SubWeightage1", SubWeightage9.ToString());
                        ReportParameter r3p2 = new ReportParameter("ApplicableWeightage1", ApplicableWeightage9.ToString());
                        ReportParameter r3p3 = new ReportParameter("Compliances1", Compliances9.ToString());
                        ReportParameter r3p4 = new ReportParameter("SubWeightage2", SubWeightage10.ToString());
                        ReportParameter r3p5 = new ReportParameter("ApplicableWeightage2", ApplicableWeightage10.ToString());
                        ReportParameter r3p6 = new ReportParameter("Compliances2", Compliances10.ToString());
                        ReportParameter r3p7 = new ReportParameter("SubWeightage3", SubWeightage11.ToString());
                        ReportParameter r3p8 = new ReportParameter("ApplicableWeightage3", ApplicableWeightage11.ToString());
                        ReportParameter r3p9 = new ReportParameter("Compliances3", Compliances11.ToString());
                        ReportParameter r3p10 = new ReportParameter("SubWeightage4", SubWeightage12.ToString());
                        ReportParameter r3p11 = new ReportParameter("ApplicableWeightage4", ApplicableWeightage12.ToString());
                        ReportParameter r3p12 = new ReportParameter("Compliances4", Compliances12.ToString());
                        ReportParameter r3p13 = new ReportParameter("TotalSubWeightage", TotalSubWeightage3.ToString());
                        ReportParameter r3p14 = new ReportParameter("TotalApplicableWeightage", TotalApplicableWeightage3.ToString());
                        ReportParameter r3p15 = new ReportParameter("TotalCompliances", TotalCompliances3.ToString());

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCriteria.rdlc");
                        var scoreCardId3 = db.tb_ehs_assessment_score.FirstOrDefault(x => x.EhsAssesmentId == EhsAssesmentID).ScorecardID;
                        var scorecardName3 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == scoreCardId3).ScoreCardName.Substring(21);
                        ReportParameter ScoreCardName3 = new ReportParameter("ScoreCardName", scorecardName3);

                        ReportDataSource rds3 = new ReportDataSource("EHSSCriteria1", EhsTransaction9);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria2", EhsTransaction10));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria3", EhsTransaction11));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria4", EhsTransaction12));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { r3p1, r3p2, r3p3, r3p4, r3p5, r3p6, r3p7, r3p8, r3p10, r3p11, r3p12, r3p13, r3p14, r3p15, ScoreCardName3 });
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds3);

                        break;
                    case 4:
                        var EhsTransaction13 = EhsTransaction.FindAll(x => x.RequirementId == 13);
                        var EhsTransaction14 = EhsTransaction.FindAll(x => x.RequirementId == 14);
                        var EhsTransaction15 = EhsTransaction.FindAll(x => x.RequirementId == 15);
                        var EhsTransaction16 = EhsTransaction.FindAll(x => x.RequirementId == 16);
                        var EhsTransaction17 = EhsTransaction.FindAll(x => x.RequirementId == 17);

                        var SubWeightage13 = EhsTransaction13.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage13 = EhsTransaction13.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances13 = EhsTransaction13.Sum(x => x.Compliances) / 2;

                        var SubWeightage14 = EhsTransaction14.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage14 = EhsTransaction14.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances14 = EhsTransaction14.Sum(x => x.Compliances) / 2;

                        var SubWeightage15 = EhsTransaction15.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage15 = EhsTransaction15.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances15 = EhsTransaction15.Sum(x => x.Compliances) / 2;

                        var SubWeightage16 = EhsTransaction16.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage16 = EhsTransaction16.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances16 = EhsTransaction16.Sum(x => x.Compliances) / 2;

                        var SubWeightage17 = EhsTransaction17.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage17 = EhsTransaction17.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances17 = EhsTransaction17.Sum(x => x.Compliances) / 2;

                        var TotalSubWeightage4 = SubWeightage13 + SubWeightage14 + SubWeightage15 + SubWeightage16 + SubWeightage17;
                        var TotalApplicableWeightage4 = ApplicableWeightage13 + ApplicableWeightage14 + ApplicableWeightage15 + ApplicableWeightage16 + ApplicableWeightage17;
                        var TotalCompliances4 = Compliances13 + Compliances14 + Compliances15 + Compliances16 + Compliances17;


                        ReportParameter r4p1 = new ReportParameter("SubWeightage1", SubWeightage13.ToString());
                        ReportParameter r4p2 = new ReportParameter("ApplicableWeightage1", ApplicableWeightage13.ToString());
                        ReportParameter r4p3 = new ReportParameter("Compliances1", Compliances13.ToString());
                        ReportParameter r4p4 = new ReportParameter("SubWeightage2", SubWeightage14.ToString());
                        ReportParameter r4p5 = new ReportParameter("ApplicableWeightage2", ApplicableWeightage14.ToString());
                        ReportParameter r4p6 = new ReportParameter("Compliances2", Compliances14.ToString());
                        ReportParameter r4p7 = new ReportParameter("SubWeightage3", SubWeightage15.ToString());
                        ReportParameter r4p8 = new ReportParameter("ApplicableWeightage3", ApplicableWeightage15.ToString());
                        ReportParameter r4p9 = new ReportParameter("Compliances3", Compliances15.ToString());
                        ReportParameter r4p10 = new ReportParameter("SubWeightage4", SubWeightage16.ToString());
                        ReportParameter r4p11 = new ReportParameter("ApplicableWeightage4", ApplicableWeightage16.ToString());
                        ReportParameter r4p12 = new ReportParameter("Compliances4", Compliances16.ToString());
                        ReportParameter r4p13 = new ReportParameter("TotalSubWeightage", TotalSubWeightage4.ToString());
                        ReportParameter r4p14 = new ReportParameter("TotalApplicableWeightage", TotalApplicableWeightage4.ToString());
                        ReportParameter r4p15 = new ReportParameter("TotalCompliances", TotalCompliances4.ToString());
                        ReportParameter r4p16 = new ReportParameter("SubWeightage5", SubWeightage17.ToString());
                        ReportParameter r4p17 = new ReportParameter("ApplicableWeightage5", ApplicableWeightage17.ToString());
                        ReportParameter r4p18 = new ReportParameter("Compliances5", Compliances17.ToString());

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/mfgEHSScoreCriteria.rdlc");
                        var scoreCardId4 = db.tb_ehs_assessment_score.FirstOrDefault(x => x.EhsAssesmentId == EhsAssesmentID).ScorecardID;
                        var scorecardName4 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == scoreCardId4).ScoreCardName.Substring(21);
                        ReportParameter ScoreCardName4 = new ReportParameter("ScoreCardName", scorecardName4);

                        ReportDataSource rds4 = new ReportDataSource("EHSSCriteria1", EhsTransaction13);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria2", EhsTransaction14));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria3", EhsTransaction15));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria4", EhsTransaction17));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria5", EhsTransaction16));

                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { r4p1, r4p2, r4p3, r4p4, r4p5, r4p6, r4p7, r4p8, r4p10, r4p11, r4p12, r4p13, r4p14, r4p15, ScoreCardName4, r4p16, r4p17, r4p18 });
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds4);

                        break;
                    case 5:
                        var EhsTransaction18 = EhsTransaction.FindAll(x => x.RequirementId == 18);
                        var EhsTransaction19 = EhsTransaction.FindAll(x => x.RequirementId == 19);
                        var EhsTransaction20 = EhsTransaction.FindAll(x => x.RequirementId == 20);
                        var EhsTransaction21 = EhsTransaction.FindAll(x => x.RequirementId == 21);


                        var SubWeightage18 = EhsTransaction18.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage18 = EhsTransaction18.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances18 = EhsTransaction18.Sum(x => x.Compliances) / 2;

                        var SubWeightage19 = EhsTransaction19.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage19 = EhsTransaction19.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances19 = EhsTransaction19.Sum(x => x.Compliances) / 2;

                        var SubWeightage20 = EhsTransaction20.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage20 = EhsTransaction20.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances20 = EhsTransaction20.Sum(x => x.Compliances) / 2;

                        var SubWeightage21 = EhsTransaction21.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage21 = EhsTransaction21.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances21 = EhsTransaction21.Sum(x => x.Compliances) / 2;

                        var TotalSubWeightage5 = SubWeightage18 + SubWeightage19 + SubWeightage20 + SubWeightage21;
                        var TotalApplicableWeightage5 = ApplicableWeightage18 + ApplicableWeightage19 + ApplicableWeightage20 + ApplicableWeightage21;
                        var TotalCompliances5 = Compliances18 + Compliances19 + Compliances20 + Compliances21;
                        
                        ReportParameter r5p1 = new ReportParameter("SubWeightage1", SubWeightage18.ToString());
                        ReportParameter r5p2 = new ReportParameter("ApplicableWeightage1", ApplicableWeightage18.ToString());
                        ReportParameter r5p3 = new ReportParameter("Compliances1", Compliances18.ToString());
                        ReportParameter r5p4 = new ReportParameter("SubWeightage2", SubWeightage19.ToString());
                        ReportParameter r5p5 = new ReportParameter("ApplicableWeightage2", ApplicableWeightage19.ToString());
                        ReportParameter r5p6 = new ReportParameter("Compliances2", Compliances19.ToString());
                        ReportParameter r5p7 = new ReportParameter("SubWeightage3", SubWeightage20.ToString());
                        ReportParameter r5p8 = new ReportParameter("ApplicableWeightage3", ApplicableWeightage20.ToString());
                        ReportParameter r5p9 = new ReportParameter("Compliances3", Compliances20.ToString());
                        ReportParameter r5p10 = new ReportParameter("SubWeightage4", SubWeightage21.ToString());
                        ReportParameter r5p11 = new ReportParameter("ApplicableWeightage4", ApplicableWeightage21.ToString());
                        ReportParameter r5p12 = new ReportParameter("Compliances4", Compliances21.ToString());
                        ReportParameter r5p13 = new ReportParameter("TotalSubWeightage", TotalSubWeightage5.ToString());
                        ReportParameter r5p14 = new ReportParameter("TotalApplicableWeightage", TotalApplicableWeightage5.ToString());
                        ReportParameter r5p15 = new ReportParameter("TotalCompliances", TotalCompliances5.ToString());
                                                                     
                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCriteria.rdlc");
                        var scoreCardId5 = db.tb_ehs_assessment_score.FirstOrDefault(x => x.EhsAssesmentId == EhsAssesmentID).ScorecardID;
                        var scorecardName5 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == scoreCardId5).ScoreCardName.Substring(21);
                        ReportParameter ScoreCardName5 = new ReportParameter("ScoreCardName", scorecardName5);

                        ReportDataSource rds5 = new ReportDataSource("EHSSCriteria1", EhsTransaction18);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria2", EhsTransaction19));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria3", EhsTransaction20));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria4", EhsTransaction21));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { r5p1, r5p2, r5p3, r5p4, r5p5, r5p6, r5p7, r5p8, r5p10, r5p11, r5p12, r5p13, r5p14, r5p15, ScoreCardName5 });
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds5);

                        break;
                    case 6:
                        var EhsTransaction22 = EhsTransaction.FindAll(x => x.RequirementId == 22);
                        var EhsTransaction23 = EhsTransaction.FindAll(x => x.RequirementId == 23);
                        var EhsTransaction24 = EhsTransaction.FindAll(x => x.RequirementId == 24);
                        var EhsTransaction25 = EhsTransaction.FindAll(x => x.RequirementId == 25);

                        var SubWeightage22 = EhsTransaction22.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage22 = EhsTransaction22.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances22 = EhsTransaction22.Sum(x => x.Compliances) / 2;

                        var SubWeightage23 = EhsTransaction23.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage23 = EhsTransaction23.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances23 = EhsTransaction23.Sum(x => x.Compliances) / 2;

                        var SubWeightage24 = EhsTransaction24.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage24 = EhsTransaction24.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances24 = EhsTransaction24.Sum(x => x.Compliances) / 2;

                        var SubWeightage25 = EhsTransaction25.Sum(x => x.SubWeightage) / 2;
                        var ApplicableWeightage25 = EhsTransaction25.Sum(x => x.ApplicableWeightage) / 2;
                        var Compliances25 = EhsTransaction25.Sum(x => x.Compliances) / 2;

                        var TotalSubWeightage6 = SubWeightage22 + SubWeightage23 + SubWeightage24 + SubWeightage25;
                        var TotalApplicableWeightage6 = ApplicableWeightage22 + ApplicableWeightage23 + ApplicableWeightage24 + ApplicableWeightage25;
                        var TotalCompliances6 = Compliances22 + Compliances23 + Compliances24 + Compliances25;

                        ReportParameter r6p1 = new ReportParameter("SubWeightage1", SubWeightage22.ToString());
                        ReportParameter r6p2 = new ReportParameter("ApplicableWeightage1", ApplicableWeightage22.ToString());
                        ReportParameter r6p3 = new ReportParameter("Compliances1", Compliances22.ToString());
                        ReportParameter r6p4 = new ReportParameter("SubWeightage2", SubWeightage23.ToString());
                        ReportParameter r6p5 = new ReportParameter("ApplicableWeightage2", ApplicableWeightage23.ToString());
                        ReportParameter r6p6 = new ReportParameter("Compliances2", Compliances23.ToString());
                        ReportParameter r6p7 = new ReportParameter("SubWeightage3", SubWeightage24.ToString());
                        ReportParameter r6p8 = new ReportParameter("ApplicableWeightage3", ApplicableWeightage24.ToString());
                        ReportParameter r6p9 = new ReportParameter("Compliances3", Compliances24.ToString());
                        ReportParameter r6p10 = new ReportParameter("SubWeightage4", SubWeightage25.ToString());
                        ReportParameter r6p11 = new ReportParameter("ApplicableWeightage4", ApplicableWeightage25.ToString());
                        ReportParameter r6p12 = new ReportParameter("Compliances4", Compliances25.ToString());
                        ReportParameter r6p13 = new ReportParameter("TotalSubWeightage", TotalSubWeightage6.ToString());
                        ReportParameter r6p14 = new ReportParameter("TotalApplicableWeightage", TotalApplicableWeightage6.ToString());
                        ReportParameter r6p15 = new ReportParameter("TotalCompliances", TotalCompliances6.ToString());

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCriteria.rdlc");
                        var scoreCardId6 = db.tb_ehs_assessment_score.FirstOrDefault(x => x.EhsAssesmentId == EhsAssesmentID).ScorecardID;
                        var scorecardName6 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == scoreCardId6).ScoreCardName.Substring(21);
                        ReportParameter ScoreCardName6 = new ReportParameter("ScoreCardName", scorecardName6);

                        ReportDataSource rds6 = new ReportDataSource("EHSSCriteria1", EhsTransaction22);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria2", EhsTransaction23));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria3", EhsTransaction24));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EHSSCriteria4", EhsTransaction25));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { r6p1, r6p2, r6p3, r6p4, r6p5, r6p6, r6p7, r6p8, r6p10, r6p11, r6p12, r6p13, r6p14, r6p15, ScoreCardName6 });
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds6);

                        break;
                }


                
            }
        }

    }
}