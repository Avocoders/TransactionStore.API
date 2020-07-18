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

        public TransferTransaction ConvertTransferInputModelToTransferTransactionDto(TransferInputModel transfer)
        {
            return new TransferTransaction()
            {
                LeadId = transfer.LeadId,
                Type = new TransactionTypeDto()
                {
                    Id = (byte)TransactionType.Transfer
                },
                Currency = new CurrencyDto()
                {
                    Id = transfer.CurrencyId
                },
                Amount = transfer.Amount,
                LeadIdReceiver = transfer.DestinationLeadId
            };
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
                Amount = transaction.Amount,
                Timestamp = transaction.Timestamp.ToString("dd.MM.yyyy HH:mm:ss")
            }; 
        }

        public List<TransactionOutputModel> ConvertTransferTransactionsToTransactionOutputModel(List<TransferTransaction> transactions)
        {
            List<TransactionOutputModel> models = new List<TransactionOutputModel>();
            foreach(var dto in transactions)
            {
                models.Add(ConvertTransferTransactionToTransactionOutputModel(dto));
            }
            return models;
        }

        public TransactionSearchParameters ConvertSearchParametersInputModelToTransactionSearchParameters(SearchParametersInputModel parameters)
        {
            return new TransactionSearchParameters()
            {
                LeadId = parameters.LeadId,
                Type = parameters.Type,
                Currency = parameters.Currency,
                Amount = parameters.Amount,
                FromDate = string.IsNullOrEmpty(parameters.FromDate)? null : (DateTime?)DateTime.ParseExact(parameters.FromDate, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                TillDate = string.IsNullOrEmpty(parameters.TillDate) ? null : (DateTime?)DateTime.ParseExact(parameters.TillDate, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture)
            };
        }


        public TransactionOutputModel ConvertTransactionDtoToTransactionOutputModelForSearch(TransactionDto transactionDto)
        {
            return new TransactionOutputModel()
            {
                Id = transactionDto.Id ?? -1,
                LeadId = transactionDto.LeadId,
                Type = transactionDto.Type.Name,
                Currency = transactionDto.Currency.Name,
                Amount = transactionDto.Amount,
                Timestamp = transactionDto.Timestamp.ToString("dd.MM.yyyy HH:mm:ss")
            };
        }

        public List<TransactionOutputModel> ConvertTransactionDtosToTransactionOutputModelsForSearch(List<TransactionDto> transactions)
        {
            List<TransactionOutputModel> models = new List<TransactionOutputModel>();
            foreach (var transaction in transactions)
            {
                if (transaction.GetType() == typeof(TransferTransaction))
                {
                    models.Add(ConvertTransferTransactionToTransactionOutputModelForSearch((TransferTransaction)transaction));
                }
                else
                {
                    models.Add(ConvertTransactionDtoToTransactionOutputModelForSearch(transaction));
                }


            }
            return models;
        }


        public TransactionOutputModel ConvertTransferTransactionToTransactionOutputModelForSearch(TransferTransaction transaction)
        {
            return new TransactionOutputModel()
            {
                LeadIdReceiver = transaction.LeadIdReceiver,
                Id = transaction.Id ?? -1,
                LeadId = transaction.LeadId,
                Type = transaction.Type.Name,
                Currency = transaction.Currency.Name,
                Amount = transaction.Amount,
                Timestamp = transaction.Timestamp.ToString("dd.MM.yyyy HH:mm:ss")
            };
        }
    }
}
