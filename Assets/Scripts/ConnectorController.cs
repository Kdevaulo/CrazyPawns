using System;

using UnityEngine;

namespace CrazyPawn
{
    public sealed class ConnectorController
    {
        private readonly ConnectionManager _connectionManager;

        public PawnController Pawn { get; }
        public ConnectorView View { get; }

        public Vector3 Position => View.transform.position;

        public ConnectorController(
            PawnController pawn,
            ConnectorView view,
            ConnectionManager connectionManager)
        {
            Pawn = pawn ?? throw new ArgumentNullException(nameof(pawn));
            View = view ?? throw new ArgumentNullException(nameof(view));
            _connectionManager = connectionManager;
        }

        public void OnClick()
        {
            _connectionManager?.OnConnectorClicked(this);
        }

        public void OnDragStart()
        {
            _connectionManager?.OnConnectorDragStart(this);
        }

        public void OnDragEnd(ConnectorController targetOrNull)
        {
            _connectionManager?.OnConnectorDragEnd(targetOrNull);
        }
    }
}