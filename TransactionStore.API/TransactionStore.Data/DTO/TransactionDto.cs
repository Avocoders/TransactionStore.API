﻿using System;

namespace TransactionStore.Data.DTO
{
    public class TransactionDto
    {
        public long? Id { get; set; }
        public long LeadId { get; set; }
        public TransactionTypeDto Type { get; set; }
        public TransactionCurrencyDto Currency { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}