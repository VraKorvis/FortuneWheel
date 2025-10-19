using FSM;

namespace Roulette.FSM
{
    public class ActiveState : IState<RoulettePresenter>
    {
        public void Enter(RoulettePresenter presenter)
        {
            presenter.ShowActiveText(); 
        }

        public void Execute(RoulettePresenter presenter)
        {
            
        }

        public void Exit(RoulettePresenter presenter)
        {
            
        }
    }
}