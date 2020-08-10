using MassTransit;
using System;
using System.Threading.Tasks;
using Messaging;

namespace TransactionStore.API
{
    public class EventConsumer : IConsumer<CurrencyRates>
    {
        public async Task Consume(ConsumeContext<CurrencyRates> context)
        {
            string usd = context.Message.USD.ToString();
            string rub = context.Message.RUB.ToString();
            string jpy = context.Message.JPY.ToString();
            await Console.Out.WriteLineAsync(usd);
            await Console.Out.WriteLineAsync(rub);
            await Console.Out.WriteLineAsync(jpy);
            //return Task.CompletedTask;
        }
    }
}
