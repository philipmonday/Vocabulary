using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

public partial class UnitWords : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            lblCurrentUnitID.Text = Session["SelectedScheduleUnitID"].ToString();
        }
    }

    protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (txtNewHint.Text != string.Empty)
        { Response.Write("<script>alert('Please save hint first!')</script>"); }
        
        //从数据库取值，将单词显示
        string connectionStr;
        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();

            string sql = "SELECT WordBody, Desc1,Desc2,Desc3,Desc4,Desc5,Desc6 FROM word Where  Id = " + ListBox1.SelectedItem.Value.ToString(); 
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                lblWordBody.Text = rdr[0].ToString();
                lblDesc1.Text = rdr[1].ToString();
                lblDesc2.Text = rdr[2].ToString();
                lblDesc3.Text = rdr[3].ToString();
                lblDesc4.Text = rdr[4].ToString();
                lblDesc5.Text = rdr[5].ToString();
                lblDesc6.Text = rdr[6].ToString();
            }
            rdr.Close();

            lblSynonyms.Text = string.Empty;
            string sqlSynonyms = "SELECT Synonyms FROM wordSynonyms Where wordId = " + ListBox1.SelectedItem.Value.ToString();
            MySqlCommand cmdSynonyms = new MySqlCommand(sqlSynonyms, conn);
            MySqlDataReader rdrSynonyms = cmdSynonyms.ExecuteReader();
            while (rdrSynonyms.Read())
            {
                lblSynonyms.Text = rdrSynonyms[0].ToString() + ", " + lblSynonyms.Text;
            }
            rdrSynonyms.Close();

            lblExample.Text = string.Empty;
            string sqlExample = "SELECT Example FROM wordExample Where wordId = " + ListBox1.SelectedItem.Value.ToString();
            MySqlCommand cmdExample = new MySqlCommand(sqlExample, conn);
            MySqlDataReader rdrExample = cmdExample.ExecuteReader();
            while (rdrExample.Read())
            {
                lblExample.Text = rdrExample[0].ToString() + "\r\n " + lblExample.Text;
            }
            rdrExample.Close();

            conn.Close();
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
        }

    }
  

protected void btnAddHint_Click(object sender, EventArgs e)
    {
        if (txtNewHint.Text.Length <= 0)
            return;


        string connectionStr;

        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try   
        {
            conn.Open();

            //xh 20210808 直接拼字符串的sql会容易被注入 string sql = "INSERT INTO wordhint (UserId, WordId, Hint) VALUES (1, "+ ListBox1.SelectedItem.Value.ToString() + ", '" + txtNewHint.Text + "') ";
            string sql = "INSERT INTO wordhint (UserId, WordId, Hint) VALUES (1, @wordid, @hint) ";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("wordid", ListBox1.SelectedItem.Value.ToString());
            cmd.Parameters.AddWithValue("hint", txtNewHint.Text);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
        }
        lvHints.DataBind();
        txtNewHint.Text = "";
    }

 

    protected void lvHints_ItemDeleting(object sender, ListViewDeleteEventArgs e)
    {
        string strHintId = "";
        Label lbl = (lvHints.Items[e.ItemIndex].FindControl("lblHintId")) as Label;
        if (lbl != null)
            strHintId = lbl.Text;

        //页面上对应的deletecommand其实并不起作用，但必须有。以这里的sql为准
        string DeleteQuery = "Delete from WordHint WHERE Id = '" + strHintId + "'";
        
        string connectionStr;

        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(DeleteQuery, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
        }
        lvHints.DataBind();
    }

    protected void btnMain_Click(object sender, EventArgs e)
    {
        Response.Redirect("UnitSchedule.aspx");
    }
}