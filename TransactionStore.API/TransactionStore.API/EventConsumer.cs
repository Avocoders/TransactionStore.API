using MassTransit;
using System.Threading.Tasks;
using Messaging;
using System.Collections.Generic;
using System;

namespace TransactionStore.API
{
    public class EventConsumer : IConsumer<Currencies>
    {
        public async Task Consume(ConsumeContext<Currencies> context)
        {
            await Console.Out.WriteLineAsync(context.Message.Rates[0].Rate.ToString());
        }
    }
}
