namespace BlazorDexie.Database.Transaction
{
    public class NextRequest
    {
        public string? LastCommandId { get; set; }
        public object? LastCommandResult { get; set; }
    }
}
