using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// View шаро-коннектора.
    /// Отвечает за инпут (клик) и визуальную подсветку.
    /// </summary>
    public sealed class ConnectorView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        private Material _defaultMaterial;
        private Material _activeMaterial;
        private bool _isHighlighted;

        public ConnectorController Controller { get; private set; }
        public Renderer Renderer => _renderer;

        /// <summary>
        /// Инициализация: связываем с контроллером и запоминаем материалы.
        /// </summary>
        public void Initialize(ConnectorController controller, Material activeMaterial)
        {
            Controller = controller;

            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }

            if (_renderer != null)
            {
                // Берём instance-материал как "обычный"
                _defaultMaterial = _renderer.material;
            }

            _activeMaterial = activeMaterial;
            SetHighlighted(false);
        }

        /// <summary>
        /// Включить/выключить подсветку коннектора.
        /// </summary>
        public void SetHighlighted(bool highlighted)
        {
            if (_renderer == null)
                return;

            if (_isHighlighted == highlighted)
                return;

            _isHighlighted = highlighted;

            if (_isHighlighted && _activeMaterial != null)
            {
                _renderer.material = _activeMaterial;
            }
            else if (_defaultMaterial != null)
            {
                _renderer.material = _defaultMaterial;
            }
        }

        private void OnMouseDown()
        {
            // Базовый режим: один клик → событие OnClick на контроллере
            Controller?.OnClick();
        }

        private void Reset()
        {
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }
        }
    }
}