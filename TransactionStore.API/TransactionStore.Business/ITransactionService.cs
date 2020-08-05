using System.Collections.Generic;
using TransactionStore.Data;
using TransactionStore.Data.DTO;

namespace TransactionStore.Business
{
    public interface ITransactionService
    {
        DataWrapper<List<TransactionDto>> SearchTransactions(TransactionSearchParameters searchParameters);
        DataWrapper<List<TransactionDto>> GetById(long id);
        DataWrapper<List<TransactionDto>> GetByAccountId(long accountId);        
        DataWrapper<long> AddTransaction(int type, TransactionDto transactionDto);
    }
}