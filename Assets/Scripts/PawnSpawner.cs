using System;
using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Логика спавна фигур: читает настройки, выбирает позиции и вызывает фабрику.
    /// </summary>
    public sealed class PawnSpawner
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly Func<Vector3, PawnView> _pawnFactory;
        private readonly System.Random _random;

        public PawnSpawner(
            SettingsProvider settingsProvider,
            Func<Vector3, PawnView> pawnFactory,
            int? seed = null)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            _pawnFactory = pawnFactory ?? throw new ArgumentNullException(nameof(pawnFactory));
            _random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        }

        /// <summary>
        /// Спавнит InitialPawnCount фигур в круге InitialZoneRadius.
        /// </summary>
        public void SpawnInitialPawns()
        {
            int count = _settingsProvider.InitialPawnCount;
            float radius = _settingsProvider.InitialZoneRadius;

            for (int i = 0; i < count; i++)
            {
                Vector3 position = GetRandomPositionInCircle(radius);
                _pawnFactory(position);
            }
        }

        private Vector3 GetRandomPositionInCircle(float radius)
        {
            // Равномерное распределение по площади круга
            double angle = _random.NextDouble() * Math.PI * 2.0;
            double distance = Math.Sqrt(_random.NextDouble()) * radius;

            float x = (float)(Math.Cos(angle) * distance);
            float z = (float)(Math.Sin(angle) * distance);

            return new Vector3(x, 0f, z);
        }
    }
}