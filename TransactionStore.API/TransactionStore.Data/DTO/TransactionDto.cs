using System;

namespace TransactionStore.Data.DTO
{
    public class TransactionDto
    {
        public long? Id { get; set; }
        public long AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }        
        public TransactionTypeDto Type { get; set; }
        public CurrencyDto Currency { get; set; }
        public decimal ExchangeRates { get; set; }
        public long? AccountIdReceiver { get; set; }
    }
}