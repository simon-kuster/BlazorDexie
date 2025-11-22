namespace BlazorDexie.Database.Transaction
{
    public enum TransactionBodyStatus
    {
        Running = 0,
        Finished = 1,
        Error = 2,
        Canceled = 3
    }
}
