using System.Collections.Generic;

namespace Messaging
{
    public interface CurrencyRates
    {
        double USD { get; set; }
        double RUB { get; set; }
        double JPY { get; set; }        
    }

    public static class Currencies
    {
        public static Dictionary<string, decimal> Rates { get; set; } 
        //public List<Currency> Rates { get; set; }        
    }

    //public class Currency  на всякий случай  
    //{       
    //    public string Code { get; set; }
    //    public decimal Rate { get; set; }
    //}
}
