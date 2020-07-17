﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using TransactionStore.Data.DTO;
using Dapper;
using TransactionStore.Core.Shared;

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
            var result = new DataWrapper<long>();
            try
            {
                string sqlExpression = "Transaction_Add @leadId, @typeId, @currencyId, @amount";
                result.Data = _connection.Query<long>(sqlExpression, transactionDto).FirstOrDefault();
                result.IsOk = true;
            }

            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<List<TransferTransaction>> GetByLeadId(long leadId)
        {
            var result = new DataWrapper<List<TransferTransaction>>();
            try
            {
                string sqlExpression = "Transaction_GetByLeadId @leadId";
                result.Data = _connection.Query<TransferTransaction>(sqlExpression, new { leadId }).ToList();
                result.IsOk = true;
            }

            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<List<long>> AddTransfer(TransferTransaction transfer)
        {

            var result = new DataWrapper<List<long>>();
            try
            {
                string sqlExpression = "Transaction_AddTransfer @leadId, @typeId, @currencyId, @amount, @destinationLeadId";
                result.Data = _connection.Query<long>(sqlExpression, transfer).ToList();
                result.IsOk = true;
            }

            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<TransferTransaction> GetById(long id)
        {
            var result = new DataWrapper<TransferTransaction>();
            try
            {
                string sqlExpression = "Transaction_GetById @id";
                result.Data = _connection.Query<TransferTransaction>(sqlExpression, new { id }).FirstOrDefault();
                result.IsOk = true;
            }

            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<List<TransactionDto>> SearchTransactions(TransactionSearchParameters searchParameters)
        {
            var result = new DataWrapper<List<TransactionDto>>();
            try
            {
                var transactions = new List<TransactionDto>();
                string sqlExpression = "Transaction_Search @leadId, @type, @currency, @amount, @fromDate, @tillDate";
                var data = _connection.Query<TransactionDto, TransactionTypeDto, CurrencyDto, TransactionDto>(sqlExpression, 
                    (transaction, type, currency) =>
                    {
                        TransactionDto transactionEntry;

                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        transactionEntry.Currency = currency;
                        transactions.Add(transactionEntry);

                        return transactionEntry;
                    },
                    searchParameters,
                    splitOn: "Name").ToList();

                result.Data = LayoutTransactions(transactions).ToList();
                result.IsOk = true;
            }
            catch(Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        private List<TransactionDto> LayoutTransactions(List<TransactionDto> transactions)
        {
            List<TransactionDto> transactionsDto = new List<TransactionDto>();
            var nonTransferTransactions = transactions.Where(t => t.Type.Id != (byte)TransactionType.Transfer).ToList();
            var transferTransactions = transactions.Where(t => t.Type.Id == (byte)TransactionType.Transfer).ToList();
            List<TransferTransaction> transfers = new List<TransferTransaction>();
            foreach (var transfer in transferTransactions)
            {
                if (transfer.Amount < 0)
                {
                    var transferReceiver = transferTransactions.Where(t => t.Amount > 0 &&
                                                     t.Currency == transfer.Currency &&
                                                     t.Timestamp == transfer.Timestamp &&
                                                     t.Amount == Math.Abs(transfer.Amount)).FirstOrDefault();
                    transfers.Add(new TransferTransaction()
                    {
                        Id = transfer.Id,
                        LeadId = transfer.LeadId,
                        Type = transfer.Type,
                        Currency = transfer.Currency,
                        Amount = transfer.Amount,
                        Timestamp = transfer.Timestamp,
                        LeadIdReceiver = transferReceiver.LeadId
                    });
                }
            }
            transactionsDto = nonTransferTransactions;
            transactionsDto.AddRange(transfers);
            return transactionsDto;
        }

        public decimal GetTotalAmountInCurrency(long leadId, byte currency)

        {

            decimal balance=0;

            List<TransferTransaction> transactions = new List<TransferTransaction>();

            transactions = GetByLeadId(leadId).Data;

            foreach(var transaction in transactions)

            {

                if (currency == 1)

                {

                    if (transaction.Currency.Id == 2) transaction.Amount *= 71;

                    if (transaction.Currency.Id == 3) transaction.Amount *= 80;

                }

                

                if(currency == 2)

                {

                    if (transaction.Currency.Id == 1) transaction.Amount /= 71;

                    if (transaction.Currency.Id == 3) transaction.Amount *= (decimal)0.89;

                }

                if(currency == 3)

                {

                    if (transaction.Currency.Id == 1) transaction.Amount /= 80;

                    if (transaction.Currency.Id == 2) transaction.Amount *= (decimal)1.13;

                }



                balance += transaction.Amount;

            }

            return balance;

        }
    }
}
