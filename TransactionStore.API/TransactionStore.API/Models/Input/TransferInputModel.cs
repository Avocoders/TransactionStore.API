namespace TransactionStore.API.Models.Input
{
    public class TransferInputModel : TransactionInputModel
    {
        public long LeadIdReceiver { get; set; }
    }
}