namespace TransactionStore.API.Models.Input
{
    public class TransferInputModel : TransactionInputModel
    {
        public long DestinationLeadId { get; set; }
    }
}