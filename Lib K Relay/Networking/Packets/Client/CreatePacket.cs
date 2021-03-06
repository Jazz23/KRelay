﻿namespace Lib_K_Relay.Networking.Packets.Client
{
    public class CreatePacket : Packet
    {
        public ushort ClassType;
        public ushort SkinType;
        public bool IsChallenger;

        public override PacketType Type
        { get { return PacketType.CREATE; } }

        public override void Read(PacketReader r)
        {
            ClassType = r.ReadUInt16();
            SkinType = r.ReadUInt16();
            IsChallenger = r.ReadBoolean();
        }

        public override void Write(PacketWriter w)
        {
            w.Write((ushort)ClassType);
            w.Write((ushort)SkinType);
            w.Write(IsChallenger);
        }
    }
}
