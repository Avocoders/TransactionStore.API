using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using TransactionStore.Data.DTO;
using Dapper;

namespace TransactionStore.Data
{
    public class TransactionRepository
    {
        private readonly IDbConnection _connection;
        public TransactionRepository()
        {
            _connection = Connection.GetConnection();
        }
        
        public DataWrapper<long> Add(TransactionDto transactionDto) // дженерик с OK, ErrorMessage, Data
        {
            try
            {
                string sqlExpression = "Transaction_Add @leadId, @typeId, @currencyId, @amount";
                return new DataWrapper<long>()
                {
                    Data = _connection.Query<long>(sqlExpression, transactionDto).FirstOrDefault(),
                    IsOk = true
                };
            }

            catch (Exception e)
            {
                return new DataWrapper<long>()
                {
                    ExceptionMessage = e.Message
                };
            }
        }

        public DataWrapper<List<TransferTransactionDto>> GetByLeadId(long leadId)
        {
            try
            {
                string sqlExpression = "Transaction_GetByLeadId @leadId";
                return new DataWrapper<List<TransferTransactionDto>>()
                {
                    Data = _connection.Query<TransferTransactionDto>(sqlExpression, new { leadId }).ToList(),
                    IsOk = true
                };
            }

            catch (Exception e)
            {
                return new DataWrapper<List<TransferTransactionDto>>()
                {
                    ExceptionMessage = e.Message
                };
            }
        }

        public DataWrapper<List<long>> AddTransfer(TransferTransactionDto transfer)
        {
            try
            {
                string sqlExpression = "Transaction_AddTransfer @leadId, @typeId, @currencyId, @amount, @destinationLeadId";
                return new DataWrapper<List<long>>()
                {
                    Data = _connection.Query<long>(sqlExpression, transfer).ToList(),
                    IsOk = true
                };
            }

            catch (Exception e)
            {
                return new DataWrapper<List<long>>()
                {
                    ExceptionMessage = e.Message
                };
            }
        }

        public DataWrapper<TransferTransactionDto> GetById(long id)
        {
            try
            {
                string sqlExpression = "Transaction_GetById @id";
                return new DataWrapper<TransferTransactionDto>()
                {
                    Data = _connection.Query<TransferTransactionDto>(sqlExpression, new { id }).FirstOrDefault(),
                    IsOk = true
                };
            }

            catch (Exception e)
            {
                return new DataWrapper<TransferTransactionDto>()
                {
                    ExceptionMessage = e.Message
                };
            }
        }       
        
        public DataWrapper<List<TransferTransactionDto>> GetSearchByTransaction()

        public decimal GetTotalAmountInCurrency(long leadId, byte currency)
        {
            decimal balance=0;
            List<TransferTransactionDto> transactions = new List<TransferTransactionDto>();
            transactions = GetByLeadId(leadId).Data;
            foreach(var transaction in transactions)
            {
                if (currency == 1)
                {
                    if (transaction.CurrencyId == 2) transaction.Amount *= 71;
                    if (transaction.CurrencyId == 3) transaction.Amount *= 80;
                }
                
                if(currency == 2)
                {
                    if (transaction.CurrencyId == 1) transaction.Amount /= 71;
                    if (transaction.CurrencyId == 3) transaction.Amount *= (decimal)0.89;
                }
                if(currency == 3)
                {
                    if (transaction.CurrencyId == 1) transaction.Amount /= 80;
                    if (transaction.CurrencyId == 2) transaction.Amount *= (decimal)1.13;
                }

                balance += transaction.Amount;
            }
            return balance;
        }
        /*public DataWrapper<List<TransferTransactionDto>> GetTransactionByLeadIdAndRange(long leadId, RangeDateDto rangeDate)
        {
            try
            {
                DataWrapper<List<TransferTransactionDto>> transactions = GetByLeadId(leadId);
                DataWrapper<List <TransferTransactionDto>> range = new DataWrapper<List<TransferTransactionDto>>();
                *//*foreach (var transact in transactions)
                {
                    if (transact.Timestamp >= rangeDate.FromDate && transact.Timestamp <= rangeDate.TillDate) range.Add(transact);
                }*//*
                return range;
            }
            catch(Exception e)
            {
                return new DataWrapper<List<TransferTransactionDto>>()
                {
                    ExceptionMessage = e.Message
                };
            }

        }*/
    }
}
