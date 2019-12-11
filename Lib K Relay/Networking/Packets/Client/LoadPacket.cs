﻿namespace Lib_K_Relay.Networking.Packets.Client
{
    public class LoadPacket : Packet
    {
        public int CharacterId;
        public bool IsFromArena;
        public bool IsChallenger;

        public override PacketType Type
        { get { return PacketType.LOAD; } }

        public override void Read(PacketReader r)
        {
            CharacterId = r.ReadInt32();
            IsFromArena = r.ReadBoolean();
            IsChallenger = r.ReadBoolean();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(CharacterId);
            w.Write(IsFromArena);
            w.Write(IsChallenger);
        }
    }
}
