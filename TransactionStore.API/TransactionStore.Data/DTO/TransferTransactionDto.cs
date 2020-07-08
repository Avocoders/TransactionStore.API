using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data.DTO
{
    public class TransferTransactionDto
    {
        public long? Id { get; set; }
        public long LeadId { get; set; }
        public byte TypeId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public long DestinationLeadId { get; set; }
    }
}
