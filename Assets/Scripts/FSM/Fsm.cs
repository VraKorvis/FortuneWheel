using System;
using System.Collections.Generic;
using FSM.Logger;

namespace FSM
{
    public class Fsm<TContext>
    {
        private readonly ILoggerService _logger;

        private readonly TContext _context;
        private IState<TContext> _currentState;
        private readonly Dictionary<Type, IState<TContext>> _states = new();

        public Fsm(TContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        public void SetInitialState<TState>() where TState : IState<TContext>
        {
            ChangeState<TState>();
        }

        public void AddState<TState>(TState state) where TState : IState<TContext>
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            var key = typeof(TState);
            if (_states.ContainsKey(key))
            {
                throw new InvalidOperationException($"[FSM] State {key} already added to FSM.");
            }

            _states[key] = state;
        }

        public void ChangeState<TState>() where TState : IState<TContext>
        {
            var desiredType = typeof(TState);
            if (!_states.TryGetValue(desiredType, out var nextState))
            {
                throw new InvalidOperationException($"[FSM] State {desiredType} is not registered in FSM.");
            }

            try
            {
                _currentState?.Exit(_context);
            }
            catch (Exception ex)
            {
                _logger.Log($"[FSM] fsm change state exception {ex.Message}");
            }

            _currentState = nextState;
            _currentState.Enter(_context);
        }

        public bool HasState<TState>() where TState : IState<TContext>
        {
            return _states.ContainsKey(typeof(TState));
        }

        public void RemoveState<TState>() where TState : IState<TContext>
        {
            _states.Remove(typeof(TState));
        }

        public void Execute()
        {
            _currentState?.Execute(_context);
        }
    }
}