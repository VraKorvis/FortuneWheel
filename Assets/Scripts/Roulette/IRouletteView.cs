using System;

namespace Roulette
{
    public interface IRouletteView
    {
        event Action OnSpinButtonClicked;
        event Action OnDestroyed;

        void ShowActiveText(); 
        void ShowTimerText(string text); 
        void SetButtonInteractable(bool interactable);
        void PlaySpinAnimation(RouletteReward reward, Action onComplete = null);
        void PlayRewardAnimation(RouletteReward reward, Action onComplete = null);
        void SetRewards(RouletteReward[] rewards);
    }
}