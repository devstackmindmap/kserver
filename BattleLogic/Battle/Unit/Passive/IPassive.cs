namespace BattleLogic
{
    public interface IPassive
    {
        bool IsConditionOk();
        bool IsConditionOk(float baseValue, float compareValue);
    }
}
