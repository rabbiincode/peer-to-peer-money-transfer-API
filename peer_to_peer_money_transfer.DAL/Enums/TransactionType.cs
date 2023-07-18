namespace peer_to_peer_money_transfer.DAL.Enums
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdrawal,
        Transfer,
        Credit,
	    Debit,
        Airtime
    }

    public static class TransactionTypeExtension
    {
        public static string? GetStringValue(this TransactionType transactionType)
        {
            return transactionType switch
            {
                TransactionType.Deposit => "Deposit",
                TransactionType.Withdrawal => "Withdrawal",
                TransactionType.Transfer => "Transfer",
                TransactionType.Credit => "Credit",
                TransactionType.Debit => "Debit",
                TransactionType.Airtime => "Airtime",
                _ => null
            };
        }
    }
}
