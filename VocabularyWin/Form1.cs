using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VocabularyWin
{
    public class Phonetic
    {
        public string text { get; set; }
        public string audio { get; set; }
    }

    public class Definition
    {
        public string definition { get; set; }
        public string example { get; set; }
        public List<string> synonyms { get; set; }
    }

    public class Meaning
    {
        private string _partOfSpeech = "";
        public string partOfSpeech {
            get { return _partOfSpeech; }
            set
            {
                if (value.Contains("exclamation"))
                { _partOfSpeech = "e."; }
                else if (value.Contains("noun"))
                { _partOfSpeech = "n."; }
                else if (value.Contains("verb"))
                { _partOfSpeech = "v."; }
                else if (value.Contains("adjective"))
                { _partOfSpeech = "adj."; }
                //else
                //{ _partOfSpeech = String.IsNullOrEmpty(value) ? "" : value; }
            }
        }
        public List<Definition> definitions { get; set; }
    }

    public class Root
    {
        public string word { get; set; }
        public List<Phonetic> phonetics { get; set; }
        public List<Meaning> meanings { get; set; }
    }

    public class SearchResult
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://api.dictionaryapi.dev/api/v2/entries/en/impending";
            //string url = "https://api.dictionaryapi.dev/api/v2/entries/en/hello";
            string temp = HttpCommon.HttpGet(url);
            string response = temp.Substring(1, temp.Length - 2);
            /*string googleSearchText = @"
{""word"":""hello"",""phonetic"":""həˈləʊ"",""phonetics"":[{""text"":""həˈləʊ"",""audio"":""//ssl.gstatic.com/dictionary/static/sounds/20200429/hello--_gb_1.mp3""},{""text"":""hɛˈləʊ""}],""origin"":""early 19th century: variant of earlier hollo ; related to holla."",""meanings"":[{""partOfSpeech"":""exclamation"",""definitions"":[{""definition"":""used as a greeting or to begin a phone conversation."",""example"":""hello there, Katie!"",""synonyms"":[],""antonyms"":[]}]},{""partOfSpeech"":""noun"",""definitions"":[{""definition"":""an utterance of ‘hello’; a greeting."",""example"":""she was getting polite nods and hellos from people"",""synonyms"":[],""antonyms"":[]}]},{""partOfSpeech"":""verb"",""definitions"":[{""definition"":""say or shout ‘hello’."",""example"":""I pressed the phone button and helloed"",""synonyms"":[],""antonyms"":[]}]}]}
";
          string response = @"
{""word"":""impending"",""phonetic"":""ɪmˈpɛndɪŋ"",""phonetics"":[{""text"":""ɪmˈpɛndɪŋ"",""audio"":""//ssl.gstatic.com/dictionary/static/sounds/20200429/impending--_gb_1.mp3""}],""meanings"":[{""partOfSpeech"":""adjective"",""definitions"":[{""definition"":""(of an event regarded as threatening or significant) about to happen; forthcoming."",""example"":""the author had returned to his country ahead of the impending war"",""synonyms"":[],""antonyms"":[]}]}]},{""word"":""impend"",""phonetic"":""ɪmˈpɛnd"",""phonetics"":[{""text"":""ɪmˈpɛnd"",""audio"":""//ssl.gstatic.com/dictionary/static/sounds/20200429/impend--_gb_1.mp3""}],""origin"":""late 16th century: from Latin impendere, from in- ‘towards, upon’ + pendere ‘hang’."",""meanings"":[{""partOfSpeech"":""verb"",""definitions"":[{""definition"":""(of an event regarded as threatening or significant) be about to happen."",""example"":""it seemed certain that some great trial of strength impended between the opponents"",""synonyms"":[""imminent"",""at hand"",""close"",""close at hand"",""near"",""nearing"",""approaching"",""coming"",""forthcoming"",""upcoming"",""to come"",""on the way"",""about to happen"",""upon us"",""in store"",""in the offing"",""in the pipeline"",""on the horizon"",""in the air"",""in the wind"",""brewing"",""looming"",""looming large"",""threatening"",""menacing"",""on the cards""],""antonyms"":[]}]}]}
";*/

            try
            {
                Root myRoot = JsonConvert.DeserializeObject<Root>(response);
                MessageBox.Show(myRoot.word);
                foreach (Meaning m in myRoot.meanings)
                {
                    MessageBox.Show(m.partOfSpeech + ":" + m.definitions[0].definition);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
 //           MessageBox.Show(myRoot.meanings[0].partOfSpeech + ":" + myRoot.meanings[0].definitions[0].definition);
  
        }
    }
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

        /// <summary>
        /// Http同步Get异步请求
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="callBackDownStringCompleted">回调事件</param>
        /// <param name="encode">编码(默认UTF8)</param>
        public static void HttpGetAsync(string url,
            DownloadStringCompletedEventHandler callBackDownStringCompleted = null, Encoding encode = null)
        {
            var webClient = new WebClient { Encoding = Encoding.UTF8 };

            if (encode != null)
                webClient.Encoding = encode;

            if (callBackDownStringCompleted != null)
                webClient.DownloadStringCompleted += callBackDownStringCompleted;

            webClient.DownloadStringAsync(new Uri(url));
        }

        /// <summary>
        ///  Http同步Post同步请求
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="postStr">请求Url数据</param>
        /// <param name="encode">编码(默认UTF8)</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postStr = "", Encoding encode = null)
        {
            string result;

            try
            {
                var webClient = new WebClient { Encoding = Encoding.UTF8 };

                if (encode != null)
                    webClient.Encoding = encode;

                var sendData = Encoding.GetEncoding("GB2312").GetBytes(postStr);

                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                webClient.Headers.Add("ContentLength", sendData.Length.ToString(CultureInfo.InvariantCulture));

                var readData = webClient.UploadData(url, "POST", sendData);

                result = Encoding.GetEncoding("GB2312").GetString(readData);

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Http同步Post异步请求
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="postStr">请求Url数据</param>
        /// <param name="callBackUploadDataCompleted">回调事件</param>
        /// <param name="encode"></param>
        public static void HttpPostAsync(string url, string postStr = "",
            UploadDataCompletedEventHandler callBackUploadDataCompleted = null, Encoding encode = null)
        {
            var webClient = new WebClient { Encoding = Encoding.UTF8 };

            if (encode != null)
                webClient.Encoding = encode;

            var sendData = Encoding.GetEncoding("GB2312").GetBytes(postStr);

            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            webClient.Headers.Add("ContentLength", sendData.Length.ToString(CultureInfo.InvariantCulture));

            if (callBackUploadDataCompleted != null)
                webClient.UploadDataCompleted += callBackUploadDataCompleted;

            webClient.UploadDataAsync(new Uri(url), "POST", sendData);
        }

        public static string HttpRequest(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string results;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            // 加入憑證驗證
            request.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate());

            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                results = sr.ReadToEnd();
                sr.Close();
            }
            return results;
        }
    }
}
/*
            //string url = "https://www.cnblogs.com/wphl-27/p/8358025.html";
            //string url = "http://i.jandan.net/";
            //string url = "http://localhost:8081/unitSchedule";
            string url = "https://api.dictionaryapi.dev/api/v2/entries/en/hello";
            //string url = "https://dictionaryapi.dev/";
            MessageBox.Show(HttpCommon.HttpGet(url));
            //MessageBox.Show(HttpCommon.HttpPost(url));
            //MessageBox.Show(HttpCommon.HttpRequest(url));
            */
