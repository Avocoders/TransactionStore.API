using System;

namespace TransactionStore.Data
{
    public class TransactionSearchParameters
    {
        public long? LeadId { get; set; }
        public byte Type { get; set; }
        public byte Currency { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? FromDate { get; set; } = null;
        public DateTime? TillDate { get; set; } = null;
    }
}
