
namespace TransactionStore.API.Models.Output
{
    public class TransactionOutputModel
    {
        public long Id { get; set; }
        public long LeadId { get; set; }
        public string Type { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Timestamp { get; set; }
        public long LeadIdReceiver { get; set; }
    }
}
