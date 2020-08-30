using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using TransactionStore.Data.DTO;
using Dapper;
using TransactionStore.Core.Shared;
using System.Data.SqlClient;
using TransactionStore.Core;
using Microsoft.Extensions.Options;
using Messaging;
using System.Threading.Tasks;

namespace TransactionStore.Data
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnection _connection;
        private readonly Currencies _currencies;

        public TransactionRepository(IOptions<StorageOptions> options, Currencies currencies)
        {
            _connection = new SqlConnection(options.Value.DBConnectionString);
            _currencies = currencies;
        }

        public async ValueTask<DataWrapper<long>> Add(TransactionDto transactionDto) 
        {
            var balance = await GetBalanceByAccountId(transactionDto.AccountId);
            if (balance.Data != null)
                transactionDto.Timestamp = balance.Data.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
            transactionDto.ExchangeRates = await GetRates(transactionDto.Currency.Id.Value);
            var result = new DataWrapper<long>();
            try
            {   
                var tmp = await _connection.QueryAsync<long>(StoredProcedures.AddTransaction,
                    new
                    {
                        accountId = transactionDto.AccountId,
                        typeId = transactionDto.Type.Id,
                        currencyId = transactionDto.Currency.Id,
                        amount = transactionDto.Amount,
                        exchangeRates = transactionDto.ExchangeRates,
                        timestamp = transactionDto.Timestamp
                    }, commandType: CommandType.StoredProcedure);
                result.Data = tmp.FirstOrDefault();
                result.IsOk = true;
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("Error 50001"))                
                    result.ExceptionMessage = "The operation was rejected";                
                else                
                    result.ExceptionMessage = e.Message;
            }
            return result;
        }
        public async ValueTask<DataWrapper<List<long>>> AddTransfer(TransferTransactionDto transfer)
        {
            var balance = await GetBalanceByAccountId(transfer.AccountId);
            if (balance.Data != null)
                transfer.Timestamp = balance.Data.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
            var exchangeRates1 = await GetRates(transfer.Currency.Id.Value);
            var exchangeRates2 = await GetRates(transfer.ReceiverCurrencyId);

            var result = new DataWrapper<List<long>>();
            try
            {
                var tmp = await _connection.QueryAsync<long>(StoredProcedures.AddTranfer,
                    new
                    {
                        accountId = transfer.AccountId,
                        typeId = transfer.Type.Id,
                        currencyId = transfer.Currency.Id,
                        amount1 = transfer.Amount,
                        amount2 = transfer.Amount / exchangeRates1 * exchangeRates2,
                        accountIdReceiver = transfer.AccountIdReceiver,
                        receiverCurrencyId = transfer.ReceiverCurrencyId,
                        exchangeRates1,
                        exchangeRates2,
                        timestamp = transfer.Timestamp

                    }, commandType: CommandType.StoredProcedure);
                result.Data = tmp.ToList();
                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }
        public async ValueTask<DataWrapper<List<TransactionDto>>> GetById(long id)
        {
            var result = new DataWrapper<List<TransactionDto>>();
            try
            {
                var tmp = await _connection.QueryAsync<TransactionDto, TransactionTypeDto, TransactionDto>
                    (StoredProcedures.GetTransactionById,
                    (transaction, type) =>
                    {
                        TransactionDto transactionEntry;
                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        return transactionEntry;
                    },
                    new { id }, commandType: CommandType.StoredProcedure);
                result.Data = tmp.ToList();
                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }
        public async ValueTask<DataWrapper<List<TransactionDto>>> GetByAccountId(long accountId)
        {
            var result = new DataWrapper<List<TransactionDto>>();
            try
            {
                var tmp = await _connection.QueryAsync<TransactionDto, TransactionTypeDto, TransactionDto>
                    (StoredProcedures.GetTransactionByAccountId,
                    (transaction, type) =>
                    {
                        TransactionDto transactionEntry;
                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        return transactionEntry;
                    },
                    new { accountId },
                    splitOn: "id", commandType: CommandType.StoredProcedure);
                result.Data = tmp.ToList();
                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }
        public async ValueTask<DataWrapper<List<TransactionDto>>> SearchTransactions(TransactionSearchParameters searchParameters)
        {
            var result = new DataWrapper<List<TransactionDto>>();
            try
            {
                var tmp = await _connection.QueryAsync<TransactionDto, TransactionTypeDto, CurrencyDto, TransactionDto>
                    (StoredProcedures.SearchTransaction,
                    (transaction, type, currency) =>
                    {
                        TransactionDto transactionEntry;

                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        transactionEntry.Currency = currency;

                        return transactionEntry;
                    },
                    searchParameters,
                    splitOn: "id", commandType: CommandType.StoredProcedure);
                result.Data = tmp.ToList();

                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public async ValueTask<DataWrapper<BalanceDto>> GetBalanceByAccountId(long accountId)
        {
            var result = new DataWrapper<BalanceDto>();
            try
            {
                var tmp = await _connection.QueryAsync<BalanceDto>(StoredProcedures.GetBalanceByAccountId, 
                    new { accountId }, commandType: CommandType.StoredProcedure);
                result.Data = tmp.FirstOrDefault();
                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public async ValueTask UpdateCurrencyRates()
        {
            foreach (var c in _currencies.Rates)
            {              
                await _connection.ExecuteAsync(StoredProcedures.UpdateCurrencyRates,
                    new
                    {
                        code = c.Code,
                        rate = c.Rate
                    }, commandType: CommandType.StoredProcedure);
            }
        }

        public async ValueTask<decimal> GetRates(byte currencyId)
        {
            string code = Enum.GetName(typeof(TransactionCurrency), currencyId);
            var currency = _currencies.Rates?.Where(t => t.Code == code).FirstOrDefault();
            if (currency != null)
                return currency.Rate;
            else
            {
                var tmp = await _connection.QueryAsync<decimal>(StoredProcedures.GetCurrencyRateById, 
                    new { id = currencyId }, commandType: CommandType.StoredProcedure);
                return tmp.FirstOrDefault();
            }
        }
    }
}
