using System;

namespace TransactionStore.Data
{
    public class TransactionSearchParameters
    {
        public long? AccountId { get; set; }
        public byte? TypeId { get; set; }
        public byte? CurrencyId { get; set; }
        public decimal? AmountBegin { get; set; }
        public decimal? AmountEnd { get; set; }
        public DateTime? FromDate { get; set; } = null;
        public DateTime? TillDate { get; set; } = null;
    }
}
