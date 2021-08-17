using MessagePack;

[MessagePackObject]
public class ProtoRankInfo
{
    [Key(0)]
    public uint UserId;

    [Key(1)]
    public string Nickname;

    [Key(2)]
    public int RankPoint;

    [Key(3)]
    public uint ProfileIconId;

    [Key(4)]
    public string ClanName;
}