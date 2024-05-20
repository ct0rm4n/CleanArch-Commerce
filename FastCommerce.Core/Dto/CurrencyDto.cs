
namespace Core.Dto
{
    public class CurrencyDto
    {
        public CurrencyByLocaleDto USDBRL { get; set; }
        public CurrencyByLocaleDto EURBRL { get; set; }
        public CurrencyByLocaleDto BTCBRL { get; set; }
    }

    public class CurrencyByLocaleDto
    {
        public string code { get; set; }
        public string codein { get; set; }
        public string name { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string varBid { get; set; }
        public string pctChange { get; set; }
        public string bid { get; set; }
        public string ask { get; set; }
        public string timestamp { get; set; }
        public string create_date { get; set; }
    }

}
