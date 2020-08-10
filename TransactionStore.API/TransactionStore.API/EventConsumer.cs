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
            await Console.Out.WriteLineAsync(context.Message.Rates[0].Code);            
            //return Task.CompletedTask;
        }
    }
}
