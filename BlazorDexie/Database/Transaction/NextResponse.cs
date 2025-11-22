namespace BlazorDexie.Database.Transaction
{
    public class NextResponse(TransactionBodyStatus transactionBodyStatus, ITransactionCommand? nextCommand, string? transactionBodyErrorMessage)
    {
        public TransactionBodyStatus TransactionBodyStatus { get; } = transactionBodyStatus;
        public string? TransactionBodyErrorMessage { get; } = transactionBodyErrorMessage;
        public ITransactionCommand? NextCommand { get; } = nextCommand;
    }
}
