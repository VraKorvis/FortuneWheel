using FSM;
using UnityEngine;

namespace Roulette.FSM
{
    public class CooldownState : IState<RoulettePresenter>
    {
        private const float CooldownDuration = 10f;
        private float _timer = 10f;
        private int _lastDisplayedTime;

        private Coroutine _cooldownCoroutine;

        public void Enter(RoulettePresenter presenter)
        {
            _timer = CooldownDuration;
            _lastDisplayedTime = Mathf.CeilToInt(_timer);
            presenter.DisableButton();
            presenter.ShowTimerText(_lastDisplayedTime.ToString());
            presenter.SetRewards();
        }

        public void Execute(RoulettePresenter presenter)
        {
            _timer -= Time.deltaTime;
            int currentTime = Mathf.CeilToInt(_timer);

            if (currentTime != _lastDisplayedTime)
            {
                _lastDisplayedTime = currentTime;
                presenter.ShowTimerText(_lastDisplayedTime.ToString());
                presenter.SetRewards();
            }
        
            if (_timer <= 0) 
            {
                presenter.OnCooldownEnded();
            }
        }
        
        public void Exit(RoulettePresenter presenter)
        {
            presenter.EnableButton();
        }
        
    }
}