namespace FSM
{
    public interface IState<in TContext>
    {
        void Enter(TContext context);
        void Execute(TContext context);
        void Exit(TContext context);
    }
}