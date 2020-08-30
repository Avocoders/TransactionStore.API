using ADO.Net.Client.Annotations;
using System;

namespace TransactionStore.Data.DTO
{
    public class BalanceDto
    {
        public decimal Balance { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
