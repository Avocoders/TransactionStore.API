
namespace TransactionStore.API.Models.Output
{
    public class TransactionOutputModel
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Timestamp { get; set; }
        public long AccountIdReceiver { get; set; }
    }
}
