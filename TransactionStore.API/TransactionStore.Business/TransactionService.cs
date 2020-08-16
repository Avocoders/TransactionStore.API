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

        public DataWrapper<List<TransactionDto>> GetById(long id) 
        {
            return MakeResponse(_transactionRepository.GetById(id));
        }

        public DataWrapper<List<TransactionDto>> GetByAccountId(long accountId) 
        {
            return MakeResponse(_transactionRepository.GetByAccountId(accountId));
        }

        public DataWrapper<List<TransactionDto>> SearchTransactions(TransactionSearchParameters searchParameters)
        {
            return MakeResponse(_transactionRepository.SearchTransactions(searchParameters));
        }

        private List<TransactionDto> ProcessTransactions(List<TransactionDto> transactions)
        {
            var nonTransferTransactions = transactions.Where(t => t.Type.Id != (byte)TransactionType.Transfer).ToList();
            var transferTransactions = transactions.Where(t => t.Type.Id == (byte)TransactionType.Transfer).ToList();
            List<TransferTransactionDto> transfers = new List<TransferTransactionDto>();
            foreach (var transfer in transferTransactions)
            {
                if (transfer.Amount < 0)
                {
                    var transferReceiver = transferTransactions
                        .Where(tT => tT.Amount > 0)
                        .Where(tT => tT.Timestamp == transfer.Timestamp)
                        .FirstOrDefault();
                    transfers.Add(new TransferTransactionDto()
                    {
                        Id = transfer.Id,
                        AccountId = transfer.AccountId,
                        Type = transfer.Type,
                        Currency = transfer.Currency,
                        Amount = transfer.Amount,
                        Timestamp = transfer.Timestamp,
                        AccountIdReceiver = transferReceiver?.AccountId ?? -1
                    });
                }
            }
            nonTransferTransactions.AddRange(transfers);
            return nonTransferTransactions;            
        }

        public DataWrapper<long> AddTransaction(int type, TransactionDto transactionDto)
        {
            if (type == 1)
            { 
                transactionDto.Type = new TransactionTypeDto() { Id = (byte)TransactionType.Deposit }; 
            }
            else
            {
                transactionDto.Type = new TransactionTypeDto() { Id = (byte)TransactionType.Withdraw };
                transactionDto.Amount *= -1;
            }
            return _transactionRepository.Add(transactionDto);
        }

        private DataWrapper<List<TransactionDto>> MakeResponse(DataWrapper<List<TransactionDto>> dataWrapper)
        {
            if (dataWrapper.IsOk)
            {
                dataWrapper.Data = ProcessTransactions(dataWrapper.Data);
            }
            return dataWrapper;
        }
    }
}
