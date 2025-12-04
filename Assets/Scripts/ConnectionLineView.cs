using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Вью линии соединения. Оборачивает LineRenderer.
    /// </summary>
    public sealed class ConnectionLineView : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        /// <summary>
        /// Первичная настройка LineRenderer:
        /// 2 точки, толщина 0.07, белый unlit-материал.
        /// </summary>
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

            // Материал: пытаемся найти unlit-шейдер, красим в белый.
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

        /// <summary>
        /// Обновляет позиции концов линии.
        /// </summary>
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

        /// <summary>
        /// Уничтожает объект линии.
        /// </summary>
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}