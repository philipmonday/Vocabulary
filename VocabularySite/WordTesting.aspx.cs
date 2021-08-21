using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Collections;

public partial class WordTesting : System.Web.UI.Page
{
    protected enum enum_TraningResult { Wrong, Correct, WithHint, QuickCorrect };
    protected enum enum_PageMode { WordDesc, CompleteSentence, Synonyms};


    protected enum_PageMode GetCurrentPageMode()
    {
        enum_PageMode tempMode = enum_PageMode.WordDesc;
        if (Session["CurrentPageMode"] == null)
        {
            tempMode = enum_PageMode.WordDesc;
            Session["CurrentPageMode"] = 0;
        }
        else if(Convert.ToInt32(Session["CurrentPageMode"]) == 0)
        {
            tempMode = enum_PageMode.WordDesc;
        }
        else if (Convert.ToInt32(Session["CurrentPageMode"]) == 1)
        {
            tempMode = enum_PageMode.CompleteSentence;
        }
        else if (Convert.ToInt32(Session["CurrentPageMode"]) == 2)
        {
            tempMode = enum_PageMode.Synonyms;
        }
        return tempMode;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
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


            string sql = "SELECT * FROM word Where  UnitId = " +
                Session["SelectedScheduleUnitID"].ToString();

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


        if (!Page.IsPostBack)
        {
            //试图从history中找到上次的状态
            int tempCurrentLoopCount = RestoreCurrentLoopCount(Session["SelectedScheduleID"].ToString());
            SetLoopCount(tempCurrentLoopCount);
            SetTestingWordIndex(4);
            if (WalkToNextTestingWordIndex())
            {
                NextProcess();
            }
        }
    }

