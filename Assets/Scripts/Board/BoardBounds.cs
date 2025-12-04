using UnityEngine;

namespace CrazyPawn
{
    public class BoardBounds
    {
        private readonly float _halfSize;

        public BoardBounds(CrazyPawnSettings settings)
        {
            _halfSize = settings.CheckerboardSize * 1.5f * 0.5f;
        }

        public bool IsInside(Vector3 position)
        {
            return Mathf.Abs(position.x) <= _halfSize &&
                   Mathf.Abs(position.z) <= _halfSize;
        }
    }
}