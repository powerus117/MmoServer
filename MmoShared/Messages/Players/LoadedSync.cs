using System.Collections.Generic;
using MmoShared.Messages.Players.Domain;
using ProtoBuf;

namespace MmoShared.Messages.Players
{
    [ProtoContract]
    public class LoadedSync : Message
    {
        public override MessageId Id => MessageId.LoadedSync;
        
        [ProtoMember(1)]
        public Dictionary<ulong, PlayerData> Players { get; set; }
    }
}