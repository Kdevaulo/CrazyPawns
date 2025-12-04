using System;
using System.Collections.Generic;

namespace CrazyPawn
{
    public class ConnectionManager
    {
        private readonly List<ConnectionInfo> _connections = new List<ConnectionInfo>();
        private readonly List<ConnectorInfo> _connectors = new List<ConnectorInfo>();

        private readonly Func<ConnectionLineView> _createdLineView;

        private ConnectorInfo _firstSelected;
        private ConnectorInfo _dragSource;

        public ConnectionManager(Func<ConnectionLineView> createLineView)
        {
            _createdLineView = createLineView;
        }

        public void RegisterConnector(PawnView pawn, ConnectorView connectorView)
        {
            if (FindConnector(connectorView) != null)
                return;

            _connectors.Add(new ConnectorInfo(pawn, connectorView));
        }

        public void Tick()
        {
            for (var i = 0; i < _connections.Count; i++)
            {
                var connection = _connections[i];

                var aView = connection.A.View;
                var bView = connection.B.View;
                var lineView = connection.LineView;

                if (aView == null || bView == null || lineView == null)
                    continue;

                var aPos = aView.transform.position;
                var bPos = bView.transform.position;

                lineView.SetPositions(aPos, bPos);
            }
        }

        public void OnConnectorClicked(ConnectorView connectorView)
        {
            var connector = FindConnector(connectorView);
            if (connector == null)
                return;

            if (_firstSelected == null)
            {
                StartSelection(connector);
                return;
            }

            if (ReferenceEquals(_firstSelected, connector))
            {
                ClearSelection();
                return;
            }

            if (!CanConnect(_firstSelected, connector))
            {
                StartSelection(connector);
                return;
            }

            CreateConnectionIfNotExists(_firstSelected, connector);
            ClearSelection();
        }

        public void OnConnectorDragStart(ConnectorView connectorView)
        {
            var connector = FindConnector(connectorView);
            if (connector == null)
                return;

            _dragSource = connector;
            StartSelection(connector);
        }

        public void OnConnectorDragEnd(ConnectorView targetView)
        {
            if (_dragSource == null)
                return;

            var source = _dragSource;
            _dragSource = null;

            ClearSelection();

            if (targetView == null)
                return;

            var target = FindConnector(targetView);
            if (target == null)
                return;

            if (!CanConnect(source, target))
                return;

            CreateConnectionIfNotExists(source, target);
        }

        public void RemoveConnectionsForPawn(PawnView pawn)
        {
            if (pawn == null)
                return;

            if (_firstSelected != null && ReferenceEquals(_firstSelected.Pawn, pawn))
            {
                ClearSelection();
            }

            if (_dragSource != null && ReferenceEquals(_dragSource.Pawn, pawn))
            {
                _dragSource = null;
            }

            for (var i = _connections.Count - 1; i >= 0; i--)
            {
                var connection = _connections[i];

                if (ReferenceEquals(connection.A.Pawn, pawn) || ReferenceEquals(connection.B.Pawn, pawn))
                {
                    connection.LineView.DestroySelf();
                    _connections.RemoveAt(i);
                }
            }

            UnregisterConnectors(pawn);
        }

        private ConnectorInfo FindConnector(ConnectorView view)
        {
            if (view == null)
                return null;

            for (var i = 0; i < _connectors.Count; i++)
            {
                var connector = _connectors[i];
                if (connector != null && ReferenceEquals(connector.View, view))
                    return connector;
            }

            return null;
        }

        private void StartSelection(ConnectorInfo connector)
        {
            _firstSelected = connector;
            HighlightAvailableConnectors();
        }

        private void CreateConnectionIfNotExists(ConnectorInfo a, ConnectorInfo b)
        {
            if (!CanConnect(a, b))
                return;

            for (var i = 0; i < _connections.Count; i++)
            {
                var connection = _connections[i];

                var sameDirection = ReferenceEquals(connection.A, a) && ReferenceEquals(connection.B, b);
                var oppositeDirection = ReferenceEquals(connection.A, b) && ReferenceEquals(connection.B, a);

                if (sameDirection || oppositeDirection)
                    return;
            }

            var view = _createdLineView.Invoke();
            if (view == null)
                return;

            _connections.Add(new ConnectionInfo(a, b, view));
        }

        private void HighlightAvailableConnectors()
        {
            ClearHighlight();

            if (_firstSelected == null)
                return;

            _firstSelected.View.SetHighlighted(true);

            for (var i = 0; i < _connectors.Count; i++)
            {
                var connector = _connectors[i];
                if (connector == null || ReferenceEquals(connector, _firstSelected))
                    continue;

                if (CanConnect(_firstSelected, connector))
                {
                    connector.View.SetHighlighted(true);
                }
            }
        }

        private bool CanConnect(ConnectorInfo a, ConnectorInfo b)
        {
            if (a == null || b == null)
                return false;

            if (ReferenceEquals(a, b))
                return false;

            return !ReferenceEquals(a.Pawn, b.Pawn);
        }

        private void ClearSelection()
        {
            _firstSelected = null;
            ClearHighlight();
        }

        private void ClearHighlight()
        {
            for (var i = 0; i < _connectors.Count; i++)
            {
                var connector = _connectors[i];
                connector?.View.SetHighlighted(false);
            }
        }

        private void UnregisterConnectors(PawnView pawn)
        {
            if (pawn == null)
                return;

            for (var i = _connectors.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(_connectors[i].Pawn, pawn))
                    _connectors.RemoveAt(i);
            }
        }
    }
}