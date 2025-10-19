using System;
using Core;
using Roulette.FSM;
using FSM;
using FSM.Logger;

namespace Roulette
{
    public class RoulettePresenter : IDisposable
    {
        private readonly Fsm<RoulettePresenter> _fsm;

        private readonly IRouletteView _view;
        private readonly RouletteModel _model;
        private readonly ILoggerService _logger;

        public RoulettePresenter(RouletteModel model, IRouletteView view)
        {
            _model = model;
            _view = view;
            _logger = ServiceLocator.Get<ILoggerService>();

            _fsm = new Fsm<RoulettePresenter>(this, _logger);

            var cooldown = new CooldownState();
            var activeSate = new ActiveState();
            var rewarding = new RewardingState();

            _fsm.AddState(cooldown);
            _fsm.AddState(activeSate);
            _fsm.AddState(rewarding);
            
        }
        
        public void Initialize()
        {
            
            _fsm.SetInitialState<CooldownState>();
            _view.OnSpinButtonClicked += HandleSpinButtonClick;
            _view.OnDestroyed += Dispose;
            ServiceLocator.Get<UpdateManager>().Register(Update);
        }
        
        public void DisableButton() => _view.SetButtonInteractable(false);
        public void EnableButton() => _view.SetButtonInteractable(true);
        public void ShowActiveText() => _view.ShowActiveText();

        public void ShowTimerText(string timerValue) => _view.ShowTimerText(timerValue);
        
        private void Update()
        {
            _fsm.Execute();
        }

        public void SetRewards()
        {
            var rewards =_model.GenerateAndSetRewards();
            _view.SetRewards(rewards);
        }

        public void PlaySpinAnimation()
        {
            int winningSlotIndex = _model.GetWinningSlotIndex();
            _logger.LogInfo($"[Roulette] winningSlotIndex {winningSlotIndex}");

            var rewards = _model.CurrentRewards;
            _view.PlaySpinAnimation(rewards[winningSlotIndex], OnRewardingStateEnded);
        }

        private void OnRewardingStateEnded()
        {
            _fsm.ChangeState<CooldownState>();
        }

        public void OnCooldownEnded()
        {
            _fsm.ChangeState<ActiveState>();
        }

        private void HandleSpinButtonClick()
        {
            _fsm.ChangeState<RewardingState>();
        }
        
        public void Dispose()
        {
            ServiceLocator.Get<UpdateManager>().Unregister(Update);
            _view.OnSpinButtonClicked -= HandleSpinButtonClick;
        }
        
    }
}