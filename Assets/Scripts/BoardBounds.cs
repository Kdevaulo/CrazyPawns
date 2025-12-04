using System;

using UnityEngine;

namespace CrazyPawn
{
    public class BoardBounds
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly float _halfSize;

        public float HalfSize => _halfSize;

        public BoardBounds(SettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));

            _halfSize = _settingsProvider.CheckerboardSize * 1.5f * 0.5f;
        }

        public bool IsInside(Vector3 position)
        {
            return Mathf.Abs(position.x) <= _halfSize &&
                   Mathf.Abs(position.z) <= _halfSize;
        }
    }
}