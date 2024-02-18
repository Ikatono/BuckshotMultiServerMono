using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuckshotMultiServerMono.Objects
{
    /// <summary>
    /// Provides an interface to communicate with players that persists past
    /// individual websocket connections. This should 
    /// </summary>
    public partial class Player
    {
        /// <summary>
        /// A message to be sent to the client
        /// </summary>
        public class PlayerTransmitMessage
        {
            public ReadOnlyMemory<byte> GetMessageData()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// A message received from the client
        /// </summary>
        public class PlayerReceiveMessage
        {
            public static PlayerReceiveMessage? Parse(ReadOnlySpan<byte> data)
            {
                return JsonSerializer.Deserialize<PlayerReceiveMessage>(data);
            }
        }
        /// <summary>
        /// The player's name and identifier
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Wraps a websocket connection to the client
        /// </summary>
        private PlayerConnection? Connection;
        public event EventHandler<PlayerReceiveMessage>? MessageReceivedEvent;
        public event EventHandler<PlayerTransmitMessage>? MessageTransmittedEvent;
        public event EventHandler? ConnectionOpenedEvent;
        public event EventHandler? ConnectionClosedEvent;
        public Player(string name)
        {
            Name = name;
        }
        /// <summary>
        /// attach a new websocket
        /// </summary>
        /// <param name="socket"></param>
        public void AddConnection(WebSocket socket)
        {
            if (Connection is not null)
            {
                Connection.Close();
                Connection = null;
            }
            Connection = new PlayerConnection(socket);
            Connection.ConnectionCloseEvent += (_, args) =>
                this.ConnectionClosedEvent?.Invoke(this, args);
            ConnectionOpenedEvent?.Invoke(this, EventArgs.Empty);
        }
        public bool Connected()
        {
            var state = Connection?.Socket.State;
            return (state == WebSocketState.Open || state == WebSocketState.Connecting);
        }

        internal void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}
