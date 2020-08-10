using System.Collections.Generic;

namespace Messaging
{
    public class CurrencyRates
    {
        public List<Currency> Rates { get; set; }
    }

    public class Currency
    {
        public string Code { get; set; }
        public decimal Rate { get; set; }
    }
}
