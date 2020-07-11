using System;

namespace TransactionStore.API.Models.Output
{
    public class TransactionOutputModel
    {
        public long Id { get; set; }
        public long LeadId { get; set; }
        public string Type { get; set; } // Type.Name
        public string Currency { get; set; } // Currency.Code
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
