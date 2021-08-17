namespace AkaEnum.Battle
{
    public enum ValidateCardResultType
    {
        Success,    // 사용 후 대기열로 들어감
        Fail,       // 사용 되지않고 핸드로 다시 돌아옴
        Pass,       // 사용 되지 않고 핸드로 다시 돌아오지 않음, 버려지는 카드
        DiscordTarget   // 도발 걸린 대상이 있는데 다른 대상을 지정했을 경우
    }
}