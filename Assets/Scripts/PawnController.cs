using System;
using System.Collections.Generic;

using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Доменная сущность "фигура".
    /// Хранит ссылку на PawnView и свои коннекторы.
    /// </summary>
    public sealed class PawnController
    {
        private readonly PawnView _view;
        private readonly List<ConnectorController> _connectors = new List<ConnectorController>();

        public PawnView View => _view;
        public IReadOnlyList<ConnectorController> Connectors => _connectors;

        /// <summary>
        /// Позиция центра фигуры (через view.transform).
        /// </summary>
        public Vector3 Position => _view.transform.position;

        public PawnController(PawnView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public void AddConnector(ConnectorController connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));

            if (!_connectors.Contains(connector))
            {
                _connectors.Add(connector);
            }
        }
    }
}