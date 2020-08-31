namespace TransactionStore.API.Models.Input
{
    public class TransactionInputModel
    {
        public long AccountId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal Amount { get; set; }
    }
}
