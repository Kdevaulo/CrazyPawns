using System;

using UnityEngine;

using Random = System.Random;

namespace CrazyPawn
{
    /// <summary>
    /// Логика спавна фигур: читает настройки, выбирает позиции и вызывает фабрику.
    /// </summary>
    public sealed class PawnSpawner
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly Func<Vector3, PawnView> _pawnFactory;
        private readonly Random _random;

        public PawnSpawner(
            SettingsProvider settingsProvider,
            Func<Vector3, PawnView> pawnFactory,
            int? seed = null)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            _pawnFactory = pawnFactory ?? throw new ArgumentNullException(nameof(pawnFactory));
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>
        /// Спавнит InitialPawnCount фигур в круге InitialZoneRadius.
        /// </summary>
        public void SpawnInitialPawns()
        {
            var count = _settingsProvider.InitialPawnCount;
            var radius = _settingsProvider.InitialZoneRadius;

            for (var i = 0; i < count; i++)
            {
                var position = GetRandomPositionInCircle(radius);
                _pawnFactory(position);
            }
        }

        private Vector3 GetRandomPositionInCircle(float radius)
        {
            // Равномерное распределение по площади круга
            var angle = _random.NextDouble() * Math.PI * 2.0;
            var distance = Math.Sqrt(_random.NextDouble()) * radius;

            var x = (float) (Math.Cos(angle) * distance);
            var z = (float) (Math.Sin(angle) * distance);

            return new Vector3(x, 0f, z);
        }
    }
}