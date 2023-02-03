﻿using System;
using GTA;
using Lidgren.Network;
using RageCoop.Core.Scripting;

namespace RageCoop.Core
{
    internal partial class Packets
    {
        internal class CustomEvent : Packet
        {
            public CustomEventFlags Flags;

            public CustomEvent(CustomEventFlags flags = CustomEventFlags.None)
            {
                Flags = flags;
            }

            public override PacketType Type => PacketType.CustomEvent;
            public int Hash { get; set; }
            public byte[] Payload;
            public object[] Args;

            protected override void Serialize(NetOutgoingMessage m)
            {
                m.Write((byte)Flags);
                m.Write(Hash);
                if (Args != null)
                {
                    lock (SharedWriter)
                    {
                        SharedWriter.Reset();
                        CustomEvents.WriteObjects(SharedWriter, Args);
                        Payload = SharedWriter.ToByteArray(SharedWriter.Position);
                    }
                }
                m.Write(Payload);
            }

            public unsafe override void Deserialize(NetIncomingMessage m)
            {
                Flags = (CustomEventFlags)m.ReadByte();
                Hash = m.ReadInt32();
                Payload = m.ReadBytes(m.LengthBytes - m.PositionInBytes);
                fixed (byte* p = Payload)
                {
                    lock (SharedReader)
                    {
                        SharedReader.Initialise(p, Payload.Length);
                        Args = CustomEvents.ReadObjects(SharedReader);
                    }
                }
            }
        }
    }
}