using System;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;
using Random = System.Random;

namespace CrazyPawn
{
    public class GameHandler
    {
        private readonly ConnectionManager _connectionManager;
        private readonly CrazyPawnSettings _settings;
        private readonly BoardBounds _bounds;
        private readonly Transform _container;
        private readonly PawnView _pawnPrefab;
        private readonly Random _random = new Random();

        private readonly Dictionary<PawnView, PawnController> _pawnControllers =
            new Dictionary<PawnView, PawnController>();
        private readonly Dictionary<PawnView, PawnSubscriptions> _subscriptions =
            new Dictionary<PawnView, PawnSubscriptions>();

        public GameHandler(CrazyPawnSettings settings, PawnView pawnPrefab, ConnectionLineView linePrefab,
            BoardBounds bounds, Transform container)
        {
            _pawnPrefab = pawnPrefab;
            _container = container;
            _settings = settings;
            _bounds = bounds;

            _connectionManager = new ConnectionManager(linePrefab, _container);
        }

        public void BeginGame()
        {
            _bounds.Initialize(_settings);

            for (var i = 0; i < _settings.InitialPawnCount; i++)
            {
                var position = GetRandomPositionInCircle(_settings.InitialZoneRadius);
                CreatePawnAt(position);
            }
        }

        public void Tick()
        {
            _connectionManager.Tick();
        }

        private void CreatePawnAt(Vector3 position)
        {
            var pawnView = Object.Instantiate(_pawnPrefab, position, Quaternion.identity, _container);

            var pawnController = new PawnController(pawnView, _connectionManager);
            _pawnControllers.Add(pawnView, pawnController);

            pawnView.Initialize(_settings.DeleteMaterial);

            var connectorViews = pawnView.ConnectorViews;

            for (var i = 0; i < connectorViews.Count; i++)
            {
                var connectorView = connectorViews[i];
                connectorView.Initialize(_settings.ActiveConnectorMaterial);

                _connectionManager.RegisterConnector(pawnView, connectorView);
            }

            var subscriptions = new PawnSubscriptions()
            {
                DragSub = p => HandleDrag(p, pawnView),
                UpSub = p => HandleUp(p, pawnView)
            };

            _subscriptions.Add(pawnView, subscriptions);

            pawnView.PawnDragged += subscriptions.DragSub;
            pawnView.MouseUp += subscriptions.UpSub;
        }

        private Vector3 GetRandomPositionInCircle(float radius)
        {
            var angle = _random.NextDouble() * Math.PI * 2.0;
            var distance = Math.Sqrt(_random.NextDouble()) * radius;

            var x = (float) (Math.Cos(angle) * distance);
            var z = (float) (Math.Sin(angle) * distance);

            return new Vector3(x, 0f, z);
        }

        private void HandleUp(Vector3 pos, PawnView view)
        {
            var isInside = _bounds.IsInside(pos);
            view.SetMarkedForDeletion(!isInside);

            if (!isInside)
            {
                OnPawnDeleteRequested(view);
            }
        }

        private void HandleDrag(Vector3 pos, PawnView view)
        {
            var isInside = _bounds.IsInside(pos);
            view.SetMarkedForDeletion(!isInside);
        }

        private void OnPawnDeleteRequested(PawnView view)
        {
            var subs = _subscriptions[view];
            view.PawnDragged -= subs.DragSub;
            view.MouseUp -= subs.UpSub;
            _subscriptions.Remove(view);

            _connectionManager.RemoveConnectionsForPawn(view);
            _pawnControllers[view].Dispose();
            _pawnControllers.Remove(view);
            view.DestroySelf();
        }
    }
}