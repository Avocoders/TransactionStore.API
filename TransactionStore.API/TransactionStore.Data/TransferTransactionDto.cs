using TransactionStore.Data.DTO;

namespace TransactionStore.Data
{
    public class TransferTransactionDto : TransactionDto
    { 
        public byte ReceiverCurrencyId { get; set; }
    }
}