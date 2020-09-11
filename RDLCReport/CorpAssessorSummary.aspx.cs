using Microsoft.Reporting.WebForms;
using RDLCReport.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RDLCReport
{
    public partial class CorpAssessorSummary : System.Web.UI.Page
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
                //var ehsAssessmentData = db.tb_project_ehs_assessment_score.Where(z => z.EhsAssesmentId == EhsAssesmentID);
                var EhsScore = db.tb_ehs_assessment_score.Where(z => z.EhsAssesmentId == EhsAssesmentID).Where(z => z.EhsAssesmentId == EhsAssesmentID).Select(x => new
                {
                    BusinessUnit = db.tb_business_unit.FirstOrDefault(y => y.BusinessUnitID == x.BusinessUnitID).BusinessUnitName.Trim(),
                    Region = db.tb_business_unit.FirstOrDefault(y => y.BusinessUnitID == x.BusinessUnitID).BusinessUnitName.Trim(),
                    Project = db.tb_business_unit.FirstOrDefault(y => y.BusinessUnitID == x.BusinessUnitID).BusinessUnitName.Trim(),
                    Location = x.Location.Trim(),
                    AssessmentDate = x.AssessmentDate,
                    AssessmentByName = db.tb_user_profile.FirstOrDefault(y => y.UserID == x.CorpEHSID).UserName.Trim(),
                    RegionManager = db.tb_user_profile.FirstOrDefault(y => y.UserID == x.RegionalManagerID).UserName.Trim(),
                    ProjectManager = db.tb_user_profile.FirstOrDefault(y => y.UserID == x.ProjectManagerID).UserName.Trim(),
                    PCH = db.tb_user_profile.FirstOrDefault(y => y.UserID == x.PCH).UserName.Trim(),
                    NoOfWorkMan = x.NoOfWorkMan,
                    ScoreCardId = x.ScorecardID
                }).FirstOrDefault();

                var EhsTransaction = db.tb_ehs_assessment_score_transaction.Where(x => x.EhsAssesmentID == EhsAssesmentID)
                                            .GroupBy(x => new
                                            {
                                                x.AreaID,
                                                x.tb_ehs_req_area.AreaName,
                                                x.tb_ehs_req_area.SrNo,
                                                x.tb_ehs_req_area.RequirementId,
                                                x.tb_ehs_req_area.TotalWeightagePerCriteria
                                            }).ToList()
                                            .Select(x => new
                                            {
                                                requirementid = x.Key.RequirementId,
                                                AreaId = x.Key.AreaID,
                                                AreaName = x.Key.AreaName.Trim(),
                                                SrNo = x.Key.SrNo,
                                                TotalWeightagePerCriteria = x.Key.TotalWeightagePerCriteria,
                                                ApplicableWeightage = x.Sum(total => total.ApplicableWeightage),
                                                Compliance = x.Sum(total => total.Accessor1Compliance),
                                                Remark = string.Join(",", x.Select(y => y.Accessor1Remark ?? string.Empty)).Trim(',')
                                            }).ToList();

                ReportDataSource rds = new ReportDataSource();
                DataSet ds = new DataSet();

                switch (EhsScore.ScoreCardId)
                {
                    case 1:
                        var EhsTransaction1 = EhsTransaction.FindAll(x => x.requirementid == 1);
                        var EhsTransaction2 = EhsTransaction.FindAll(x => x.requirementid == 2);
                        var EhsTransaction3 = EhsTransaction.FindAll(x => x.requirementid == 3);
                        var EhsTransaction4 = EhsTransaction.FindAll(x => x.requirementid == 4);

                        var EhsNegativeScore1 = db.tb_ehs_assessment_score_neg.Where(x => x.EhsAssesmentId == EhsAssesmentID).ToList();

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCard.rdlc");
                        var scorecardName1 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == EhsScore.ScoreCardId).ScoreCardName.Substring(21);
                        ReportParameter[] parameters1 = new ReportParameter[1];
                        parameters1[0] = new ReportParameter("ScoreCardName", scorecardName1);
                        ReportViewer1.LocalReport.SetParameters(parameters1);

                        ds.Tables.Add("Abc");
                        ds.Tables[0].Columns.Add("BusinessUnitID");
                        ds.Tables[0].Columns.Add("AssessmentDate");
                        ds.Tables[0].Columns.Add("AssessmentBy");
                        ds.Tables[0].Columns.Add("Location");
                        ds.Tables[0].Columns.Add("NoOfWorkMan");
                        ds.Tables[0].Columns.Add("ProjectManagerID");
                        ds.Tables[0].Columns.Add("RegionalManagerID");
                        ds.Tables[0].Columns.Add("PCH");
                        ds.Tables[0].Rows.Add(EhsScore.BusinessUnit, EhsScore.AssessmentDate?.ToString("dd/MM/yyyy"), EhsScore.AssessmentByName, EhsScore.Location, EhsScore.NoOfWorkMan
                            , EhsScore.ProjectManager, EhsScore.RegionManager, EhsScore.PCH);
                        rds = new ReportDataSource("SafteyApp", ds.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp1", EhsTransaction1));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp2", EhsTransaction2));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp3", EhsTransaction3));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp4", EhsTransaction4));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp5", EhsNegativeScore1));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp6", EhsTransaction));
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds);


                        break;
                    case 2:
                        var EhsTransaction5 = EhsTransaction.FindAll(x => x.requirementid == 5);
                        var EhsTransaction6 = EhsTransaction.FindAll(x => x.requirementid == 6);
                        var EhsTransaction7 = EhsTransaction.FindAll(x => x.requirementid == 7);
                        var EhsTransaction8 = EhsTransaction.FindAll(x => x.requirementid == 8);

                        var EhsNegativeScore2 = db.tb_ehs_assessment_score_neg.Where(x => x.EhsAssesmentId == EhsAssesmentID).ToList();

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCard.rdlc");
                        var scorecardName2 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == EhsScore.ScoreCardId).ScoreCardName.Substring(21);
                        ReportParameter[] parameters2 = new ReportParameter[1];
                        parameters2[0] = new ReportParameter("ScoreCardName", scorecardName2);
                        ReportViewer1.LocalReport.SetParameters(parameters2);


                        ds.Tables.Add("Abc");
                        ds.Tables[0].Columns.Add("BusinessUnitID");
                        ds.Tables[0].Columns.Add("AssessmentDate");
                        ds.Tables[0].Columns.Add("AssessmentBy");
                        ds.Tables[0].Columns.Add("Location");
                        ds.Tables[0].Columns.Add("NoOfWorkMan");
                        ds.Tables[0].Columns.Add("ProjectManagerID");
                        ds.Tables[0].Columns.Add("RegionalManagerID");
                        ds.Tables[0].Columns.Add("PCH");
                        ds.Tables[0].Rows.Add(EhsScore.BusinessUnit, EhsScore.AssessmentDate?.ToString("dd/MM/yyyy"), EhsScore.AssessmentByName, EhsScore.Location, EhsScore.NoOfWorkMan
                            , EhsScore.ProjectManager, EhsScore.RegionManager, EhsScore.PCH);
                        rds = new ReportDataSource("SafteyApp", ds.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp1", EhsTransaction5));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp2", EhsTransaction6));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp3", EhsTransaction7));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp4", EhsTransaction8));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp5", EhsNegativeScore2));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp6", EhsTransaction));
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds);

                        break;
                    case 3:
                        var EhsTransaction9 = EhsTransaction.FindAll(x => x.requirementid == 9);
                        var EhsTransaction10 = EhsTransaction.FindAll(x => x.requirementid == 10);
                        var EhsTransaction11 = EhsTransaction.FindAll(x => x.requirementid == 11);
                        var EhsTransaction12 = EhsTransaction.FindAll(x => x.requirementid == 12);

                        var EhsNegativeScore3 = db.tb_ehs_assessment_score_neg.Where(x => x.EhsAssesmentId == EhsAssesmentID).ToList();

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCard.rdlc");
                        var scorecardName3 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == EhsScore.ScoreCardId).ScoreCardName.Substring(21);
                        ReportParameter[] parameters3 = new ReportParameter[1];
                        parameters3[0] = new ReportParameter("ScoreCardName", scorecardName3);
                        ReportViewer1.LocalReport.SetParameters(parameters3);

                        ds.Tables.Add("Abc");
                        ds.Tables[0].Columns.Add("BusinessUnitID");
                        ds.Tables[0].Columns.Add("AssessmentDate");
                        ds.Tables[0].Columns.Add("AssessmentBy");
                        ds.Tables[0].Columns.Add("Location");
                        ds.Tables[0].Columns.Add("NoOfWorkMan");
                        ds.Tables[0].Columns.Add("ProjectManagerID");
                        ds.Tables[0].Columns.Add("RegionalManagerID");
                        ds.Tables[0].Columns.Add("PCH");
                        ds.Tables[0].Rows.Add(EhsScore.BusinessUnit, EhsScore.AssessmentDate?.ToString("dd/MM/yyyy"), EhsScore.AssessmentByName, EhsScore.Location, EhsScore.NoOfWorkMan
                            , EhsScore.ProjectManager, EhsScore.RegionManager, EhsScore.PCH);
                        rds = new ReportDataSource("SafteyApp", ds.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp1", EhsTransaction9));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp2", EhsTransaction10));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp3", EhsTransaction11));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp4", EhsTransaction12));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp5", EhsNegativeScore3));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp6", EhsTransaction));
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        break;
                    case 4:
                        var EhsTransaction13 = EhsTransaction.FindAll(x => x.requirementid == 13);
                        var EhsTransaction14 = EhsTransaction.FindAll(x => x.requirementid == 14);
                        var EhsTransaction15 = EhsTransaction.FindAll(x => x.requirementid == 15);
                        var EhsTransaction16 = EhsTransaction.FindAll(x => x.requirementid == 16);
                        var EhsTransaction17 = EhsTransaction.FindAll(x => x.requirementid == 17);

                        var EhsNegativeScore4 = db.tb_ehs_assessment_score_neg.Where(x => x.EhsAssesmentId == EhsAssesmentID).ToList();

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/mfgEHSScoreCard.rdlc");
                        var scorecardName4 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == EhsScore.ScoreCardId).ScoreCardName.Substring(21);
                        ReportParameter[] parameters4 = new ReportParameter[1];
                        parameters4[0] = new ReportParameter("ScoreCardName", scorecardName4);
                        ReportViewer1.LocalReport.SetParameters(parameters4);

                        ds.Tables.Add("Abc");
                        ds.Tables[0].Columns.Add("BusinessUnitID");
                        ds.Tables[0].Columns.Add("AssessmentDate");
                        ds.Tables[0].Columns.Add("AssessmentBy");
                        ds.Tables[0].Columns.Add("Location");
                        ds.Tables[0].Columns.Add("NoOfWorkMan");
                        ds.Tables[0].Columns.Add("ProjectManagerID");
                        ds.Tables[0].Columns.Add("RegionalManagerID");
                        ds.Tables[0].Columns.Add("PCH");
                        ds.Tables[0].Rows.Add(EhsScore.BusinessUnit, EhsScore.AssessmentDate?.ToString("dd/MM/yyyy"), EhsScore.AssessmentByName, EhsScore.Location, EhsScore.NoOfWorkMan
                            , EhsScore.ProjectManager, EhsScore.RegionManager, EhsScore.PCH);
                        rds = new ReportDataSource("SafteyApp", ds.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp1", EhsTransaction13));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp2", EhsTransaction14));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp3", EhsTransaction15));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp4", EhsTransaction16));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp7", EhsTransaction17));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp5", EhsNegativeScore4));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp6", EhsTransaction));
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds);

                        break;//so on
                    case 5:
                        var EhsTransaction18 = EhsTransaction.FindAll(x => x.requirementid == 18);
                        var EhsTransaction19 = EhsTransaction.FindAll(x => x.requirementid == 19);
                        var EhsTransaction20 = EhsTransaction.FindAll(x => x.requirementid == 20);
                        var EhsTransaction21 = EhsTransaction.FindAll(x => x.requirementid == 21);

                        var EhsNegativeScore5 = db.tb_ehs_assessment_score_neg.Where(x => x.EhsAssesmentId == EhsAssesmentID).ToList();

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCard.rdlc");
                        var scorecardName5 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == EhsScore.ScoreCardId).ScoreCardName.Substring(21);
                        ReportParameter[] parameters5 = new ReportParameter[1];
                        parameters5[0] = new ReportParameter("ScoreCardName", scorecardName5);
                        ReportViewer1.LocalReport.SetParameters(parameters5);

                        ds.Tables.Add("Abc");
                        ds.Tables[0].Columns.Add("BusinessUnitID");
                        ds.Tables[0].Columns.Add("AssessmentDate");
                        ds.Tables[0].Columns.Add("AssessmentBy");
                        ds.Tables[0].Columns.Add("Location");
                        ds.Tables[0].Columns.Add("NoOfWorkMan");
                        ds.Tables[0].Columns.Add("ProjectManagerID");
                        ds.Tables[0].Columns.Add("RegionalManagerID");
                        ds.Tables[0].Columns.Add("PCH");
                        ds.Tables[0].Rows.Add(EhsScore.BusinessUnit, EhsScore.AssessmentDate?.ToString("dd/MM/yyyy"), EhsScore.AssessmentByName, EhsScore.Location, EhsScore.NoOfWorkMan
                            , EhsScore.ProjectManager, EhsScore.RegionManager, EhsScore.PCH);
                        rds = new ReportDataSource("SafteyApp", ds.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp1", EhsTransaction18));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp2", EhsTransaction19));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp3", EhsTransaction20));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp4", EhsTransaction21));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp5", EhsNegativeScore5));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp6", EhsTransaction));
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        break;
                    case 6:
                        var EhsTransaction22 = EhsTransaction.FindAll(x => x.requirementid == 22);
                        var EhsTransaction23 = EhsTransaction.FindAll(x => x.requirementid == 23);
                        var EhsTransaction24 = EhsTransaction.FindAll(x => x.requirementid == 24);
                        var EhsTransaction25 = EhsTransaction.FindAll(x => x.requirementid == 25);

                        var EhsNegativeScore6 = db.tb_ehs_assessment_score_neg.Where(x => x.EhsAssesmentId == EhsAssesmentID).ToList();

                        ReportViewer1.Visible = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/EHSScoreCard.rdlc");
                        var scorecardName6 = db.tb_ehs_scorecard_master.FirstOrDefault(x => x.ScoreCardID == EhsScore.ScoreCardId).ScoreCardName.Substring(21);
                        ReportParameter[] parameters6 = new ReportParameter[1];
                        parameters6[0] = new ReportParameter("ScoreCardName", scorecardName6);
                        ReportViewer1.LocalReport.SetParameters(parameters6);

                        ds.Tables.Add("Abc");
                        ds.Tables[0].Columns.Add("BusinessUnitID");
                        ds.Tables[0].Columns.Add("AssessmentDate");
                        ds.Tables[0].Columns.Add("AssessmentBy");
                        ds.Tables[0].Columns.Add("Location");
                        ds.Tables[0].Columns.Add("NoOfWorkMan");
                        ds.Tables[0].Columns.Add("ProjectManagerID");
                        ds.Tables[0].Columns.Add("RegionalManagerID");
                        ds.Tables[0].Columns.Add("PCH");
                        ds.Tables[0].Rows.Add(EhsScore.BusinessUnit, EhsScore.AssessmentDate?.ToString("dd/MM/yyyy"), EhsScore.AssessmentByName, EhsScore.Location, EhsScore.NoOfWorkMan
                            , EhsScore.ProjectManager, EhsScore.RegionManager, EhsScore.PCH);
                        rds = new ReportDataSource("SafteyApp", ds.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp1", EhsTransaction22));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp2", EhsTransaction23));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp3", EhsTransaction24));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp4", EhsTransaction25));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp5", EhsNegativeScore6));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SafteyApp6", EhsTransaction));
                        ReportViewer1.Width = 1000;
                        ReportViewer1.Height = 800;
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        break;
                }
            }
        }
    }
}