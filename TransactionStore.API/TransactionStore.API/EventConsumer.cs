using MassTransit;
using System.Threading.Tasks;
using Messaging;
using System.Collections.Generic;
using System;
using TransactionStore.Data;

namespace TransactionStore.API
{
    public class EventConsumer : IConsumer<Currencies>
    {
        private Currencies _currencies;
        private ITransactionRepository _repo;
        public EventConsumer(Currencies currencies, ITransactionRepository repo)
        {
            _currencies = currencies;
            _repo = repo;
        }
        public async Task Consume(ConsumeContext<Currencies> context)
        {                     
            _currencies.Rates = context.Message.Rates;
            _repo.UpdateCurrencyRates();
        }
    }
}