    protected int RestoreCurrentLoopCount(string scheduleId)
    {
        int wordsTrained = 0;
        int wordsInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["WordsInEveryLoop"]);
        string connectionStr;
        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
            string sql = "SELECT count(Id) FROM WordStudyHistory WHERE UnitScheduleId =" + scheduleId;

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                wordsTrained = Int32.Parse(rdr[0].ToString());
            }
            rdr.Close();
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
        //如果训练过了5个词，不减掉1，就会到下一个loop去。所以减掉1
        if (wordsTrained > 0)
        {
            wordsTrained--;
        }
        Decimal re = wordsTrained / wordsInEveryLoop;
        int loopCount = Convert.ToInt32(Math.Floor(re));
        return loopCount;
    }

    protected void ShowAllAboutWord(int rowId)
    {
        ShowQuizAndOptions(rowId);
        ShowTrainingResult(rowId);
        Session["LoggedInTime"] = DateTime.Now;

    }

    protected int GetTimeUsedBefore()
    {
        int TimeUsedBefore = 0;
        string connectionStr;
        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();

            string sql = "SELECT timeused FROM unitstudyschedule WHERE id =" + Session["SelectedScheduleID"].ToString();

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                TimeUsedBefore = Int32.Parse(rdr[0].ToString());
            }
            rdr.Close();
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
        return TimeUsedBefore;
    }

    protected void ShowTrainingResult(int rowId)
    {
        if (rowId >= Convert.ToInt32(ConfigurationManager.AppSettings["PerUnitWordsAmount"]))
        {
            return;
        }

        string wordId = GetWordId(rowId);
        lblWordId.Text = wordId;

        string strTrainingResult = "";
        strTrainingResult = GetTrainingResultStr(Session["SelectedScheduleID"].ToString(), wordId);
        lblTrainingResult.Text = strTrainingResult;
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

    protected void SetTestingWordIndex(int idx)
    {
        Session["TestingWordIndex"] = idx;
    }


    protected void UpdateTrainingResult(enum_TraningResult result)
    {

        string strCodeStr = GetEnumTrainingResultCodeStr(result);
        string strTrainingResult = lblTrainingResult.Text;
        int MaxLen_TrainingResult = Convert.ToInt32(ConfigurationManager.AppSettings["MaxLenTrainingResult"]); ;
        if (strTrainingResult.Length < MaxLen_TrainingResult)
        {
            strTrainingResult = strTrainingResult + strCodeStr;
        }
        else
        {
            strTrainingResult = strTrainingResult.Substring(1); //去掉左起第一个字
            strTrainingResult = strTrainingResult + strCodeStr;
        }

        string connectionStr;
        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
            string sql = "UPDATE wordstudyhistory SET trainingresult = '" +
                    strTrainingResult + "' WHERE UnitScheduleId = " +
                    Session["SelectedScheduleID"].ToString() + " AND WordId =" +
                    lblWordId.Text;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            int TimeUsedBefore = 0;
            string sqlTimeUsedBefore = "SELECT timeused FROM unitstudyschedule WHERE id =" + Session["SelectedScheduleID"].ToString();
            MySqlCommand cmdTimeUsedBefore = new MySqlCommand(sqlTimeUsedBefore, conn);
            MySqlDataReader rdr = cmdTimeUsedBefore.ExecuteReader();
            while (rdr.Read())
            {
                TimeUsedBefore = Int32.Parse(rdr.IsDBNull(0) ? "0" : rdr[0].ToString());
            }
            rdr.Close();

            TimeUsedBefore = TimeUsedBefore + GetTimeUsedOfCurrentWord();

            string sqlUpdateTimeSpend = "UPDATE unitstudyschedule SET TimeUsed ='" + TimeUsedBefore.ToString() + "' WHERE id = " + Session["SelectedScheduleID"].ToString();
            MySqlCommand cmdUpdateTimeSpend = new MySqlCommand(sqlUpdateTimeSpend, conn);
            cmdUpdateTimeSpend.ExecuteNonQuery();
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
        lblTrainingResult.Text = strTrainingResult;
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
    protected string GetWordExample(string wordId)
    {
        bool bRecordExist = false;
        string strExample = "";
        ArrayList strExampleArr = new ArrayList();
        string strWordBody = "";
        string strExampleFinal = "";
        string connectionStr;
        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
            string sql = "SELECT WordExample.Id, WordExample.WordId, WordExample.Example, word.WordBody FROM WordExample, word WHERE wordexample.WordId = word.Id and word.id = " + wordId;

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                bRecordExist = true;
                strExample = (rdr.IsDBNull(2) ? "" : rdr[2].ToString());
                strExampleArr.Add(strExample);
                strWordBody = (rdr.IsDBNull(3) ? "" : rdr[3].ToString());
                
            }
            rdr.Close();

            if (bRecordExist)
            {
                Random rnd = new Random();
                int rndKey = rnd.Next(strExampleArr.Count-1);
                strExample = (string)strExampleArr[rndKey];
                strExampleFinal = strExample.Replace(strWordBody, "__________");
            }
            else
            {
                strExampleFinal = "NO QUIZ FOUND, Please report the wordId.";
            }
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

        return strExampleFinal;
    }

    protected string GetWordSynonyms(string wordId)
    {
        bool bRecordExist = false;
        string strSynonyms = "";
        ArrayList strSynonymsArr = new ArrayList();
        string strWordBody = "";
        string strSynonymsFinal = "";
        string connectionStr;
        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
            string sql = "SELECT WordSynonyms.Id, WordSynonyms.WordId, WordSynonyms.Synonyms, word.WordBody FROM WordSynonyms, word WHERE wordSynonyms.WordId = word.Id and word.id = " + wordId;

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                bRecordExist = true;
                strSynonyms = (rdr.IsDBNull(2) ? "" : rdr[2].ToString());
                strSynonymsArr.Add(strSynonyms);
                strWordBody = (rdr.IsDBNull(3) ? "" : rdr[3].ToString());

            }
            rdr.Close();

            if (bRecordExist)
            {
                int seed = Guid.NewGuid().GetHashCode();
                Random rnd = new Random(seed);
                int rndKey = rnd.Next(strSynonymsArr.Count - 1);
                strSynonymsFinal = (string)strSynonymsArr[rndKey];
            }
            else
            {
                strSynonymsFinal = string.Empty;
            }
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

        return strSynonymsFinal;
    }
    protected string[] GetOptions(enum_PageMode pageMode, int rowId)
    {
        //返回的数组中，前4个为选项
        //第5个为正确答案索引
        string[] straryOptions = new string[5];
        //bool blRowIdFound = false;
        //int intRowIdIndex = 0;

        int PerUnitWordsAmount = Convert.ToInt32(ConfigurationManager.AppSettings["PerUnitWordsAmount"]);
        int[] array1 = UseDoubleArrayToNonRepeatedRandom(PerUnitWordsAmount);

        int seed = Guid.NewGuid().GetHashCode();
        Random rnd = new Random(seed);
        int currentWordOptionKey = rnd.Next(4);

        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] == rowId) //说明i对应的词中是待测的词
            {
                int temp = array1[currentWordOptionKey];
                array1[currentWordOptionKey] = array1[i];
                array1[i] = temp;
            }
        }
        /*
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
        }*/

        int findIndexStart = currentWordOptionKey;
        for (int i = 0; i <= 3; i++)
        {
            if (pageMode == enum_PageMode.WordDesc)
            {
                straryOptions[i] = GetWordRdnDesc(array1[i]); //GetWordAllDesc(array1[i]);
            }
            else if (pageMode == enum_PageMode.CompleteSentence)
            {
                straryOptions[i] = GetWordBody(array1[i]);
            }
            else if (pageMode == enum_PageMode.Synonyms)
            {
                string synonymsFound = string.Empty;
                if (i != currentWordOptionKey)
                {
                    while (synonymsFound == string.Empty && findIndexStart < array1.Length-1)
                    {
                        findIndexStart++;
                        string wordId = GetWordId(array1[findIndexStart]);
                        synonymsFound = GetWordSynonyms(wordId); 
                    }
                }
                else
                {
                    string wordId = GetWordId(array1[i]);
                    synonymsFound = GetWordSynonyms(wordId);
                }

                if (synonymsFound == string.Empty)
                {
                    synonymsFound = "NO Synonyms found";
                }
                straryOptions[i] = synonymsFound;

            }
        }
        straryOptions[4] = currentWordOptionKey.ToString();

        return straryOptions;
    }

    protected void ShowQuizAndOptions(int rowId)
    {
        if (rowId >= Convert.ToInt32(ConfigurationManager.AppSettings["PerUnitWordsAmount"]))
        {
            return;
        }

        if (GetCurrentPageMode()==enum_PageMode.WordDesc)
        {
            lblQuizBody.Text = ((Label)HolderTable.Rows[rowId].Cells[2].Controls[0]).Text;
        }
        else if(GetCurrentPageMode() == enum_PageMode.CompleteSentence)
        {
            string wordId = GetWordId(rowId);
            lblQuizBody.Text = GetWordExample(wordId);
        }
        else if (GetCurrentPageMode()== enum_PageMode.Synonyms)
        {
            lblQuizBody.Text = ((Label)HolderTable.Rows[rowId].Cells[2].Controls[0]).Text;
        }

        string[] strarrOptions = GetOptions(GetCurrentPageMode(),rowId);
        rblOptions.Items.Clear();
        for (int i = 0; i < 4; i++)
        {
            ListItem item = new ListItem();
            item.Value = i.ToString();
            item.Text = strarrOptions[i];
            rblOptions.Items.Add(item);
        }
        ListItem itemDontknow = new ListItem();
        itemDontknow.Value = "4";
        itemDontknow.Text = "I don't know!";
        rblOptions.Items.Add(itemDontknow);

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
        int seed = Guid.NewGuid().GetHashCode();
        Random rnd = new Random(seed);
        int rndKey, tryTimes = 0;
        while (strOption == string.Empty && tryTimes < 20)
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
    protected string GetWordId(int rowId)
    {
        return ((Label)HolderTable.Rows[rowId].Cells[0].Controls[0]).Text;
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
            array[j] = index[idx]-1;          // 在随机索引位置取出一个数，保存到结果数组 
            index[idx] = index[site - 1];   // 作废当前索引位置数据，并用数组的最后一个数据代替之
            site--;                         // 索引位置的上限减一（弃置最后一个数据）
        }
        return array;
    }

    protected string GetEnumTrainingResultCodeStr(enum_TraningResult result)
    {
        string strTempCodeStr = "";
        if (result == enum_TraningResult.Wrong)
        { strTempCodeStr = "0"; }
        else if (result == enum_TraningResult.Correct)
        { strTempCodeStr = "1"; }
        else if (result == enum_TraningResult.WithHint)
        { strTempCodeStr = "2"; }
        else if (result == enum_TraningResult.QuickCorrect)
        { strTempCodeStr = "3"; }
        return strTempCodeStr;
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

            int quickCorrectSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["QuickCorrectSeconds"]);
            int timeUsedOfCurrentWord = GetTimeUsedOfCurrentWord();
            if (timeUsedOfCurrentWord < quickCorrectSeconds)
            {
                lblSubmitResult.Text = "QUICK CORRECT!";
                UpdateTrainingResult(enum_TraningResult.QuickCorrect);
            }
            else
            {
                lblSubmitResult.Text = "CORRECT!";
                UpdateTrainingResult(enum_TraningResult.Correct);
            }


            /*          if (lvHints.Visible)
                      {
                          //xh 20210806 无论是否打开了hint，都提交Correct
                          //UpdateTrainingResult(enum_TraningResult.WithHint); 
                          UpdateTrainingResult(enum_TraningResult.Correct);
                      }
                      else
                      {
                          UpdateTrainingResult(enum_TraningResult.Correct);
                      }   */
        }
        else
        {
            int idx = GetTestingWordIndex();
            int rowId = GetRowIdByCurrentLoopIndex(idx);
            lblSubmitResult.ForeColor = System.Drawing.Color.Red;
            lblSubmitResult.Text = "SORRY! Anwser is " + Convert.ToString(iCorrectOptionIndex + 1) + ". <br/>Definition: " +
                                    GetWordAllDesc(rowId);
            UpdateTrainingResult(enum_TraningResult.Wrong);
        }

        btnSubmit.Enabled = false;
        btnNext.Enabled = true;
    }

    protected string GetInspectingStr()
    {
        string strTemp = "";
        int inspectingLastNTimes = Convert.ToInt32(ConfigurationManager.AppSettings["InspectingLastNTimes"]);
        for (int i = 0; i < inspectingLastNTimes; i++)
        {
            strTemp = strTemp + "1";
        }
        return strTemp;
    }

    protected bool WalkToNextTestingWordIndex()
    {
        //如果走到末尾，返回false，否则返回true
        int PerUnitWordsAmount = Convert.ToInt32(ConfigurationManager.AppSettings["PerUnitWordsAmount"]);
        int wordsInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["WordsInEveryLoop"]);
        string inspectingStr = GetInspectingStr();
        int currentLoopUpperIndex = GetCurrentLoopUpperIndex();
        int currentLoopLowerIndex = GetCurrentLoopLowerIndex();

        int idx = GetTestingWordIndex();
        int stepIdx = GetRowIdByCurrentLoopIndex(idx);
        int stepCount = 0;
        bool bFindWordWaitingTrainingInCurrentLoop = false;

        string strTrainingResult = "";
        while (!bFindWordWaitingTrainingInCurrentLoop && stepCount < wordsInEveryLoop)
        {
            stepIdx++;
            stepCount++;
            if (stepIdx >= currentLoopUpperIndex)
            {
                stepIdx = currentLoopLowerIndex;
            }
            //int rowId = GetRowIdByCurrentLoopIndex(stepIdx);
            string wordId = GetWordId(stepIdx);
            strTrainingResult = GetTrainingResultStr(Session["SelectedScheduleID"].ToString(), wordId);
            if (!(strTrainingResult.EndsWith(inspectingStr) || strTrainingResult.EndsWith(GetEnumTrainingResultCodeStr(enum_TraningResult.QuickCorrect))))
            {
                bFindWordWaitingTrainingInCurrentLoop = true;
            }
        }

        if (bFindWordWaitingTrainingInCurrentLoop)
        {//说明currentLoop还有要training的word
            int indexInLoop = stepIdx % wordsInEveryLoop;
            SetTestingWordIndex(indexInLoop);
            return true;
        }
        else
        {//说明currentLoop所有的word已经training完成，可以去下一个loop
            IncreaseLoopCount();
            int totalLoops = PerUnitWordsAmount / wordsInEveryLoop;
            if (GetLoopCount() >= totalLoops)
            {
                //如果走到末尾，返回fals
                return false;
            }
            else
            {
                SetTestingWordIndex(0);
                return true;
            }
        }
    }

    protected string GetTrainingResultStr(string scheduleId, string wordId)
    {
        string strTrainingResult = "";
        bool bRecordExist = false;
        string connectionStr;
        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
            string sql = "SELECT Id, TrainingResult FROM WordStudyHistory WHERE UnitScheduleId =" +
                scheduleId + " AND WordId =" + wordId;

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                bRecordExist = true;
                strTrainingResult = (rdr.IsDBNull(1) ? "" : rdr[1].ToString());
            }
            rdr.Close();

            if (!bRecordExist)
            {
                string sqlInsert = "INSERT INTO wordstudyhistory(UnitScheduleId, WordId, TrainingResult) VALUES(" +
                scheduleId + "," +
                wordId + ", '')";
                MySqlCommand cmdInsert = new MySqlCommand(sqlInsert, conn);
                cmdInsert.ExecuteNonQuery();
            }
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
        return strTrainingResult;
    }

    protected void NextProcess()
    {
        int idx = GetTestingWordIndex();
        int rowId = GetRowIdByCurrentLoopIndex(idx);
        ShowAllAboutWord(rowId);

        btnSubmit.Enabled = true;
        btnNext.Enabled = false;
        lvHints.Visible = false;
        txtNewHint.Visible = false;
        btnAddHint.Visible = false;
        txtNewHint.Text = "";
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        if (WalkToNextTestingWordIndex())
        {
            NextProcess();
        }
        else
        {
            btnNext.Enabled = false;

            //          string sql = "UPDATE unitstudyschedule SET Result=1, TimeUsed='"+ lblTimer.Text + "' WHERE id = "+ Session["SelectedScheduleID"].ToString();
            string sql = "UPDATE unitstudyschedule SET Result=1, CompleteDate='" + DateTime.Now.Date.ToString() + "' WHERE id = " + Session["SelectedScheduleID"].ToString();

            string connectionStr;

            connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
            MySqlConnection conn = new MySqlConnection(connectionStr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
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
            Response.Write("<script>alert('Completed!');location.href='ScheduleMgr.aspx';</script>");

        }
    }

    protected int GetCurrentLoopUpperIndex()
    {
        int wordsInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["WordsInEveryLoop"]);
        int loopCount = GetLoopCount();
        return loopCount * wordsInEveryLoop + wordsInEveryLoop;
    }
    protected int GetCurrentLoopLowerIndex()
    {
        int wordsInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["WordsInEveryLoop"]);
        int loopCount = GetLoopCount();
        return loopCount * wordsInEveryLoop;
    }

    protected int GetRowIdByCurrentLoopIndex(int indexInLoop)
    {
        int wordsInEveryLoop = Convert.ToInt32(ConfigurationManager.AppSettings["WordsInEveryLoop"]);
        int loopCount = GetLoopCount();
        return loopCount * wordsInEveryLoop + indexInLoop;
    }

    protected void lvHints_ItemDeleting(object sender, ListViewDeleteEventArgs e)
    {
        string strHintId = "";
        Label lbl = (lvHints.Items[e.ItemIndex].FindControl("lblHintId")) as Label;
        if (lbl != null)
            strHintId = lbl.Text;

        string DeleteQuery = "Delete from WordHint WHERE Id = '" + strHintId + "'";
        string connectionStr;

        connectionStr = ConfigurationManager.ConnectionStrings["worddbConnectionString"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connectionStr);
        try
        {
            conn.Open();
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
        lvHints.DataBind();
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

            string sql = "INSERT INTO wordhint (UserId, WordId, Hint) VALUES (1, @wordid, @hint) ";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("wordid", lblWordId.Text);
            cmd.Parameters.AddWithValue("hint", txtNewHint.Text);
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
        lvHints.DataBind();
        txtNewHint.Text = "";
    }

    protected void btnShowHint_Click(object sender, EventArgs e)
    {
        lvHints.DataBind();
        lvHints.Visible = true;
        txtNewHint.Visible = true;
        btnAddHint.Visible = true;
    }

    protected void btnMain_Click(object sender, EventArgs e)
    {
        Response.Redirect("UnitSchedule.aspx");
    }

    protected int GetTimeUsedOfCurrentWord()
    {
        DateTime _loggedInTime;
        _loggedInTime = Convert.ToDateTime(Session["LoggedInTime"]);

        TimeSpan ts = DateTime.Now.Subtract(_loggedInTime);
        int seconds = (((ts.Days * 24) * 3600) + (ts.Hours * 3600) + (ts.Minutes * 60) + (ts.Seconds));
        return seconds;
    }

    /*
    protected string GetTimeTrainingSpent()
    {
        DateTime _loggedInTime;
        _loggedInTime = Convert.ToDateTime(Session["LoggedInTime"]);

        TimeSpan ts = DateTime.Now.Subtract(_loggedInTime);

        string str = "";
        if (ts.Hours > 0)
        {
            str = String.Format("{0:00}", ts.Hours) + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
        }
        if (ts.Hours == 0 && ts.Minutes > 0)
        {
            str = "00:" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
        }
        if (ts.Hours == 0 && ts.Minutes == 0)
        {
            str = "00:00:" + String.Format("{0:00}", ts.Seconds);
        }

        return str;
        //int elapsedtime = Convert.ToInt32(elapsedtimespan.TotalSeconds);
        //lblTimer.Text = DateTime.Now.ToString();
    }*/

}