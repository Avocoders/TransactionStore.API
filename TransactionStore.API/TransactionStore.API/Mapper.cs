﻿using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using TransactionStore.Data;
using System.Collections.Generic;
using System;
using TransactionStore.Core.Shared;

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
                Currency = new TransactionCurrencyDto()
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
                Currency = new TransactionCurrencyDto()
                {
                    Id = withdraw.CurrencyId
                },
                Amount = -withdraw.Amount
            };
        }

        public TransferTransactionDto ConvertTransferInputModelToTransferTransactionDto(TransferInputModel transfer)
        {
            return new TransferTransactionDto()
            {
                LeadId = transfer.LeadId,
                Type = new TransactionTypeDto()
                {
                    Id = (byte)TransactionType.Transfer
                },
                Currency = new TransactionCurrencyDto()
                {
                    Id = transfer.CurrencyId
                },
                Amount = transfer.Amount,
                DestinationLeadId = transfer.DestinationLeadId
            };
        }

        public TransactionOutputModel ConvertTransferTransactionDtoToTransactionOutputModel(TransferTransactionDto transaction)
        {
            return new TransactionOutputModel()
            {
                TransientLeadId = transaction.DestinationLeadId,
                Id = transaction.Id ?? -1,
                LeadId = transaction.LeadId,
                Type = (string)Enum.GetName(typeof(TransactionType), transaction.Type.Id),
                Currency = (string)Enum.GetName(typeof(TransactionCurrency), transaction.Currency.Id),
                Amount = transaction.Amount,
                Timestamp = transaction.Timestamp.ToString("dd.MM.yyyy HH:mm:ss")
            }; 
        }

        public List<TransactionOutputModel> ConvertTransferTransactionDtosToTransactionOutputModel(List<TransferTransactionDto> transactions)
        {
            List<TransactionOutputModel> models = new List<TransactionOutputModel>();
            foreach(var dto in transactions)
            {
                models.Add(ConvertTransferTransactionDtoToTransactionOutputModel(dto));
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
                FromDate = Convert.ToDateTime(parameters.FromDate),
                TillDate = Convert.ToDateTime(parameters.TillDate)
            };
        }
    }
}
