using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using api.dictionaryapi.dev;

public partial class ToolFetchWordDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnToList_Click(object sender, EventArgs e)
    {
        string txtWords = tbWords.Text;
        if (txtWords.Length == 0)
        {
            Response.Write("<script>alert('Please input word list!');</script>");
            return;
        }

        List<string> listWords = new List<string>();
        listWords = txtWords.Split(Environment.NewLine.ToCharArray()).ToList();
        lbWordList.Items.Clear();
        for (int i = 0; i<listWords.Count; i++)
        {
            if (listWords[i].Length >0)
            {
                ListItem li = new ListItem();
                li.Text = listWords[i];
                li.Value = i.ToString();
                lbWordList.Items.Add(li);
            }
          }
        Panel1.Visible = false;
        Panel2.Visible = true;
        Panel3.Visible = true;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Panel1.Visible = true;
        Panel2.Visible = false;
        Panel3.Visible = false;
    }

    protected void btnFetch_Click(object sender, EventArgs e)
    {
        string word = lbWordList.SelectedItem.Text;
        string url = "https://api.dictionaryapi.dev/api/v2/entries/en/"+ word;
        //string url = "https://dictionaryapi.dev/";
        string temp = MyHttpTool.HttpGet(url);
        txtWordDetails.Text = temp;// temp.Substring(1, temp.Length - 2);
    }

    protected void btnInsertDB_Click(object sender, EventArgs e)
    {
        if(txtUnit.Text == string.Empty)
        {
            lblParseResult.Text = "Please Input unitID";
            return;
        }

        List<api.dictionaryapi.dev.Root> myRoot;
        api.dictionaryapi.dev.Root RootFirst;
        try
        {
            myRoot = JsonConvert.DeserializeObject<List<api.dictionaryapi.dev.Root>>(txtWordDetails.Text);
            RootFirst = myRoot[0];
            lblParseResult.Text = RootFirst.word + " Parsing Start...";
        }
        catch (Exception ex)
        {
            lblParseResult.Text = ex.Message;
            return;
        }
           

        string connectionStr;

        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
            string sqlRowCount = "SELECT count(id) FROM word ";
            int iRowCount = 0;
            MySqlCommand cmdRowCount = new MySqlCommand(sqlRowCount, conn);
            MySqlDataReader rdr = cmdRowCount.ExecuteReader();
            while (rdr.Read())
            {
                iRowCount = Int32.Parse(rdr.IsDBNull(0) ? "0" : rdr[0].ToString());
            }
            rdr.Close();
            iRowCount++;

            //Id, UnitId, ScheduleDate, ScheduleTitle, TimeUsed, Result
            string sql = "INSERT INTO Word (Id, UnitId, WordBody, Desc1, Desc2, Desc3, Desc4, Desc5, Desc6)" +
                                 "VALUES (@Id, @UnitId, @WordBody, @Desc1, @Desc2, @Desc3, @Desc4, @Desc5,@Desc6)";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("Id", iRowCount.ToString());
            cmd.Parameters.AddWithValue("UnitId", txtUnit.Text);
            cmd.Parameters.AddWithValue("WordBody", RootFirst.word);
            int DescIndex = 0;
            foreach (api.dictionaryapi.dev.Root r in myRoot)
            {
                foreach (api.dictionaryapi.dev.Meaning m in r.meanings)
                {
                    foreach (api.dictionaryapi.dev.Definition d in m.definitions)
                    {
                        DescIndex++;
                        if (DescIndex <= 6)
                        {
                            string partOfSpeech = m.partOfSpeech;
                            string tempDesc = d.definition;
                            cmd.Parameters.AddWithValue("Desc" + DescIndex.ToString(), partOfSpeech + " " + tempDesc);
                        }
                    }
                }
            }

            for (DescIndex++; DescIndex <= 6; DescIndex++)
            {
                    cmd.Parameters.AddWithValue("Desc" + DescIndex.ToString(), "");
            }

            cmd.ExecuteNonQuery();

            //insert new Synonyms
            string sqlSynonyms = "INSERT INTO WordSynonyms (WordId, Synonyms) VALUES (@wordId, @Synonyms)";
            foreach (api.dictionaryapi.dev.Root r in myRoot)
            {
                foreach (api.dictionaryapi.dev.Meaning m in r.meanings)
                {
                    foreach (api.dictionaryapi.dev.Definition d in m.definitions)
                    {
                        foreach (string strSynonyms in d.synonyms)
                        {
                            MySqlCommand cmdSynonyms = new MySqlCommand(sqlSynonyms, conn);
                            cmdSynonyms.Parameters.AddWithValue("wordId", iRowCount.ToString());
                            cmdSynonyms.Parameters.AddWithValue("Synonyms", strSynonyms);
                            cmdSynonyms.ExecuteNonQuery();
                         }
                    }
                }
            }

            //insert new Example
            string sqlExample = "INSERT INTO WordExample (WordId, Example) VALUES (@wordId, @Example)";
            foreach (api.dictionaryapi.dev.Root r in myRoot)
            {
                foreach (api.dictionaryapi.dev.Meaning m in r.meanings)
                {
                    foreach (api.dictionaryapi.dev.Definition d in m.definitions)
                    {
                        if (d.example != null)
                        {
                            MySqlCommand cmdExample = new MySqlCommand(sqlExample, conn);
                            cmdExample.Parameters.AddWithValue("wordId", iRowCount.ToString());
                            cmdExample.Parameters.AddWithValue("Example", d.example);
                            cmdExample.ExecuteNonQuery();
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
        }
        finally
        {
            conn.Close();
        }
        lblParseResult.Text = RootFirst.word + " Parsing Finished.";
    }

    protected void lbWordList_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblWordBody.Text = lbWordList.SelectedItem.Text;
        txtWordDetails.Text = "";
    }
}



public class MyHttpTool
{
    /// <summary>
    /// Http同步Get同步请求
    /// </summary>
    /// <param name="url">Url地址</param>
    /// <param name="encode">编码(默认UTF8)</param>
    /// <returns></returns>
    public static string HttpGet(string url, Encoding encode = null)
    {
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        string result;

        try
        {
            var webClient = new WebClient { Encoding = Encoding.UTF8 };

            if (encode != null)
                webClient.Encoding = encode;

            result = webClient.DownloadString(url);
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        return result;
    }
}

