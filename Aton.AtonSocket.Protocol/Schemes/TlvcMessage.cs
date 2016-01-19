using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aton.AtonSocket.Core;

namespace Aton.AtonSocket.Protocol.Schemes
{
 
    /// <summary>
    ///  default message Tlvc
    /// </summary>
    public class TlvcMessage : IMsg
    {
        public int Tag { get; set; }
        public int Length { get; set; }
        public byte[] Value { get; set; }
        public long Checksum { get; set; }

        public override byte[] SerializeToBytes()
        {
            Data = new byte[Value.Length+16];
            Array.Copy(BitConverter.GetBytes(Tag), 0, Data, 0, 4);
            Array.Copy(BitConverter.GetBytes(Length), 0, Data,4, 4);
            Array.Copy(Value, 0, Data, 8, 4);
            Array.Copy(BitConverter.GetBytes(Checksum), 0, Data, Length+8, 8);
            return Data;
        }
        public override void Reset()
        {
            Tag = 0;
            Length = 0;
            Checksum = 0;
            Array.Clear(Value, 0, Value.Length);
        }
    }
}
