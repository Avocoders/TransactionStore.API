using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data.DTO
{
    public class TransactionDTO
    {
        public TransactionDTO() { }
        public TransactionDTO(long id, long leadId, byte typeId, byte currencyId, decimal amount, DateTime timestamp)
        {
            Id = id;
            LeadId = leadId;
            TypeId = typeId;
            CurrencyId = currencyId;
            Amount = amount;
            Timestamp = timestamp;
        }


        public long? Id { get; set; }
        public long LeadId { get; set; }
        public byte TypeId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
