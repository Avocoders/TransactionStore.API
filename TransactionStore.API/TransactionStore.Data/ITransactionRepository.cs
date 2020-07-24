using System.Collections.Generic;
using TransactionStore.Data.DTO;

namespace TransactionStore.Data
{
    public interface ITransactionRepository
    {
        DataWrapper<long> Add(TransactionDto transactionDto);
        DataWrapper<List<long>> AddTransfer(TransferTransaction transfer);
        DataWrapper<List<TransactionDto>> GetById(long id);
        DataWrapper<List<TransactionDto>> GetByLeadId(long leadId);
        decimal GetTotalAmountInCurrency(long leadId, byte currency);
        DataWrapper<List<TransactionDto>> SearchTransactions(TransactionSearchParameters searchParameters);
    }
}