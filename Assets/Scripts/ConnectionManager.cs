using System;
using System.Collections.Generic;

namespace CrazyPawn
{
    public sealed class ConnectionManager
    {
        private readonly Func<ConnectionLineView> _createLineView;

        private readonly List<ConnectorController> _connectors = new List<ConnectorController>();
        private readonly List<ConnectionLine> _lines = new List<ConnectionLine>();

        private ConnectorController _firstSelected;

        private ConnectorController _dragSource;

        public IReadOnlyList<ConnectorController> Connectors => _connectors;

        public ConnectionManager(Func<ConnectionLineView> createLineView)
        {
            _createLineView = createLineView ?? throw new ArgumentNullException(nameof(createLineView));
        }

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

        private void StartSelection(ConnectorController connector)
        {
            _firstSelected = connector;
            HighlightAvailableConnectors();
        }

        private bool CanConnect(ConnectorController a, ConnectorController b)
        {
            if (a == null || b == null)
                return false;

            if (ReferenceEquals(a, b))
                return false;

            return !ReferenceEquals(a.Pawn, b.Pawn);
        }

        private void CreateConnectionIfNotExists(ConnectorController a, ConnectorController b)
        {
            if (!CanConnect(a, b))
                return;

            foreach (var line in _lines)
            {
                if (ReferenceEquals(line.A, a) && ReferenceEquals(line.B, b) ||
                    ReferenceEquals(line.A, b) && ReferenceEquals(line.B, a))
                {
                    return;
                }
            }

            var view = _createLineView?.Invoke();

            if (view == null)
            {
                return;
            }

            var connectionLine = new ConnectionLine(a, b, view);
            _lines.Add(connectionLine);
        }

        private void HighlightAvailableConnectors()
        {
            ClearHighlight();

            if (_firstSelected == null)
                return;

            _firstSelected.View?.SetHighlighted(true);

            foreach (var connector in _connectors)
            {
                if (connector == null || ReferenceEquals(connector, _firstSelected))
                    continue;

                if (CanConnect(_firstSelected, connector))
                {
                    connector.View?.SetHighlighted(true);
                }
            }
        }

        private void ClearHighlight()
        {
            foreach (var connector in _connectors)
            {
                connector?.View?.SetHighlighted(false);
            }
        }

        private void ClearSelection()
        {
            _firstSelected = null;
            ClearHighlight();
        }

        public void OnConnectorDragStart(ConnectorController connector)
        {
            if (connector == null)
                return;

            _dragSource = connector;

            StartSelection(connector);
        }

        public void OnConnectorDragEnd(ConnectorController connectorOrNull)
        {
            if (_dragSource == null)
                return;

            var source = _dragSource;
            _dragSource = null;

            ClearSelection();

            if (connectorOrNull == null)
                return;

            if (!CanConnect(source, connectorOrNull))
                return;

            CreateConnectionIfNotExists(source, connectorOrNull);
        }

        public void Tick()
        {
            for (var i = 0; i < _lines.Count; i++)
            {
                _lines[i].Tick();
            }
        }

        public void RemoveConnectionsForPawn(PawnController pawn)
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

            for (var i = _lines.Count - 1; i >= 0; i--)
            {
                var line = _lines[i];

                if (ReferenceEquals(line.A?.Pawn, pawn) || ReferenceEquals(line.B?.Pawn, pawn))
                {
                    line.Dispose();
                    _lines.RemoveAt(i);
                }
            }

            for (var i = _connectors.Count - 1; i >= 0; i--)
            {
                var connector = _connectors[i];

                if (connector?.Pawn != null && ReferenceEquals(connector.Pawn, pawn))
                {
                    _connectors.RemoveAt(i);
                }
            }
        }
    }
}