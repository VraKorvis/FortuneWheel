using Services;

namespace Roulette.FSM
{
    public class RewardingState : IState<RoulettePresenter>
    {
        public void Enter(RoulettePresenter presenter)
        {
            presenter.DisableButton();
            presenter.PlaySpinAnimation();
        }

        public void Execute(RoulettePresenter presenter)
        {
        }

        public void Exit(RoulettePresenter presenter)
        {
        }
    }
}