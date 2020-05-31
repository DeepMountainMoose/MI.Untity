using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public class Producer:IDisposable
    {
        private readonly IConnection _connection;
        private readonly ILogger _logger;
        private readonly Timer _timer;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<IModel> _queue = new ConcurrentQueue<IModel>();
        private readonly ConcurrentDictionary<IModel, (TaskCompletionSource<bool>, DateTime)> _callbackList = new ConcurrentDictionary<IModel, (TaskCompletionSource<bool>, DateTime)>();
        private AlreadyClosedException _alreadyClosedException;
        private CancellationTokenSource _alreadyClosedSource;
        public string ExchangeName { get; }

        public Producer(IConnection connection,string exchangeName,int max,ILogger logger)
        {
            _connection = connection;
            _logger = logger;
            ExchangeName = exchangeName;
            _semaphore = new SemaphoreSlim(max, max);
            connection.RecoverySucceeded += Connection_RecoverySucceeded;
            connection.ConnectionShutdown += Connection_ConnectionShutdown;
            _timer = new Timer(state =>
            {
                var list = (ConcurrentDictionary<IModel, (TaskCompletionSource<bool>, DateTime)>)state;

                foreach (var producer in list.Keys.ToArray())
                {
                    if (list.TryGetValue(producer, out var value) &&
                        value.Item2.AddMinutes(10) < DateTime.Now)
                        TryComplete(CanBeLose, producer);
                }
            }, _callbackList, 60000, 60000);
        }

        public IModel Channel => Get(CancellationToken.None).GetAwaiter().GetResult();

        private static readonly WaitCallback ConnectionShutdown = state =>
        {
            var (tcs, e) = (Tuple<(TaskCompletionSource<bool>, DateTime), object>)state;

            tcs.Item1.TrySetException(new AlreadyClosedException((ShutdownEventArgs)e));
        };

        private static readonly WaitCallback BasicAcks = state => ((TaskCompletionSource<bool>)state).TrySetResult(true);

        private static readonly WaitCallback BasicNacks = state => ((TaskCompletionSource<bool>)state).TrySetResult(false);

        private static readonly WaitCallback CanBeLose = state => ((TaskCompletionSource<bool>)state).TrySetCanceled();

        public async Task<IModel> Get(CancellationToken cancellationToken)
        {
            try
            {
                var cts = _alreadyClosedSource;
                var ex = _alreadyClosedException;
                if(cts!=null)
                {
                    var tcs = new TaskCompletionSource<object>();
                    using (cts.Token.Register(() => tcs.TrySetResult(null)))
                    using (cancellationToken.Register(() => tcs.TrySetException(ex)))
                    {
                        await tcs.Task.ConfigureAwait(false);
                    }
                }

                await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                return _queue.TryDequeue(out var producer) ? producer : Create();
            }
            catch(OperationCanceledException)
            {
                throw new TimeoutException();
            }
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            var ex = _alreadyClosedException = new AlreadyClosedException(e);
            _alreadyClosedSource = new CancellationTokenSource();

            foreach (var producer in _callbackList.Keys.ToArray())
            {
                if(_callbackList.TryGetValue(producer,out _))
                    TryComplete(ConnectionShutdown, producer, ex);
            }
        }

        private void Connection_RecoverySucceeded(object sender, EventArgs e)
        {
            Interlocked.Exchange(ref _alreadyClosedSource, null)?.Cancel();
            _alreadyClosedException = null;
        }

        private void TryComplete(WaitCallback callback,IModel channel,object state=null)
        {
            if (!_callbackList.TryRemove(channel, out var tcs)) return;

            ThreadPool.QueueUserWorkItem(callback, state == null ? (object)tcs.Item1 : Tuple.Create(tcs, state));

            if (_disposed)
                channel.Dispose();
            else
            {
                _queue.Enqueue(channel);
                _semaphore.Release();
            }
        }

        private IModel Create()
        {
            var channel = _connection.CreateModel();
            channel.ConfirmSelect();

            channel.BasicAcks += (sender, e) => Channel_BasicAcks(channel, e);
            channel.BasicNacks += (sender, e) => Channel_BasicNacks(channel, e);

            return channel;
        }

        private static async Task<bool> Timeout(TaskCompletionSource<bool> tcs)
        {
            using (var cts = new CancellationTokenSource(6000))
            using (cts.Token.Register(() => tcs.TrySetCanceled(), false))
                return await tcs.Task.ConfigureAwait(false);
        }

        private void Channel_BasicAcks(object sender, BasicAckEventArgs e) => TryComplete(BasicAcks, (IModel)sender);

        private void Channel_BasicNacks(object sender, BasicNackEventArgs e) => TryComplete(BasicNacks, (IModel)sender);

        private async Task<bool> Send(Action<IModel> action,CancellationToken cancellationToken)
        {
            var producer = await Get(cancellationToken).ConfigureAwait(false);
            var (cts, _) = _callbackList[producer] = (new TaskCompletionSource<bool>(), DateTime.Now);

            var ex = _alreadyClosedException;
            if (ex != null)
                TryComplete(_ => cts.TrySetException(ex), producer);
            else
                action(producer);

            return await Timeout(cts).ConfigureAwait(false);
        }

        public Task<bool> Send(IEnumerable<(string routingKey, byte[] body, IBasicProperties basicProperties)> messages, CancellationToken cancellationToken) =>
            Send(producer =>
            {
                var basicPublishBatch = producer.CreateBasicPublishBatch();

                foreach (var (routingKey, body, basicProperties) in messages)
                    basicPublishBatch.Add(ExchangeName, routingKey, false, basicProperties, body);

                basicPublishBatch.Publish();
            }, cancellationToken);

        public Task<bool> Send(string routingKey, byte[] body, IBasicProperties basicProperties, CancellationToken cancellationToken) =>
            Send(producer => producer.BasicPublish(ExchangeName, routingKey, basicProperties, body), cancellationToken);

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _semaphore.Dispose();
            _timer.Dispose();

            _connection.ConnectionShutdown -= Connection_ConnectionShutdown;
            _connection.RecoverySucceeded -= Connection_RecoverySucceeded;

            while(_queue.TryDequeue(out var producer))
            {
                producer.Dispose();
            }

            foreach(var producer in _callbackList.Keys.ToArray())
            {
                TryComplete(CanBeLose, producer);
            }
        }
    }
}
