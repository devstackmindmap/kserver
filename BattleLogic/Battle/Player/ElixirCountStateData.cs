using AkaEnum;

namespace BattleLogic
{
    public struct ElixirCountStateData
    {
        public ElixirCountState ElixirCountState;
        public double CurrentElixir;
        public int NeedElixir;

        public ElixirCountStateData(ElixirCountState state, double currentElixir, int needElixir)
        {
            ElixirCountState = state;
            CurrentElixir = currentElixir;
            NeedElixir = needElixir;
        }
    }
}