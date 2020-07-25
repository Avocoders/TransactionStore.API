namespace TransactionStore.Core.Shared
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdraw,
        Transfer
    }

    public enum TransactionCurrency
    {
        RUR = 1,
        USD,
        EUR
    }
}
