using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.Filter;
using Aton.AtonSocket.Core.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Aton.AtonSocket
{
    internal class UdpSocketAsyncSender
    {
        SocketAsyncEventArgs m_socketArgs;

        ISocketServer m_server;

        Socket m_socket;

        int m_BufferSize;

        byte[] m_singleBuffer;

        public event EventHandler<SocketAsyncEventArgs> DataSent;

        /// <summary>
        /// 最大客户端数
        /// </summary>
        /// <param name="numClient"></param>
        public UdpSocketAsyncSender(Socket socket, ISocketServer server)
        {
            m_socket = socket;
            m_server = server;
            m_BufferSize = (server as UdpSocketAsyncServer).BufferSize;

            m_singleBuffer = new byte[m_BufferSize];

            m_socketArgs = new SocketAsyncEventArgs();
            m_socketArgs.SetBuffer(m_singleBuffer, 0, 0);
            m_socketArgs.Completed += socketArgs_Completed;
        }

        public bool SendSync(byte[] content, EndPoint remoteEndPoint)
        {
            int result = m_socket.SendTo(content, remoteEndPoint);
            return result == content.Length;
        }

        public bool SendAsync(byte[] content, EndPoint remoteEndPoint)
        {
            m_socketArgs.RemoteEndPoint = remoteEndPoint;

            if (m_BufferSize < content.Length)//buffer size not enough
            {
                List<ArraySegment<byte>> m_multiBuffer = new List<ArraySegment<byte>>();
                int copyedBytes = 0;

                while (copyedBytes < content.Length)
                {
                    byte[] b = new byte[m_BufferSize];

                    if (content.Length - copyedBytes > m_BufferSize)
                    {
                        Buffer.BlockCopy(content, copyedBytes, b, 0, m_BufferSize);
                        copyedBytes += m_BufferSize;
                    }
                    else
                    {
                        Buffer.BlockCopy(content, copyedBytes, b, 0, content.Length - copyedBytes);
                        copyedBytes = content.Length;
                    }
                    m_multiBuffer.Add(new ArraySegment<byte>(b));
                }


                m_socketArgs.SetBuffer(null, 0, 0);
                m_socketArgs.BufferList = m_multiBuffer;

                if (!m_socket.SendToAsync(m_socketArgs))
                {
                    ProcessSent(m_socketArgs);
                }
            }
            else
            {
                Array.Clear(m_singleBuffer, 0, m_BufferSize);
                m_socketArgs.SetBuffer(m_singleBuffer, 0, content.Length);
                Buffer.BlockCopy(content, 0, m_socketArgs.Buffer, 0, content.Length);
                if (!m_socket.SendToAsync(m_socketArgs))
                {
                    ProcessSent(m_socketArgs);
                }
            }

            return true;
        }

        private void socketArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.SendTo:
                    ProcessSent(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a send");
            }
        }

        private void ProcessSent(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (DataSent != null)
                {
                    DataSent(m_socket, e);
                }
            }
        }
    }
}
