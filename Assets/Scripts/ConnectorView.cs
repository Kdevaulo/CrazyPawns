using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Вьюшка шаро-коннектора. Висит на дочернем объекте фигуры.
    /// </summary>
    public sealed class ConnectorView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        public ConnectorController Controller { get; private set; }
        public Renderer Renderer => _renderer;

        /// <summary>
        /// Вызывается после создания контроллера.
        /// </summary>
        public void Initialize(ConnectorController controller)
        {
            Controller = controller;
        }

        private void Reset()
        {
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }
        }

        // Обработка кликов/drag появится на этапах 4 и 6.
    }
}