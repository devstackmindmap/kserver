namespace MatchingServer
{
    public class SearchingRange
    {
        public long BeforeMin;
        public long BeforeMax;
        public long AfterMin;
        public long AfterMax;

        public SearchingRange(long rank)
        {
            BeforeMin = rank - 20;
            BeforeMax = rank  - 1;
            AfterMin = rank + 1;
            AfterMax = rank + 20;
        }
    }
}
