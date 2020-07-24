using System;
using System.Collections.Generic;
using System.Linq;
using TransactionStore.Core.Shared;
using TransactionStore.Data;
using TransactionStore.Data.DTO;

namespace TransactionStore.Business
{
    public class TransactionService : ITransactionService
    {
        ITransactionRepository _transactionRepository;
        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public DataWrapper<List<TransactionDto>> SearchTransactions(TransactionSearchParameters searchParameters)
        {
            var data = _transactionRepository.SearchTransactions(searchParameters);
            if (data.IsOk)
            {
                data.Data = ProcessTransactions(data.Data);
            }
            return data;
        }

        private List<TransactionDto> ProcessTransactions(List<TransactionDto> transactions)
        {
            var nonTransferTransactions = transactions.Where(t => t.Type.Id != (byte)TransactionType.Transfer).ToList();
            var transferTransactions = transactions.Where(t => t.Type.Id == (byte)TransactionType.Transfer).ToList();
            List<TransferTransaction> transfers = new List<TransferTransaction>();
            foreach (var transfer in transferTransactions)
            {
                if (transfer.Amount < 0)
                {
                    var transferReceiver = transferTransactions
                        .Where(tT => tT.Amount > 0)
                        .Where(tT => tT.Currency.Id == transfer.Currency.Id)
                        .Where(tT => tT.Timestamp == transfer.Timestamp)
                        .FirstOrDefault(tT => tT.Amount == Math.Abs(transfer.Amount));
                    transfers.Add(new TransferTransaction()
                    {
                        Id = transfer.Id,
                        LeadId = transfer.LeadId,
                        Type = transfer.Type,
                        Currency = transfer.Currency,
                        Amount = transfer.Amount,
                        Timestamp = transfer.Timestamp,
                        LeadIdReceiver = transferReceiver?.LeadId ?? -1
                    });
                }
            }
            nonTransferTransactions.AddRange(transfers);
            return nonTransferTransactions;
        }
    }
}
