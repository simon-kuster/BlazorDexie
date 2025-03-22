using BlazorDexie.Definitions;
using Microsoft.JSInterop;

namespace BlazorDexie.Database
{
    public interface IDb
    {
        string DatabaseName { get; }
        IJSObjectReference? DbJsReference { get; }
        int VersionNumber { get; }
        List<DbVersionDefinition> Versions { get; }

        Task Close(CancellationToken cancellationToken = default);
        Task Delete(CancellationToken cancellationToken = default);
        ValueTask DisposeAsync();
        Task Init(CancellationToken cancellationToken);
        Task Transaction(string mode, string[] storeNames, int timeout, Func<Task> transactionBody, CancellationToken cancellationToken = default);
    }
}