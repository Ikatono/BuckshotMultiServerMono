using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace BuckshotMultiServerMono.Objects
{
    public partial class Player
    {
        private class PlayerConnection : IDisposable
        {
            public static readonly TimeSpan EndThreadTimeout = TimeSpan.FromSeconds(5);

            internal readonly WebSocket Socket;
            internal readonly BlockingCollection<PlayerTransmitMessage> Queue = [];
            internal readonly Thread TransmitThread;
            internal readonly Thread ReceiveThread;
            internal readonly CancellationTokenSource CancelTokenSource = new();
            private bool disposedValue;

            public event EventHandler<PlayerTransmitMessage>? TransmitEvent;
            public event EventHandler<PlayerReceiveMessage>? ReceiveEvent;
            public event EventHandler? ConnectionCloseEvent;
            public PlayerConnection(WebSocket socket)
            {
                Socket = socket;
                TransmitThread = new Thread(TransmitFunc);
                TransmitThread.Start();
                ReceiveThread = new Thread(ReceiveFunc);
                ReceiveThread.Start();
            }
            private void TransmitFunc()
            {
                try
                {
                    while (!CancelTokenSource.IsCancellationRequested)
                    {
                        var message = Queue.Take(CancelTokenSource.Token);
                        if (message is not null)
                        {
                            Socket.SendAsync(message.GetMessageData(), WebSocketMessageType.Text, true, CancelTokenSource.Token)
                                .AsTask().Wait();
                            TransmitEvent?.Invoke(this, message);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    //this is expected, no need to act on it
                }
            }
            private void ReceiveFunc()
            {
                var buffer = new Memory<byte>();
                while (!CancelTokenSource.IsCancellationRequested)
                {
                    List<byte> message = [];
                    ValueWebSocketReceiveResult res;
                    do
                    {
                        var task = Socket.ReceiveAsync(buffer, CancelTokenSource.Token).AsTask();
                        task.Wait();
                        if (task.Exception?.InnerException is OperationCanceledException)
                            return;
                        res = task.Result;
                        message.AddRange(buffer.Span);
                        if (res.MessageType == WebSocketMessageType.Close)
                        {
                            Close();
                            return;
                        }
                    } while (!res.EndOfMessage);
                    var sendMessage = PlayerReceiveMessage.Parse(message.ToArray());
                    if (sendMessage is not null)
                        ReceiveEvent?.Invoke(this, sendMessage);
                }
            }
            public void EnqueueMessage(PlayerTransmitMessage message)
            {
                Queue.Add(message);
            }
            public void Close()
            {
                CancelTokenSource.Cancel();
                ConnectionCloseEvent?.Invoke(this, EventArgs.Empty);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        Close();
                    }
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
