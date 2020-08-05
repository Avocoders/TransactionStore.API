using TransactionStore.Data.DTO;

namespace TransactionStore.Data
{
    public class TransferTransaction : TransactionDto
    { 
        public long AccountIdReceiver { get; set; }
    }
}