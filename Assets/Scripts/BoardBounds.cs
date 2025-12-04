using System;

using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Логика расчёта границ доски и проверка, находится ли точка внутри.
    /// Доска — квадрат в плоскости XZ с центром в (0, 0, 0).
    /// Ширина клетки: 1.5, размер в клетках берём из CheckerboardSize.
    /// </summary>
    public class BoardBounds
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly float _halfSize;

        /// <summary>
        /// Половина длины стороны доски в юнитах.
        /// </summary>
        public float HalfSize => _halfSize;

        public BoardBounds(SettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));

            // halfSize = CheckerboardSize * 1.5f / 2f
            _halfSize = _settingsProvider.CheckerboardSize * 1.5f * 0.5f;
        }

        /// <summary>
        /// Проверка, находится ли точка (по X/Z) внутри доски.
        /// Границы считаем включительно.
        /// </summary>
        public bool IsInside(Vector3 position)
        {
            return Mathf.Abs(position.x) <= _halfSize &&
                   Mathf.Abs(position.z) <= _halfSize;
        }
    }
}