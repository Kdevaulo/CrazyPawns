using System;

using UnityEngine;

using Random = System.Random;

namespace CrazyPawn
{
    public class PawnSpawner
    {
        private readonly Func<Vector3, PawnView> _pawnFactory;
        private readonly CrazyPawnSettings _settings;
        private readonly Random _random = new Random();

        public PawnSpawner(CrazyPawnSettings settings, Func<Vector3, PawnView> pawnFactory)
        {
            _settings = settings;
            _pawnFactory = pawnFactory;
        }

        public void SpawnInitialPawns()
        {
            for (var i = 0; i < _settings.InitialPawnCount; i++)
            {
                var position = GetRandomPositionInCircle(_settings.InitialZoneRadius);
                _pawnFactory(position);
            }
        }

        private Vector3 GetRandomPositionInCircle(float radius)
        {
            var angle = _random.NextDouble() * Math.PI * 2.0;
            var distance = Math.Sqrt(_random.NextDouble()) * radius;

            var x = (float) (Math.Cos(angle) * distance);
            var z = (float) (Math.Sin(angle) * distance);

            return new Vector3(x, 0f, z);
        }
    }
}