using Roulette;
using FSM.Logger;
using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(UpdateManager))]
    public class Bootstrap : MonoBehaviour
    {
        private UpdateManager _updateManager;
        [SerializeField] private GameObject roulettePopup;
        [SerializeField] private Transform uiContainer;
        
        private void Awake()
        {
            RegisterLogger();
            RegisterUpdateManager();
            RegisterRoulette();
        }

        private void RegisterUpdateManager()
        {
            _updateManager = GetComponent<UpdateManager>();
            ServiceLocator.Register(_updateManager);
        }

        private void RegisterRoulette()
        {
            var rouletteModel = new RouletteModel();
            var popup = Instantiate(roulettePopup, uiContainer);
            var view = popup.GetComponent<IRouletteView>();
            var roulette = new RoulettePresenter(rouletteModel, view);
            roulette.Initialize();
            ServiceLocator.Register(roulette);
        }

        private void RegisterLogger()
        {
            var logger = new UnityConsoleLogger();
            ServiceLocator.Register<ILoggerService>(logger);
        }

    }
}