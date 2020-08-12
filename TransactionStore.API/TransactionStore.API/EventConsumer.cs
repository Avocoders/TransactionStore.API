using MassTransit;
using System.Threading.Tasks;
using Messaging;
using System.Collections.Generic;
using System;

namespace TransactionStore.API
{
    public class EventConsumer : IConsumer<Currencies>
    {
        Currencies _currencies;
        public EventConsumer(Currencies currencies)
        {
            _currencies = currencies;
        }
        public async Task Consume(ConsumeContext<Currencies> context)
        {                     
            _currencies.Rates = context.Message.Rates;
            await Console.Out.WriteLineAsync(context.Message.Rates[0].Rate.ToString());
        }
    }
}
