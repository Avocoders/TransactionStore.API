using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data.DTO
{
    public class SearchByTransactionDto
    {
        public long LeadId { get; set; }
        public string Type { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime TillDate { get; set; }
    }
}
