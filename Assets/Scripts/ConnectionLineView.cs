using UnityEngine;

namespace CrazyPawn
{
    public sealed class ConnectionLineView : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        public void Init()
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();

                if (_lineRenderer == null)
                {
                    _lineRenderer = gameObject.AddComponent<LineRenderer>();
                }
            }

            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;

            const float width = 0.07f;
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;

            if (_lineRenderer.material == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit");

                if (shader == null)
                {
                    shader = Shader.Find("Unlit/Color");
                }

                if (shader != null)
                {
                    var mat = new Material(shader);
                    mat.color = Color.white;
                    _lineRenderer.material = mat;
                }
            }
            else
            {
                _lineRenderer.material.color = Color.white;
            }
        }

        public void SetPositions(Vector3 a, Vector3 b)
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();

                if (_lineRenderer == null)
                {
                    return;
                }
            }

            _lineRenderer.SetPosition(0, a);
            _lineRenderer.SetPosition(1, b);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}