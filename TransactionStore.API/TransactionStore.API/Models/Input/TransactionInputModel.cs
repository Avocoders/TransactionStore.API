namespace TransactionStore.API.Models.Input
{
    public class TransactionInputModel
    {
        public long LeadId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal Amount { get; set; }
    }
}
