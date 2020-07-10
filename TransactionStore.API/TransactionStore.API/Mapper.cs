﻿using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using System.Collections.Generic;
using TransactionStore.API.Shared;
using System;

namespace TransactionStore.API
{
    public class Mapper
    {
        public TransactionDto ConvertTransactionInputModelDepositToTransactionDto(TransactionInputModel deposit)
        {
            return new TransactionDto()
            {
                LeadId = deposit.LeadId,
                TypeId = (byte)Enums.TransactionType.Deposit,
                CurrencyId = deposit.CurrencyId,
                Amount = deposit.Amount
            };
        }

        public TransactionDto ConvertTransactionInputModelWithdrawToTransactionDto(TransactionInputModel withdraw)
        {
            return new TransactionDto()
            {
                LeadId = withdraw.LeadId,
                TypeId = (byte)Enums.TransactionType.Withdraw,
                CurrencyId = withdraw.CurrencyId,
                Amount = -withdraw.Amount
            };
        }

        public TransferTransactionDto ConvertTransferInputModelToTransferTransactionDto(TransferInputModel transfer)
        {
            return new TransferTransactionDto()
            {
                LeadId = transfer.LeadId,
                TypeId = (byte)Enums.TransactionType.Transfer,
                CurrencyId = transfer.CurrencyId,
                Amount = transfer.Amount,
                DestinationLeadId = transfer.DestinationLeadId
            };
        }

        public TransferOutputModel ConvertTransferTransactionDtoToTransferOutputModel(TransferTransactionDto transaction)
        {
            return new TransferOutputModel()
            {
                Id = transaction.Id ?? -1,
                LeadId = transaction.LeadId,
                TypeId = transaction.TypeId,
                CurrencyId = transaction.CurrencyId,
                Amount = transaction.Amount,
                Timestamp = transaction.Timestamp,
                DestinationLeadId = transaction.DestinationLeadId
            };
        }

        public List<TransferOutputModel> ConvertTransferTransactionDtosToTransferOutputModels(List<TransferTransactionDto> transactions)
        {
            List<TransferOutputModel> models = new List<TransferOutputModel>();
            foreach(var dto in transactions)
            {
                models.Add(ConvertTransferTransactionDtoToTransferOutputModel(dto));
            }
            return models;
        }
    }
}
