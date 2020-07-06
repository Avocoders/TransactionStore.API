using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data.DTO
{
    public class TransactionDTO
    {
        public Int64 Id { get; set; }
        public Int64 LeadId { get; set; }
        public byte TypeId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
       
        public TransactionDTO(Int64 id, Int64 leadId, byte typeId, byte currencyId, decimal amount, DateTime timestamp)
        {
            Id = id;
            LeadId = leadId;
            TypeId = typeId;
            CurrencyId = currencyId;
            Amount = amount;
            Timestamp = timestamp;
        }
    }
}
