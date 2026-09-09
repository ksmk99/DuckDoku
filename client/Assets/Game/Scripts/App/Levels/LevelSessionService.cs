using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    public class LevelSessionService : ILevelSessionService
    {
        private readonly ILevelsClient _levelsClient;
        private readonly IEnergyService _energyService;
        private readonly ICurrencyService _currencyService;
        private readonly IHintWalletService _hintWalletService;
        private readonly IHintWalletClient _hintWalletClient;

        public LevelSessionService(ILevelsClient levelsClient,
            IEnergyService energyService,
            ICurrencyService currencyService,
            IHintWalletService hintWalletService,
            IHintWalletClient hintWalletClient)
        {
            if (levelsClient == null)
            {
                throw new ArgumentNullException(nameof(levelsClient));
            }

            if (energyService == null)
            {
                throw new ArgumentNullException(nameof(energyService));
            }

            if (currencyService == null)
            {
                throw new ArgumentNullException(nameof(currencyService));
            }

            if (hintWalletService == null)
            {
                throw new ArgumentNullException(nameof(hintWalletService));
            }

            if (hintWalletClient == null)
            {
                throw new ArgumentNullException(nameof(hintWalletClient));
            }

            _levelsClient = levelsClient;
            _energyService = energyService;
            _currencyService = currencyService;
            _hintWalletService = hintWalletService;
            _hintWalletClient = hintWalletClient;
        }

        public async UniTask<string> StartLevelAsync(int levelId)
        {
            StartLevelResponse response = await _levelsClient.StartLevel(levelId);

            _energyService.Apply(response.energy, response.energyMax, response.energyRefillMs);

            return response.sessionId;
        }

        public async UniTask<LevelResult> CompleteLevelAsync(int levelId, string sessionId, IReadOnlyList<Cell> placement)
        {
            CompleteLevelResponse response = await _levelsClient.CompleteLevel(levelId, sessionId, placement);

            _currencyService.Apply(response.balance);

            return new LevelResult(response.stars, response.duration, response.coinsEarned);
        }

        public async UniTask<bool> UseHintAsync(int levelId, string sessionId)
        {
            if (!_hintWalletService.HasEnough(1))
            {
                _hintWalletService.NotifyDenied(1);
                return false;
            }

            UseHintResponse response = await _levelsClient.UseHint(levelId, sessionId);

            _hintWalletService.Apply(response.hints);

            return true;
        }

        public async UniTask<bool> PurchaseHintAsync()
        {
            if (!_currencyService.HasEnough(HintPolicy.Cost))
            {
                _currencyService.NotifyDenied(HintPolicy.Cost);
                return false;
            }

            PurchaseHintResponse response = await _hintWalletClient.Purchase();

            _currencyService.Apply(response.balance);
            _hintWalletService.Apply(response.hints);

            return true;
        }
    }
}
