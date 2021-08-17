using AkaInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AkaUtility
{
    public static class AkaRandom
    {
        public static Random Random = new Random();
    }

    public static class RandomExtention
    {
        public static float NextFloat(this Random random, float minimum, float maximum)
        {
            return (float)(random.NextDouble() * (maximum - minimum) + minimum);
        }

        public static uint NextUint(this Random random, uint minimum, uint maximum)
        {
            var intMin = (int)(minimum + Int32.MinValue);
            var intMax = (int)(maximum + Int32.MinValue);
            var intResult = random.Next(intMin, intMax);
            if (intResult < 0)
                return (uint)(intResult - Int32.MinValue);
            else
                return (uint)intResult + (uint)Int32.MaxValue + 1;
        }

        public static DateTime NextDateTime(this Random random, DateTime minTime, DateTime maxTime)
        {
            return minTime.AddMilliseconds(random.NextDouble() * (maxTime - minTime).TotalMilliseconds);
        }

        public static T ChooseElementRandomlyInCount<T>(this Random random, IList<T> source)
        {
            return source[random.Next(source.Count)];
        }
        public static T ChooseElementRandomlyInCount<T>(this Random random, IEnumerable<T> source)
        {
            return source.ElementAt( random.Next(source.Count()));
        }

        public static int ChooseIndexRandomlyInCount<T>(this Random random, IList<T> source)
        {
            return random.Next(source.Count);
        }

        public static IEnumerable<T> ChooseIndexRanddomlyInSourceByCount<T>(this Random random, IEnumerable<T> source, int peekCount)
        {
            var tmpList = source?.ToList() ?? new List<T>();
            var resultList = new List<T>();
            while(peekCount > 0 && tmpList.Count >= peekCount)
            {
                peekCount--;
                var index = random.ChooseIndexRandomlyInCount(tmpList);
                resultList.Add(tmpList[index]);
                tmpList.RemoveAt(index);
            }
            return resultList;
        }

        public static int ChooseIndexRandomlyInCount<TKey, TValue>(this Random random, IDictionary<TKey, TValue> source)
        {
            return random.Next(source.Count);
        }

        public static TValue ChooseElementRandomlyInCount<TKey, TValue>(this Random random, IDictionary<TKey, TValue> source)
        {
            var choicedNumber = random.Next(source.Count);

            return source.Values.Skip(choicedNumber).First();
        }

        public static int ChooseIndexRandomlyInSum(this Random random, IList<float> source)
        {
            var totalSum = source.Sum(data => data);
            var choicedValue = random.NextFloat(0.0f, totalSum);

            float sum = 0.0f;
            int index = 0;
            foreach (var value in source)
            {
                sum += value;
                if (choicedValue < sum)
                {
                    return index;
                }

                index++;
            }

            throw new Exception("ChooseKeyPercently is not invalid logic");
        }

        public static int ChooseIndexRandomlyInSum(this Random random, IEnumerable<float> source)
        {
            var totalSum = source.Sum(data => data);
            var choicedValue = random.NextFloat(0.0f, totalSum);

            float sum = 0.0f;
            int index = 0;
            foreach (var value in source)
            {
                sum += value;
                if (choicedValue < sum)
                {
                    return index;
                }

                index++;
            }

            throw new Exception("ChooseKeyPercently is not invalid logic");
        }

        public static bool IsSuccess(this Random random, int pickingPercent)
        {
            if (pickingPercent <= 0)
            {
                return false;
            }

            var pickedValue = random.Next(100);

            return pickedValue < pickingPercent;
        }

        /// <summary>
        /// pickingRate: 0.0 ~ 1.0
        /// </summary>
        /// <param name="random"></param>
        /// <param name="pickingRate"></param>
        /// <returns></returns>
        public static bool IsSuccess(this Random random, float pickingRate)
        {
            if (pickingRate <= 0.0f)
            {
                return false;
            }

            var pickedValue = (float)random.NextDouble();

            return pickedValue < Math.Min(pickingRate, 1);
        }

        public static void Shuffle<T>(this Random random, IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, random.Next(i, list.Count));
        }

        static private void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static void Shuffle<T>(this Random random, Queue<T> queue)
        {
            List<T> list = new List<T>();
            while(queue.Count > 0)
                list.Add(queue.Dequeue());
            
            AkaRandom.Random.Shuffle(list);

            foreach(var item in list)
                queue.Enqueue(item);
        }

        public static int ChooseIndexRandomlyInSumOfProbability(this Random random, IEnumerable<IProbability> source)
        {
            var totalSum = source.Sum(data => data.Probability);
            var choicedValue = random.Next(0, totalSum);

            var currentSum = 0;
            int index = 0;
            foreach (var value in source)
            {
                currentSum += value.Probability;
                if (choicedValue < currentSum)
                    return index;

                index++;
            }

            throw new Exception("ChooseKeyPercently is not invalid logic");
        }

        public static int ChooseIndexRandomlyInSumOfProbabilityWithCorrection(this Random random, IEnumerable<ICorrection> source, IDictionary<uint, int> correction)
        {
            var probabilities = SetCorrectionToProbability(source, correction);

            var totalSum = probabilities.Sum();
            var choicedValue = random.Next(0, totalSum);

            var currentSum = 0;
            int index = 0;
            foreach (var value in probabilities)
            {
                currentSum += value;
                if (choicedValue < currentSum)
                    return index;

                index++;
            }

            throw new Exception("ChooseKeyPercently is not invalid logic");
        }

        public static List<int> SetCorrectionToProbability(IEnumerable<ICorrection> source, IDictionary<uint, int> correction)
        {
            return source.Select(value => value.Probability + (correction.ContainsKey(value.ElementId) ? correction[value.ElementId] : 0) ).ToList();
        }
    }
}
