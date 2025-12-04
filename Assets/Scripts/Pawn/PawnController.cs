using System;

namespace CrazyPawn
{
    public class PawnController : IDisposable
    {
        private readonly ConnectionManager _connectionManager;
        private readonly PawnView _view;

        public PawnController(PawnView view, ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _view = view;

            SubscribeToConnectorEvents();
        }

        public void Dispose()
        {
            UnsubscribeFromConnectorEvents();
        }

        private void SubscribeToConnectorEvents()
        {
            var connectors = _view.ConnectorViews;

            for (var i = 0; i < connectors.Count; i++)
            {
                var connectorView = connectors[i];

                connectorView.Clicked += OnConnectorClicked;
                connectorView.DragStarted += OnConnectorDragStarted;
                connectorView.DragEnded += OnConnectorDragEnded;
            }
        }

        private void UnsubscribeFromConnectorEvents()
        {
            var connectors = _view.ConnectorViews;

            for (var i = 0; i < connectors.Count; i++)
            {
                var connectorView = connectors[i];

                connectorView.Clicked -= OnConnectorClicked;
                connectorView.DragStarted -= OnConnectorDragStarted;
                connectorView.DragEnded -= OnConnectorDragEnded;
            }
        }

        private void OnConnectorClicked(ConnectorView connectorView)
        {
            _connectionManager.OnConnectorClicked(connectorView);
        }

        private void OnConnectorDragStarted(ConnectorView connectorView)
        {
            _connectionManager.OnConnectorDragStart(connectorView);
        }

        private void OnConnectorDragEnded(ConnectorView view)
        {
            _connectionManager.OnConnectorDragEnd(view);
        }
    }
}