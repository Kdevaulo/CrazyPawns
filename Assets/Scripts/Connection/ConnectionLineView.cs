using UnityEngine;

namespace CrazyPawn
{
    public class ConnectionLineView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;

        private const float Width = 0.07f;

        public void Init()
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;

            _lineRenderer.startWidth = Width;
            _lineRenderer.endWidth = Width;
        }

        public void SetPositions(Vector3 a, Vector3 b)
        {
            _lineRenderer.SetPosition(0, a);
            _lineRenderer.SetPosition(1, b);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}