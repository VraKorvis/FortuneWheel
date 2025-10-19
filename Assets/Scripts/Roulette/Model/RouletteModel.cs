using System.Collections.Generic;
using UnityEngine;

namespace Roulette
{
    public class RouletteReward
    {
        public int Value;
        public RewardType Type;
        public int SlotIndex;
    }

    public class RouletteModel
    {
        private const int CountRewards = 12;
        private const int RewardPoolSize = 20;
        private const int RewardStep = 5;

        private RewardType _lastGeneratedType = RewardType.None;

        private readonly RouletteReward[] _cachedRewards = new RouletteReward[CountRewards];
        private readonly int[] _valuePool = new int[RewardPoolSize];
        public RouletteReward[] CurrentRewards { get; private set; }

        public RouletteModel()
        {
            for (int i = 0; i < RewardPoolSize; i++)
            {
                _valuePool[i] = (i + 1) * RewardStep;
            }

            for (int i = 0; i < CountRewards; i++)
            {
                _cachedRewards[i] = new RouletteReward();
            }
        }

        public RouletteReward[] GenerateAndSetRewards()
        {
            RewardType newType = GenerateRewardType();

            int[] shuffledValues = FisherYatesShuffle(_valuePool);

            for (int i = 0; i < CountRewards; i++)
            {
                _cachedRewards[i].Value = shuffledValues[i];
                _cachedRewards[i].Type = newType;
                _cachedRewards[i].SlotIndex = i;
            }

            CurrentRewards = _cachedRewards;
            return _cachedRewards;
        }

        public int GetWinningSlotIndex()
        {
            return Random.Range(0, CountRewards);
        }

        private int[] FisherYatesShuffle(int[] array)
        {
            int[] copy = (int[])array.Clone();

            for (int i = copy.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int temp = copy[i];
                copy[i] = copy[j];
                copy[j] = temp;
            }

            return copy;
        }

        private RewardType GenerateRewardType()
        {
            List<RewardType> availableTypes = new List<RewardType>
            {
                RewardType.Crystals,
                RewardType.Coins,
                RewardType.Rubies
            };

            if (_lastGeneratedType != RewardType.None)
            {
                availableTypes.Remove(_lastGeneratedType);
            }

            int selectedIndex = Random.Range(0, availableTypes.Count);
            RewardType newType = availableTypes[selectedIndex];
            _lastGeneratedType = newType;

            return newType;
        }
    }
}