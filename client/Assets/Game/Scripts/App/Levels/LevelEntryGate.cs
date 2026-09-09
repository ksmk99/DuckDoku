using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class LevelEntryGate : ILevelEntryGate
    {
        private readonly IEnergyService _energyService;
        private readonly ILevelLauncher _levelLauncher;

        public LevelEntryGate(IEnergyService energyService, ILevelLauncher levelLauncher)
        {
            _energyService = energyService;
            _levelLauncher = levelLauncher;
        }

        public bool TryStart(int levelId)
        {
            if (!_energyService.HasEnough(EnergyPolicy.EntryCost))
            {
                _energyService.NotifyDenied(EnergyPolicy.EntryCost);
                return false;
            }

            _levelLauncher.LaunchLevel(levelId);
            return true;
        }
    }
}
