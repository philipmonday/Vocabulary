using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using MySql.Data.MySqlClient;

public partial class UnitSchedule : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Calendar1.SelectedDate = DateTime.Today;
            SqlDataSourceDatePick.DataBind();
            SqlDataSourceDatePick2.DataBind();
        }
    }
    
    protected void Calendar1_SelectionChanged(object sender, EventArgs e)
    {
        //lblday.Text = Calendar1.TodaysDate.ToShortDateString();
        //lblday.Text = Calendar1.SelectedDate.ToShortDateString();
        SqlDataSourceDatePick.DataBind();
        SqlDataSourceDatePick2.DataBind();
    }
    protected void ListBoxSelectedIndexChanged(object sender, EventArgs e)
    {
        lblSelectedScheduleUnitID.Text = ((ListBox)sender).SelectedItem.Text.ToString();
        lblSelectedScheduleID.Text = ((ListBox)sender).SelectedItem.Value.ToString();


        //创建4行5列的表格，放上label控件
        for (int i = 0; i < 4; i++)
        {
            TableRow row = new TableRow();
            for (int j = 0; j < 5; j++)
            {
                TableCell cell = new TableCell();
                cell.BorderStyle = BorderStyle.Solid;
                cell.BorderColor = System.Drawing.Color.Gray;
                cell.BorderWidth = 2;
                cell.Width = 100;
                cell.Height = 25;
                Label li = new Label();
                li.ID = "lblTest" + i.ToString() + j.ToString();
                li.Text = "";
                cell.Controls.Add(li);
                row.Cells.Add(cell);
            }
            /*//每行末尾放一个按钮
            TableCell btCell = new TableCell();
            btCell.BorderStyle = BorderStyle.Solid;
            btCell.BorderColor = System.Drawing.Color.Gray;
            btCell.BorderWidth = 2;
            btCell.Width = 50;
            Button bt = new Button();
            bt.ID = "btTest" + i.ToString();
            bt.Text = "测试";
            btCell.Controls.Add(bt);
            row.Cells.Add(btCell);*/

            //把这行放入表中
            HolderTable.Rows.Add(row);
        }


        //从数据库取值，将单词显示在表格内
        string connectionStr;
        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();

            string sql = "SELECT WordBody FROM word Where  UnitId = " + lblSelectedScheduleUnitID.Text;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            int rowIndex = 0; int colIndex = 0;
            while (rdr.Read())
            {
                ((Label)HolderTable.Rows[rowIndex].Cells[colIndex].Controls[0]).Text = rdr[0].ToString();
                colIndex++;
                if (colIndex >= 5)
                {
                    rowIndex++;
                    colIndex = 0;
                }
            }
            rdr.Close();
            conn.Close();
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
        }

    }
    protected void lbTodo_SelectedIndexChanged(object sender, EventArgs e)
    {
        ListBoxSelectedIndexChanged(sender, e); 
    }
    protected void lbDone_SelectedIndexChanged(object sender, EventArgs e)
    {
        ListBoxSelectedIndexChanged(sender, e);
    }


    protected void btnViewWords_Click(object sender, EventArgs e)
    {
        Session["SelectedScheduleUnitID"] = lblSelectedScheduleUnitID.Text;
        Response.Redirect("UnitWords.aspx");
    }

    protected void btnTraining_Click(object sender, EventArgs e)
    {
        Session["SelectedScheduleUnitID"] = lblSelectedScheduleUnitID.Text;
        Session["SelectedScheduleID"] = lblSelectedScheduleID.Text;
        Response.Redirect("WordTraining.aspx");
    }

    
    protected void btnTesting_Click(object sender, EventArgs e)
    {
        /*Response.Write("<script>alert('Testing!')</script>");
        btnTesting.Enabled = false;
        */
        //Session["SelectedScheduleUnitID"] = lblSelectedScheduleUnitID.Text;
        //Session["TestingWordIndex"] = 0;
        Response.Redirect("WordTesting.aspx");
        
    }
    /*
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        Label1.Text = DateTime.Now.ToString();
    }*/

    protected void btnScheduleMgr_Click(object sender, EventArgs e)
    {
        Session["SelectedScheduleUnitID"] = lblSelectedScheduleUnitID.Text;
        Response.Redirect("ScheduleMgr.aspx");
    }


}