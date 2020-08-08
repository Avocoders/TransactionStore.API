using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Core
{
    public class ExchangeRates
    {
        public decimal GetExchangeRates(byte currencyId)
        {
            switch (currencyId)
            {
                case 1:
                    return 86.12M;

                case 2:
                    return 1.14M;

                case 3:
                    return 1.0M;
                case 4:
                    return 124.1M;

            }
            return -1;
        }

    }
}
