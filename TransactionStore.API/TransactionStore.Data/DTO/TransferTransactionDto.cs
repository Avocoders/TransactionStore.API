namespace TransactionStore.Data.DTO
{
    public class TransferTransactionDto : TransactionDto
    { 
        public long DestinationLeadId { get; set; }
    }
}