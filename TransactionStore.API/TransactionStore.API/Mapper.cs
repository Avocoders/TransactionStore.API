using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using TransactionStore.Data;
using System.Collections.Generic;
using System;
using TransactionStore.Core.Shared;
using System.Globalization;
using NUnit.Framework;

namespace TransactionStore.API
{
    public class Mapper
    {
        public TransactionDto ConvertTransactionInputModelDepositToTransactionDto(TransactionInputModel deposit)
        {
            return new TransactionDto()
            {
                LeadId = deposit.LeadId,
                Type = new TransactionTypeDto()
                {
                    Id = (byte)TransactionType.Deposit
                },
                Currency = new CurrencyDto()
                {
                    Id = deposit.CurrencyId
                },
                Amount = deposit.Amount
            };
        }

        public TransactionDto ConvertTransactionInputModelWithdrawToTransactionDto(TransactionInputModel withdraw)
        {
            return new TransactionDto()
            {
                LeadId = withdraw.LeadId,
                Type = new TransactionTypeDto()
                {
                    Id = (byte)TransactionType.Withdraw
                },
                Currency = new CurrencyDto()
                {
                    Id = withdraw.CurrencyId
                },
                Amount = -withdraw.Amount
            };
        }

        public TransferTransaction ConvertTransferInputModelToTransferTransaction(TransferInputModel transfer)
        {
            return new TransferTransaction()
            {
                LeadId = transfer.LeadId,
                Amount = transfer.Amount,
                LeadIdReceiver = transfer.LeadIdReceiver,
                Currency = new CurrencyDto()
                {
                    Id = transfer.CurrencyId
                }
            };
        }

        public TransactionSearchParameters ConvertSearchParametersInputModelToTransactionSearchParameters(SearchParametersInputModel parameters)
        {
            return new TransactionSearchParameters()
                {
                    LeadId = parameters.LeadId,
                    TypeId = parameters.Type,
                    CurrencyId = parameters.Currency,
                    AmountBegin = parameters.Type == (byte)TransactionType.Deposit ? parameters.AmountBegin : -parameters.AmountEnd,
                    AmountEnd = parameters.Type == (byte)TransactionType.Deposit ? parameters.AmountEnd : -parameters.AmountBegin,
                    FromDate = string.IsNullOrEmpty(parameters.FromDate) ? null : (DateTime?)DateTime.ParseExact(parameters.FromDate, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    TillDate = string.IsNullOrEmpty(parameters.TillDate) ? null : (DateTime?)DateTime.ParseExact(parameters.TillDate, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                };
            
        }


        public TransactionOutputModel ConvertTransactionDtoToTransactionOutputModel(TransactionDto transactionDto)
        {
            return new TransactionOutputModel()
            {
                Id = transactionDto.Id ?? -1,
                LeadId = transactionDto.LeadId,
                Type = (string)Enum.GetName(typeof(TransactionType), transactionDto.Type.Id),
                Currency = (string)Enum.GetName(typeof(TransactionCurrency), transactionDto.Currency.Id),
                Amount = transactionDto.Type.Id == (byte)TransactionType.Deposit ? transactionDto.Amount : -transactionDto.Amount,
                Timestamp = transactionDto.Timestamp.ToString("dd.MM.yyyy HH:mm:ss")
            };
        }

        public List<TransactionOutputModel> ConvertTransactionDtosToTransactionOutputModels(List<TransactionDto> transactions)
        {
            List<TransactionOutputModel> models = new List<TransactionOutputModel>();
            foreach (var transaction in transactions)
            {
                if (transaction.GetType() == typeof(TransferTransaction))
                {
                    models.Add(ConvertTransferTransactionToTransactionOutputModel((TransferTransaction)transaction));
                }
                else
                {
                    models.Add(ConvertTransactionDtoToTransactionOutputModel(transaction));
                }


            }
            return models;
        }


        public TransactionOutputModel ConvertTransferTransactionToTransactionOutputModel(TransferTransaction transaction) 
        {
            return new TransactionOutputModel()
            {
                LeadIdReceiver = transaction.LeadIdReceiver,
                Id = transaction.Id ?? -1,
                LeadId = transaction.LeadId,
                Type = (string)Enum.GetName(typeof(TransactionType), transaction.Type.Id),
                Currency = (string)Enum.GetName(typeof(TransactionCurrency), transaction.Currency.Id),
                Amount = transaction.Type.Id == (byte)TransactionType.Deposit ? transaction.Amount : -transaction.Amount,
                Timestamp = transaction.Timestamp.ToString("dd.MM.yyyy HH:mm:ss")
            };
        }
    }
}
