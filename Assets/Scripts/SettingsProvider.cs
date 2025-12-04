using UnityEngine;

namespace CrazyPawn
{
    public class SettingsProvider
    {
        private readonly CrazyPawnSettings _settings;

        public SettingsProvider(CrazyPawnSettings settings)
        {
            _settings = settings;
        }

        public float InitialZoneRadius => _settings.InitialZoneRadius;
        public int InitialPawnCount => _settings.InitialPawnCount;

        public Material DeleteMaterial => _settings.DeleteMaterial;
        public Material ActiveConnectorMaterial => _settings.ActiveConnectorMaterial;

        public int CheckerboardSize => _settings.CheckerboardSize;
        public Color BlackCellColor => _settings.BlackCellColor;
        public Color WhiteCellColor => _settings.WhiteCellColor;

        public CrazyPawnSettings RawSettings => _settings;
    }
}