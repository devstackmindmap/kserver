using MessagePack;

[MessagePackObject]
public class ProtoRankInfoClan
{
    [Key(0)]
    public uint ClanId;

    [Key(1)]
    public string ClanName;

    [Key(2)]
    public uint ClanSymbolId;

    [Key(3)]
    public int MemberCount;

    [Key(4)]
    public int RankPoint;
}