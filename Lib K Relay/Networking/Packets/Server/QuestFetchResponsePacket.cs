using Lib_K_Relay.Networking.Packets.DataObjects;

namespace Lib_K_Relay.Networking.Packets.Server
{
    public class QuestFetchResponsePacket : Packet // BROKEN AS OF X32.3.1
    {
        public QuestData[] Quests = new QuestData[0];
        public int NextRefreshPrice = -1;

        public override PacketType Type
        { get { return PacketType.QUESTFETCHRESPONSE; } }

        public override void Read(PacketReader r)
        {
            Quests = new QuestData[r.ReadInt16()];
            for (int i = 0; i < Quests.Length; i++)
                Quests[i] = (QuestData)new QuestData().Read(r);
            NextRefreshPrice = r.ReadInt16();
        }

        public override void Write(PacketWriter w)
        {
            w.Write((short)Quests.Length);
            for (int i = 0; i < Quests.Length; i++)
                Quests[i].Write(w);
            w.Write((short)NextRefreshPrice);
        }
    }
}
