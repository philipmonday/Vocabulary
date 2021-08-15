using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Globalization;


public class HttpCommon
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
    public partial class WordTesting : Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        string url = "https://api.dictionaryapi.dev/api/v2/entries/en/hello";
        //string url = "https://dictionaryapi.dev/";
        //MessageBox.Show(HttpCommon.HttpGet(url));
        Response.Write(HttpCommon.HttpGet(url));                        
        

        /*
        int testingWordIndex = 0;

        testingWordIndex = GetTestingWordIndex();

        int PerUnitWordsAmount = Convert.ToInt32(ConfigurationManager.AppSettings["PerUnitWordsAmount"]);
        int WordsAttributes = Convert.ToInt32(ConfigurationManager.AppSettings["WordsAttributes"]);
        //创建表格，放上所有单词位置控件
        for (int i = 0; i < PerUnitWordsAmount; i++)
        {
            TableRow row = new TableRow();
            for (int j = 0; j < WordsAttributes; j++)
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

            //把这行放入表中
            HolderTable.Rows.Add(row);
        }


        //从数据库取值，将单词显示出来
        string connectionStr;
        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();

#if DEBUG
            string sql = "SELECT * FROM word Where  UnitId = 1";
#else
            string sql = "SELECT * FROM word Where  UnitId = " + Session["SelectedScheduleUnitID"].ToString();

#endif
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            int rowIndex = 0; int colIndex = 0;
            while (rdr.Read())
            {
                for (colIndex = 0; colIndex < WordsAttributes; colIndex++)
                {
                    ((Label)HolderTable.Rows[rowIndex].Cells[colIndex].Controls[0]).Text = (rdr.IsDBNull(colIndex) ? "" : rdr[colIndex].ToString());

                }
                rowIndex++;
            }
            rdr.Close();
            conn.Close();
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
        }

        BuildInspectingTable();

        //Session.Remove("SelectedScheduleID");
        if (!Page.IsPostBack)
        {
            ShowWordAndOptions(testingWordIndex);
//
        }*/
    }

    protected void BuildInspectingTable()
    {
        int inspectingLastNTimes = Convert.ToInt32(ConfigurationManager.AppSettings["InspectingLastNTimes"]);
        int timesInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["TimesInEveryLoop"]);
        //放上单词位置控件
        TableRow rowLoopWordBody = new TableRow();
        for (int j = 0; j < timesInEveryLoop; j++)
        {
            TableCell cell = new TableCell();
            cell.BorderStyle = BorderStyle.Solid;
            cell.BorderColor = System.Drawing.Color.Gray;
            cell.BorderWidth = 2;
            cell.Width = 100;
            cell.Height = 25;
            Label li = new Label();
            li.ID = "lblLoopWordBody" +  j.ToString();
            li.Text = GetWordBody(j);
            cell.Controls.Add(li);
            rowLoopWordBody.Cells.Add(cell);
        }
        InspectingTable.Rows.Add(rowLoopWordBody);

        //放上反映 答对次数/监控最后n次  的控件
        TableRow rowInspecting = new TableRow();
        for (int j = 0; j < timesInEveryLoop; j++)
        {
            TableCell cell = new TableCell();
            cell.BorderStyle = BorderStyle.Solid;
            cell.BorderColor = System.Drawing.Color.Gray;
            cell.BorderWidth = 2;
            cell.Width = 100;
            cell.Height = 25;
            Label li = new Label();
            li.ID = "lblInspecting" + j.ToString();
            li.Text = GetInspectingStr(j);
            cell.Controls.Add(li);
            rowInspecting.Cells.Add(cell);
        }

        //把这行放入表中
        InspectingTable.Rows.Add(rowInspecting);

    }
    protected int GetTestingWordIndex()
    {
        int tempIndex;
        if (Session["TestingWordIndex"] == null)
        {
            tempIndex = 0;
            Session["TestingWordIndex"] = tempIndex;
        }
        else
        {
            tempIndex = (int)Session["TestingWordIndex"];
        }
        return tempIndex;
    }
    protected string GetInspectingStr(int indexInLoop)
    {
        int correctTimes;

        int rowId = GetRowIdByCurrentLoopIndex(indexInLoop);
        if (Session["Inspecting_" + rowId.ToString()] == null)
        {
            correctTimes = 0;
            Session["Inspecting_" + rowId.ToString()] = correctTimes;
        }
        else
        {
            correctTimes = (int)Session["Inspecting_" + rowId.ToString()];
        }

        return correctTimes.ToString() +"/"+ ConfigurationManager.AppSettings["InspectingLastNTimes"].ToString();
    }

 
    protected void IncreaseInspectingCorrectTime()
    {
        int rowId = GetTestingWordIndex();
        int correctTimes = (int)Session["Inspecting_" + rowId.ToString()];
        correctTimes++;
        Session["Inspecting_" + rowId.ToString()] = correctTimes;
        int timesInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["TimesInEveryLoop"]);
        int indexInLoop = rowId % timesInEveryLoop;
        ((Label)InspectingTable.Rows[1].Cells[indexInLoop].Controls[0]).Text = correctTimes.ToString() + "/" + ConfigurationManager.AppSettings["InspectingLastNTimes"].ToString();
    }
    protected void DecreaseInspectingCorrectTime()
    {
        int rowId = GetTestingWordIndex();
        int correctTimes = (int)Session["Inspecting_" + rowId.ToString()];
        correctTimes--;
        if ( correctTimes < 0) { correctTimes = 0; }
        Session["Inspecting_" + rowId.ToString()] = correctTimes;
        int timesInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["TimesInEveryLoop"]);
        int indexInLoop = rowId % timesInEveryLoop;
        ((Label)InspectingTable.Rows[1].Cells[indexInLoop].Controls[0]).Text = correctTimes.ToString() + "/" + ConfigurationManager.AppSettings["InspectingLastNTimes"].ToString();

    }
    protected int GetLoopCount()
    {
        int tempCount;
        if (Session["CurrentLoopCount"] == null)
        {
            tempCount = 0;
            Session["CurrentLoopCount"] = tempCount;
        }
        else
        {
            tempCount = (int)Session["CurrentLoopCount"];
        }
        return tempCount;
    }
    protected void SetLoopCount(int count)
    {
        Session["CurrentLoopCount"] = count;
    }
    protected void IncreaseLoopCount()
    {
        int tempCount;
        tempCount = GetLoopCount();
        tempCount++;
        SetLoopCount(tempCount);
    }

    protected void ShowWordAndOptions(int rowId)
    {
        if (rowId >= Convert.ToInt32(ConfigurationManager.AppSettings["PerUnitWordsAmount"]))
        {
            return;
        }
        lblWordBody.Text = ((Label)HolderTable.Rows[rowId].Cells[2].Controls[0]).Text;
        string[] strarrOptions = GetOptions(rowId);
        rblOptions.Items.Clear();
        for (int i = 0; i < 4; i++)
        {
            ListItem item = new ListItem();
            item.Value = i.ToString();
            item.Text = strarrOptions[i];
            rblOptions.Items.Add(item);
        }
        lblCorrectOptionIndex.Text = strarrOptions[4];
        lblSubmitResult.Text = string.Empty;
    }

    //根据rowId，随机取出对应单词的释义
    protected string GetWordRdnDesc(int rowId)
    {
        string strOption = string.Empty;
        if (rowId >= Convert.ToInt32(ConfigurationManager.AppSettings["PerUnitWordsAmount"]))
        {
            return string.Empty;
        }

        Random rnd = new Random();
        int rndKey, tryTimes=0;
        while (strOption == string.Empty && tryTimes <20)
        {
            rndKey = rnd.Next(3, 9);
            tryTimes++;
            strOption = ((Label)HolderTable.Rows[rowId].Cells[rndKey].Controls[0]).Text;
        }
        if (tryTimes >= 20)
        {
            //如果一直取不出合适的option，就默认用desc1的内容
            strOption = ((Label)HolderTable.Rows[rowId].Cells[3].Controls[0]).Text;
        }

        return strOption;
    }
    protected string GetWordAllDesc(int rowId)
    {
        string strOption = string.Empty;
        if (rowId >= Convert.ToInt32(ConfigurationManager.AppSettings["PerUnitWordsAmount"]))
        {
            return string.Empty;
        }
        for (int i = 3; i < 9; i++)
        {
            string strTemp = ((Label)HolderTable.Rows[rowId].Cells[i].Controls[0]).Text;
            if (strTemp != string.Empty)
            {
                strOption = strOption + " / " + strTemp;
            }
        }

        return strOption;
    }
    protected string GetWordBody(int rowId)
    {
        return ((Label)HolderTable.Rows[rowId].Cells[2].Controls[0]).Text;
    }
    private string[] GetOptions(int rowId)
    {
        //返回的数组中，前4个为选项
        //第5个为正确答案索引
        string[] straryOptions = new string[5];
        bool blRowIdFound = false;
        int intRowIdIndex = 0;

        int PerUnitWordsAmount = Convert.ToInt32(ConfigurationManager.AppSettings["PerUnitWordsAmount"]);
        int[] array1 = UseDoubleArrayToNonRepeatedRandom(PerUnitWordsAmount - 1);
        for (int i = 0; i <= 3; i++)
        {
            if (array1[i] == rowId) //说明随机选的4个词中正好有待测的词，
            {
                blRowIdFound = true;
                intRowIdIndex = i;
            }
        }

        if (!blRowIdFound)
        {

            //随机指定一个索引，替换成答案id
            Random rnd = new Random();
            int rndKey = rnd.Next(4);
            array1[rndKey] = rowId;
            intRowIdIndex = rndKey;
        }
        
        for (int i = 0; i <= 3; i++)
        {
            straryOptions[i] = GetWordAllDesc(array1[i]);//GetWordRdnDesc(array1[i]);
        }
        straryOptions[4] = intRowIdIndex.ToString();

        return straryOptions;
    }

    static int[] UseDoubleArrayToNonRepeatedRandom(int length)
    {
        int seed = Guid.NewGuid().GetHashCode();
        Random radom = new Random(seed);
        int[] index = new int[length];
        for (int i = 0; i < length; i++)
        {
            index[i] = i + 1;
        }

        int[] array = new int[length]; // 用来保存随机生成的不重复的数 
        int site = length;             // 设置上限 
        int idx;                       // 获取index数组中索引为idx位置的数据，赋给结果数组array的j索引位置
        for (int j = 0; j < length; j++)
        {
            idx = radom.Next(0, site - 1);  // 生成随机索引数
            array[j] = index[idx];          // 在随机索引位置取出一个数，保存到结果数组 
            index[idx] = index[site - 1];   // 作废当前索引位置数据，并用数组的最后一个数据代替之
            site--;                         // 索引位置的上限减一（弃置最后一个数据）
        }
        return array;
    }



    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (rblOptions.SelectedItem == null)
        {
            lblSubmitResult.Text = "PLEASE CHOOSE ONE OPTION!";
            return;
        }
        int iSelectedValue = Convert.ToInt32(rblOptions.SelectedValue);
        int iCorrectOptionIndex = Convert.ToInt32(lblCorrectOptionIndex.Text);
        if (iSelectedValue == iCorrectOptionIndex)
        {
            lblSubmitResult.ForeColor = System.Drawing.Color.GreenYellow;
            lblSubmitResult.Text = "CORRECT!";
            IncreaseInspectingCorrectTime();
        }
        else
        {
            lblSubmitResult.ForeColor = System.Drawing.Color.Red;
            lblSubmitResult.Text = "WRONG! Anwser is " + Convert.ToString(iCorrectOptionIndex+1) +".";
            DecreaseInspectingCorrectTime();
        }
        btnSubmit.Enabled = false;
        btnNext.Enabled = true;
      }

    protected void WalkToNextTestingWordIndex()
    {
        int testingWordIndex = GetTestingWordIndex();
        testingWordIndex++;

        if (testingWordIndex >= GetCurrentLoopUpperIndex())
        {
            testingWordIndex = GetCurrentLoopLowerIndex();
        }
        Session["TestingWordIndex"] = testingWordIndex;
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        WalkToNextTestingWordIndex();

        ShowWordAndOptions(GetTestingWordIndex());
        btnSubmit.Enabled = true;
        btnNext.Enabled = false;
    }

    protected int GetCurrentLoopUpperIndex()
    {
        int TimesInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["TimesInEveryLoop"]);
        int loopCount = GetLoopCount();
        return loopCount * TimesInEveryLoop + TimesInEveryLoop;
    }
    protected int GetCurrentLoopLowerIndex()
    {
        int TimesInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["TimesInEveryLoop"]);
        int loopCount = GetLoopCount();
        return loopCount * TimesInEveryLoop;
    }

    protected int GetRowIdByCurrentLoopIndex(int indexInLoop)
    {
        int TimesInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["TimesInEveryLoop"]);
        int loopCount = GetLoopCount();
        return loopCount * TimesInEveryLoop + indexInLoop;
    }
    /// <summary>
    /// 发起GET同步请求
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="headers">请求头</param>
    /// <param name="timeOut">超时时间</param>
    /// <returns></returns>
    public static string HttpGet(string url, Dictionary<string, string> headers = null, int timeOut = 30)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = new TimeSpan(0, 0, timeOut);
            if (headers != null)
            {
                foreach (var header in headers)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            HttpResponseMessage response = client.GetAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }

    protected void btnGetWebData_Click(object sender, EventArgs e)
    {
        string url = "https://api.dictionaryapi.dev/api/v2/entries/en/hello";
        string response = HttpGet(url);
        lblWordBody.Text = response;
  
    }
}