/*
string googleSearchText = @"{
'responseData': {
'results': [
{
'GsearchResultClass': 'GwebSearch',
'unescapedUrl': 'http://en.wikipedia.org/wiki/Paris_Hilton',
'url': 'http://en.wikipedia.org/wiki/Paris_Hilton',
'visibleUrl': 'en.wikipedia.org',
'cacheUrl': 'http://www.google.com/search?q=cache:TwrPfhd22hYJ:en.wikipedia.org',
'title': '<b>Paris Hilton</b> - Wikipedia, the free encyclopedia',
'titleNoFormatting': 'Paris Hilton - Wikipedia, the free encyclopedia',
'content': '[1] In 2006, she released her debut album...'
},
{
'GsearchResultClass': 'GwebSearch',
'unescapedUrl': 'http://www.imdb.com/name/nm0385296/',
'url': 'http://www.imdb.com/name/nm0385296/',
'visibleUrl': 'www.imdb.com',
'cacheUrl': 'http://www.google.com/search?q=cache:1i34KkqnsooJ:www.imdb.com',
'title': '<b>Paris Hilton</b>',
'titleNoFormatting': 'Paris Hilton',
'content': 'Self: Zoolander. Socialite <b>Paris Hilton</b>...'
}
],
'cursor': {
'pages': [
{
'start': '0',
'label': 1
},
{
'start': '4',
'label': 2
},
{
'start': '8',
'label': 3
},
{
'start': '12',
'label': 4
}
],
'estimatedResultCount': '59600000',
'currentPageIndex': 0,
'moreResultsUrl': 'http://www.google.com/search?oe=utf8&ie=utf8...'
}
},
'responseDetails': null,
'responseStatus': 200
}";
JObject googleSearch = JObject.Parse(googleSearchText);
//将获得的 Json 结果转换为列表
IList<JToken> results = googleSearch["responseData"]["results"].Children().ToList();
//将 Json 结果反序列化为 .NET 对象
IList<SearchResult> searchResults = new List<SearchResult>();
foreach (JToken result in results)
{
    SearchResult searchResult = JsonConvert.DeserializeObject<SearchResult>(result.ToString());
    searchResults.Add(searchResult);
}
// lblWordBody.Text = searchResults[0].Content;
// Title = <b>Paris Hilton</b> - Wikipedia, the free encyclopedia
// Content = [1] In 2006, she released her debut album...
// Url = http://en.wikipedia.org/wiki/Paris_Hilton

// Title = <b>Paris Hilton</b>
// Content = Self: Zoolander. Socialite <b>Paris Hilton</b>...
// Url = http://www.imdb.com/name/nm0385296/

    MessageBox.Show(searchResults[0].Content);
    */

