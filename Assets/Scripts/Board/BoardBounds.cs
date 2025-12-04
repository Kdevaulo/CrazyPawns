using System;

using UnityEngine;

namespace CrazyPawn
{
    public class BoardBounds : MonoBehaviour
    {
        private Vector3 _borderCubeSize;
        private float _halfSide;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(Vector3.zero, _borderCubeSize);
        }

        public void Initialize(CrazyPawnSettings settings)
        {
            var side = settings.CheckerboardSize * 1.5f;
            _halfSide = side * 0.5f;
            _borderCubeSize = new Vector3(side, 0.1f, side);
        }

        public bool IsInside(Vector3 position)
        {
            return Mathf.Abs(position.x) <= _halfSide &&
                   Mathf.Abs(position.z) <= _halfSide;
        }
    }
}