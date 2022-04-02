using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class _4
    {
        public int Course_Type { get; set; }
        public string Course_Type_Name { get; set; }
        public int Scale { get; set; }
        public int Course { get; set; }
    }

    public class _1
    {
        public string Course_Type { get; set; }
        public string Course_Type_Name { get; set; }
        public int Scale { get; set; }
        public int Course { get; set; }
    }

    public class _5
    {
        public int Course_Type { get; set; }
        public string Course_Type_Name { get; set; }
        public int Scale { get; set; }
        public int Course { get; set; }
    }

    [DataContract]
    public class Rates
    {
        [DataMember(Name = "4")]
        public _4 _4 { get; set; }
        [DataMember(Name = "1")]
        public _1 _1 { get; set; }
        [DataMember(Name = "5")]
        public _5 _5 { get; set; }
    }

    #region CurrencyTypes

    public class USD
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class CHF
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class EUR
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class GBP
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class JPY
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class NOK
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class MYR
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class DKK
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class KWD
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class CNY
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class PLN
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class ISK
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class AUD
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class AED
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class TRY
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class SGD
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class RUB
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class CAD
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class LBP
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class KZT
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class UAH
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class EGP
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }

    public class SEK
    {
        public string label { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Rates rates { get; set; }
    }
    #endregion 

    public class Data
    {
        public USD USD { get; set; }
        public CHF CHF { get; set; }
        public EUR EUR { get; set; }
        public GBP GBP { get; set; }
        public JPY JPY { get; set; }
        public NOK NOK { get; set; }
        public MYR MYR { get; set; }
        public DKK DKK { get; set; }
        public KWD KWD { get; set; }
        public CNY CNY { get; set; }
        public PLN PLN { get; set; }
        public ISK ISK { get; set; }
        public AUD AUD { get; set; }
        public AED AED { get; set; }
        public TRY TRY { get; set; }
        public SGD SGD { get; set; }
        public RUB RUB { get; set; }
        public CAD CAD { get; set; }
        public LBP LBP { get; set; }
        public KZT KZT { get; set; }
        public UAH UAH { get; set; }
        public EGP EGP { get; set; }
        public SEK SEK { get; set; }
    }

    public class IYB
    {
        public string status { get; set; }
        public Data data { get; set; }
        public string text { get; set; }
        public List<object> messages { get; set; }
        public int code { get; set; }
    }


}
