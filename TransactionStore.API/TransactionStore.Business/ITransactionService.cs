using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionStore.Data;
using TransactionStore.Data.DTO;

namespace TransactionStore.Business
{
    public interface ITransactionService
    {
        ValueTask<DataWrapper<List<TransactionDto>>> SearchTransactions(TransactionSearchParameters searchParameters);
        ValueTask<DataWrapper<List<TransactionDto>>> GetById(long id);
        ValueTask<DataWrapper<List<TransactionDto>>> GetByAccountId(long accountId);        
        ValueTask<DataWrapper<long>> AddTransaction(int type, TransactionDto transactionDto);        
    }
}