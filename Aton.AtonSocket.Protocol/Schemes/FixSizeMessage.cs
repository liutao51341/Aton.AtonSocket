using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aton.AtonSocket.Core;
using System.Collections;

namespace Aton.AtonSocket.Protocol.Schemes
{
    public class FixSizeMessage : IMsg
    {
        public FixSizeMessage(int fixSize)
        {
            Data = new byte[fixSize];
        }
        public override byte[] SerializeToBytes()
        {
            return Data;
        }
        public override void Reset()
        {
            Array.Clear(Data, 0, Data.Length);
        }
    }
}
