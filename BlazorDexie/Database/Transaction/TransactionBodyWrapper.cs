using BlazorDexie.JsInterop;
using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Text.Json;

namespace BlazorDexie.Database.Transaction
{
    public class TransactionBodyWrapper
    {
        private readonly Func<Task> _transactionBody;
        private Task? _runningTask;
        private CancellationToken _cancellationToken;
        private readonly ConcurrentQueue<ITransactionCommand> _commandQueue = new();
        private ConcurrentDictionary<string, TransactionCommand> _commandDictionary = new ConcurrentDictionary<string, TransactionCommand>();

        public TransactionBodyWrapper(Func<Task> transactionBody, CancellationToken cancellationToken)
        {
            _transactionBody = transactionBody;
            _cancellationToken = cancellationToken;
        }

        [JSInvokable]
        public async Task CallTransactionBody()
        {
            await _transactionBody();
        }

        [JSInvokable]
        public void StartTransactionBody()
        {
            _runningTask = _transactionBody();
        }

        [JSInvokable]
        public NextResponse Next(NextRequest nextRequest)
        {
            if (nextRequest.LastCommandId != null &&
                _commandDictionary.TryGetValue(nextRequest.LastCommandId, out TransactionCommand? lastCommand) &&
                lastCommand != null)
            {
                var test = lastCommand.Tcs.TrySetResult(nextRequest.LastCommandResult);
            }

            var status = GetTransactionBodyStatus();

            if (status == TransactionBodyStatus.Running && _commandQueue.TryDequeue(out ITransactionCommand? command))
            {
                return new NextResponse(status, command, null);
            }

            var errorMessage = _runningTask?.Status == TaskStatus.Faulted ? _runningTask.Exception?.InnerException?.Message : null;
            return new NextResponse(status, null, errorMessage);
        }

        public async Task<TRet> Execute<TRet>(string storeName, List<Command> commands, CancellationToken cancellationToken)
        {
            var command = new TransactionCommand(TransactionCommandType.Execute, storeName, commands);
            
            _commandDictionary.AddOrUpdate(
                command.Id, 
                command, 
                (id, c) => { throw new InvalidOperationException($"Command with Id {id} alreay exists!"); });
            
            _commandQueue.Enqueue(command);
            
            var result = await command.Tcs.Task 
                ?? throw new InvalidOperationException("No value is returned from JavaScript");
            
            return ((JsonElement)result).Deserialize<TRet>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                ?? throw new InvalidOperationException("Value returned from JavaScript cannot be deserialized");
        }

        public async Task ExecuteNonQuery(string storeName, List<Command> commands, CancellationToken cancellationToken)
        {
            var command = new TransactionCommand(TransactionCommandType.ExecuteNonQuery, storeName, commands);

            _commandDictionary.AddOrUpdate(
                command.Id, 
                command, 
                (id, c) => { throw new InvalidOperationException($"Command with Id {id} alreay exists!"); });

            _commandQueue.Enqueue(command);
            await command.Tcs.Task;
        }

        private TransactionBodyStatus GetTransactionBodyStatus()
        {
            if( _runningTask == null)
            {
                throw new InvalidOperationException("Transaction was not started");
            }

            var status = _runningTask.Status switch
            {
                TaskStatus.RanToCompletion => TransactionBodyStatus.Finished,
                TaskStatus.Faulted => TransactionBodyStatus.Error,
                TaskStatus.Canceled => TransactionBodyStatus.Canceled,
                _ => TransactionBodyStatus.Running,
            };

            if (_cancellationToken.IsCancellationRequested)
            {
                status = TransactionBodyStatus.Canceled;
            }

            return status;
        }
    }
}
