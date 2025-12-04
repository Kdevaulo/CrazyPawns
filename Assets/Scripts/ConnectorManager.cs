using System;
using System.Collections.Generic;

namespace CrazyPawn
{
    /// <summary>
    /// Управляет коннекторами и (в будущем) соединениями между ними.
    /// Сейчас умеет только регистрировать коннекторы и содержит заглушки под будущую логику.
    /// </summary>
    public sealed class ConnectionManager
    {
        private readonly List<ConnectorController> _connectors = new List<ConnectorController>();

        public IReadOnlyList<ConnectorController> Connectors => _connectors;

        public void RegisterConnector(ConnectorController connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));

            if (!_connectors.Contains(connector))
            {
                _connectors.Add(connector);
            }
        }

        public void OnConnectorClicked(ConnectorController connector)
        {
            // Реальная логика появится на этапе 4.
        }

        public void OnConnectorDragStart(ConnectorController connector)
        {
            // Реальная логика появится на этапе 6.
        }

        public void OnConnectorDragEnd(ConnectorController connectorOrNull)
        {
            // Реальная логика появится на этапе 6.
        }

        public void RemoveConnectionsForPawn(PawnController pawn)
        {
            // Реальная логика удаления всех линий для фигуры будет на этапе 7.
        }
    }
}