namespace TransactionStore.API.Models.Output
{
    public class TransferOutputModel : TransactionOutputModel
    {
        public long DestinationLeadId { get; set; }
    }
}