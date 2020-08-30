using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionStore.Data.DTO;

namespace TransactionStore.Data
{
    public interface ITransactionRepository
    {
        ValueTask<DataWrapper<long>> Add(TransactionDto transactionDto);
        ValueTask<DataWrapper<List<long>>> AddTransfer(TransferTransactionDto transfer);
        ValueTask<DataWrapper<List<TransactionDto>>> GetById(long id);
        ValueTask<DataWrapper<List<TransactionDto>>> GetByAccountId(long accountId);
        ValueTask<DataWrapper<List<TransactionDto>>> SearchTransactions(TransactionSearchParameters searchParameters);
        ValueTask<DataWrapper<BalanceDto>> GetBalanceByAccountId (long accountId);
        ValueTask UpdateCurrencyRates();
        ValueTask DeleteAllTransaction();
    }
}