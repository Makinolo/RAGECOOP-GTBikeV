using Lidgren.Network;
using System;
namespace RageCoop.Core
{
    internal partial class Packets
    {
        internal class CustomEvent : Packet
        {
            public override PacketType Type => (_queued ? PacketType.CustomEventQueued : PacketType.CustomEvent);
            public CustomEvent(Func<byte, NetIncomingMessage, object> onResolve = null, bool queued = false)
            {
                _resolver = onResolve;
                _queued = queued;
            }
            private readonly bool _queued;
            private Func<byte, NetIncomingMessage, object> _resolver { get; set; }
            public int Hash { get; set; }
            public object[] Args { get; set; }

            protected override void Serialize(NetOutgoingMessage m)
            {
                Args = Args ?? new object[] { };

                m.Write(Hash);
                m.Write(Args.Length);
                foreach (var arg in Args)
                {
                    CoreUtils.GetBytesFromObject(arg, m);
                }
            }

            public override void Deserialize(NetIncomingMessage m)
            {
                Hash = m.ReadInt32();
                var len = m.ReadInt32();
                Args = new object[len];
                for (int i = 0; i < len; i++)
                {
                    Args[i] = CoreUtils.GetObjectFromBytes(m, _resolver);
                }
            }
        }
    }
}
