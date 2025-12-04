using System.Collections.Generic;
using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Вьюшка фигуры. Висит на корневом объекте префаба Pawn.
    /// Держит ссылки на контроллер и дочерние ConnectorView.
    /// </summary>
    public sealed class PawnView : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private Renderer _bodyRenderer;

        [Header("Connectors")]
        [SerializeField] private ConnectorView[] _connectorViews;

        public PawnController Controller { get; private set; }
        public Renderer BodyRenderer => _bodyRenderer;
        public IReadOnlyList<ConnectorView> ConnectorViews => _connectorViews;

        /// <summary>
        /// Вызывается из фабрики/спавнера после инстанциирования.
        /// </summary>
        public void Initialize(PawnController controller)
        {
            Controller = controller;

            // Если коннекторы не заданы руками — забираем всех детей.
            if (_connectorViews == null || _connectorViews.Length == 0)
            {
                _connectorViews = GetComponentsInChildren<ConnectorView>(includeInactive: true);
            }
        }

        private void Reset()
        {
            if (_bodyRenderer == null)
            {
                _bodyRenderer = GetComponentInChildren<Renderer>();
            }

            if (_connectorViews == null || _connectorViews.Length == 0)
            {
                _connectorViews = GetComponentsInChildren<ConnectorView>(includeInactive: true);
            }
        }
    }
}