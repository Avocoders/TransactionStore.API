using MassTransit;
using System.Threading.Tasks;
using Messaging;
using System.Collections.Generic;

namespace TransactionStore.API
{
    public class EventConsumer : IConsumer<CurrencyRates>
    {
        public async Task Consume(ConsumeContext<CurrencyRates> context)
        {
            //вариант со словарем (рабочий)          
            Currencies.Rates = new Dictionary<string, decimal>();
            Currencies.Rates.Add("USD", (decimal)context.Message.USD);
            Currencies.Rates.Add("RUB", (decimal)context.Message.RUB);
            Currencies.Rates.Add("JPY", (decimal)context.Message.JPY);
            Currencies.Rates.Add("EUR", 1);

            //вариант со списоком
            //currencies.Rates = new List<Currency>();
            //currencies.Rates.Add(new Currency { Code = "USD", Rate = (decimal)context.Message.USD });
            //currencies.Rates.Add(new Currency { Code = "RUB", Rate = (decimal)context.Message.RUB });
            //currencies.Rates.Add(new Currency { Code = "JPY", Rate = (decimal)context.Message.JPY });                       
        }
    }
}
