namespace TransactionStore.API.Models.Input
{
    public class TransferInputModel : TransactionInputModel
    {
        public long AccountIdReceiver { get; set; }
        public byte ReceiverCurrencyId { get; set; }
    }
}