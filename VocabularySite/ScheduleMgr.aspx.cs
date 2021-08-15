using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using MySql.Data.MySqlClient;

public partial class ScheduleMgr : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            int currentUnit = 0;
            if (Session["SelectedScheduleUnitID"] != null)
            {
                currentUnit = Convert.ToInt32(Session["SelectedScheduleUnitID"]);
            }

            lblUnitID.Text = currentUnit.ToString();
        }
    }

    protected void lvSchedule_ItemDeleting(object sender, ListViewDeleteEventArgs e)
    {
        string strScheduleId = "";
        Label lbl = (lvSchedule.Items[e.ItemIndex].FindControl("lblScheduleId")) as Label;
        if (lbl != null)
            strScheduleId = lbl.Text;

        string DeleteWordStudyHistory = "Delete from WordStudyHistory where UnitScheduleID = '" + strScheduleId + "'";
        string DeleteQuery = "Delete from UnitStudySchedule WHERE Id = '" + strScheduleId + "'";
        string connectionStr;

        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
            MySqlCommand cmd1 = new MySqlCommand(DeleteWordStudyHistory, conn);
            cmd1.ExecuteNonQuery();
            MySqlCommand cmd = new MySqlCommand(DeleteQuery, conn);
            cmd.ExecuteNonQuery();
            //conn.Close();
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
        }
        finally
        {
            conn.Close();
        }
        lvSchedule.DataBind();
    }

    protected void btnAddSchedule_Click(object sender, EventArgs e)
    {

        if (txtScheduleDate.Text.Length <= 0)
        { 
            Response.Write("<script>alert('Please Input the Schedule Date!');</script>");
            return;
        }

        string connectionStr;

        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
            //Id, UnitId, ScheduleDate, ScheduleTitle, TimeUsed, Result
            string sql = "INSERT INTO UnitStudySchedule (UserId, UnitId, ScheduleDate, ScheduleTitle, TimeUsed, Result)"+
                                              "VALUES (1, @UnitId, @ScheduleDate, @ScheduleTitle, @TimeUsed, @Result)";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("UnitId", lblUnitID.Text);
            cmd.Parameters.AddWithValue("ScheduleDate", txtScheduleDate.Text);
            cmd.Parameters.AddWithValue("ScheduleTitle", txtScheduleTitle.Text);
            cmd.Parameters.AddWithValue("TimeUsed", "0");
            cmd.Parameters.AddWithValue("Result", "0");

            cmd.ExecuteNonQuery();
            //conn.Close();
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
        }
        finally
        {
            conn.Close();
        }
        lvSchedule.DataBind();
    }

    protected void btnOnClick(object sender, EventArgs e)
    {
        LayerCC.Visible = !LayerCC.Visible;
    }
    protected void Calendar1_SelectionChanged(object sender, EventArgs e)
    {
        txtScheduleDate.Text = Calendar1.SelectedDate.ToString("yyyy-MM-dd");
        LayerCC.Visible = false;
    }

    protected void btnMain_Click(object sender, EventArgs e)
    {
        Response.Redirect("UnitSchedule.aspx");
    }

    protected void lvSchedule_DataBound(object sender, EventArgs e)
    {
        int count = lvSchedule.Items.Count;
        DateTime dtNextDate = DateTime.Today;

        string strReviewFrequency = ConfigurationManager.AppSettings["ReviewFrequency"];
        List<string> listReviewFrequency = new List<string>();
        listReviewFrequency = strReviewFrequency.Split(',').ToList();
        if (count > 0)
        {
            string lastdate = ((Label)lvSchedule.Items[count - 1].FindControl("lblScheduleDate")).Text;
            DateTime dtLastdate = Convert.ToDateTime(lastdate);
            if (count >= listReviewFrequency.Count)
            {
                dtNextDate = dtLastdate.AddDays(Convert.ToInt32(listReviewFrequency.Last()));
            }
            else
            {
                string strAddDate = listReviewFrequency[count-1];
                int addDates = Convert.ToInt32(strAddDate);
                dtNextDate = dtLastdate.AddDays(addDates);
            }
            
            txtScheduleDate.Text = dtNextDate.ToString();
        }
        lblReviewFrequency.Text = strReviewFrequency;

        count++;
        txtScheduleTitle.Text = lblUnitID.Text + "-" + count.ToString();


    }
}