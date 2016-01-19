using System;
using System.Collections.Concurrent;
using System.Net.Sockets;


namespace Aton.AtonSocket
{
    /// <summary>
    /// SocketAsyncEventArgs pool class
    /// </summary>
    internal class SocketAsyncEventArgsPool : IDisposable
    {
        /// <summary>
        /// SAEA Stack Pool
        /// </summary>
        ConcurrentStack<SocketAsyncEventArgs> m_Pool;

        public int BufferSize { get;set; }

        internal SocketAsyncEventArgsPool(Int32 capacity)
        {
            Capacity = capacity;
            m_Pool = new ConcurrentStack<SocketAsyncEventArgs>();
        }

        internal void Initize(BufferManager bufferManager)
        {
            for (int i = 0; i < Capacity; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                if (bufferManager != null)
                {
                    bufferManager.SetBuffer(args);
                    SocketAsyncToken socketAsyncToken = new SocketAsyncToken();
                    socketAsyncToken.OriginOffset = args.Offset;
                    socketAsyncToken.OffsetDelta = args.Offset;
                    socketAsyncToken.BufferSize = args.Buffer.Length/Capacity;
                    socketAsyncToken.TokenID = string.Empty;
                    args.UserToken = socketAsyncToken;
                    BufferSize = socketAsyncToken.BufferSize;
                }
                Push(args);
            }
            
        }

        internal Int32 Count
        {
            get { return m_Pool.Count; }
        }

        internal Int32 Capacity
        {
            get { return _Capacity; }
            private set { _Capacity = value; }
        }
        private Int32 _Capacity;

        internal SocketAsyncEventArgs Pop()
        {
            SocketAsyncEventArgs args;
            if (m_Pool.TryPop(out args))
            {
                return args;
            }
            return null;
        }

        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("socketasynceventargs can not push into pool, beacause it is null");
            }
            m_Pool.Push(item);
        }

        public void Dispose()
        {
            m_Pool.Clear();
        }
    }
}
