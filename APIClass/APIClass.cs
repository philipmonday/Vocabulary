using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

//使用https://json2csharp.com/生成下面的类，然后将他们放入namespace
namespace api.dictionaryapi.dev
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
        public string partOfSpeech
        {
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
}

//使用https://json2csharp.com/生成下面的类，然后将他们放入namespace
namespace dictionaryapi.com//Merriam-Webster
{
    public class AppShortdef
    {
        public string hw { get; set; }
        public string fl { get; set; }
        public List<string> def { get; set; }
    }

    public class Target
    {
        public string tuuid { get; set; }
        public string tsrc { get; set; }
    }

    public class Meta
    {
        public string id { get; set; }
        public string uuid { get; set; }
        public string src { get; set; }
        public string section { get; set; }
        public string highlight { get; set; }
        public List<string> stems { get; set; }

        [JsonProperty("app-shortdef")]
        public AppShortdef AppShortdef { get; set; }
        public bool offensive { get; set; }
        public Target target { get; set; }
    }

    public class Sound
    {
        public string audio { get; set; }
    }

    public class Pr
    {
        public string ipa { get; set; }
        public Sound sound { get; set; }
    }

    public class Altpr
    {
        public string ipa { get; set; }
    }

    public class Hwi
    {
        public string hw { get; set; }
        public List<Pr> prs { get; set; }
        public List<Altpr> altprs { get; set; }
    }

    public class In
    {
        public string il { get; set; }
        public string @if { get; set; }
        public string ifc { get; set; }
    }

    public class Def
    {
        public List<List<List<object>>> sseq { get; set; }
    }

    public class Vr
    {
        public string vl { get; set; }
        public string va { get; set; }
    }

    public class Dro
    {
        public string drp { get; set; }
        public List<Def> def { get; set; }
        public List<Vr> vrs { get; set; }
    }

    public class Uro
    {
        public string ure { get; set; }
        public List<Pr> prs { get; set; }
        public string fl { get; set; }
        public List<List<object>> utxt { get; set; }
        public List<In> ins { get; set; }
        public string gram { get; set; }
    }

    public class Root
    {
        public Meta meta { get; set; }
        public int hom { get; set; }
        public Hwi hwi { get; set; }
        public string fl { get; set; }
        public List<In> ins { get; set; }
        public string gram { get; set; }
        public List<Def> def { get; set; }
        public List<Dro> dros { get; set; }
        public List<string> dxnls { get; set; }
        public List<string> shortdef { get; set; }
        public List<Uro> uros { get; set; }
    }
}