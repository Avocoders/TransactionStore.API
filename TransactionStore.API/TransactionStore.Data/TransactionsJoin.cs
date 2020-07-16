using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using TransactionStore.Data.DTO;

namespace TransactionStore.Data
{
    public class TransactionsJoin
    {
        public List<TransactionDto> NonTransfers { get; set; }
        public List<TransferTransaction> Transfers { get; set; }
    }
}
