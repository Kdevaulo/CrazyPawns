using System.Collections.Generic;

using UnityEngine;

namespace CrazyPawn
{
    public class GameEntryPoint : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private CrazyPawnSettings _settings;

        [Header("Prefabs")]
        [SerializeField] private PawnView _pawnPrefab;
        [SerializeField] private ConnectionLineView _linePrefab;

        [Header("References")]
        [SerializeField] private Transform _pawnsRoot;

        private ConnectionManager _connectionManager;
        private BoardBounds _boardBounds;
        private PawnSpawner _pawnSpawner;

        private readonly Dictionary<PawnView, PawnController> _pawnControllers =
            new Dictionary<PawnView, PawnController>();

        private void Awake()
        {
            _boardBounds = new BoardBounds(_settings);

            _connectionManager = new ConnectionManager(CreateConnectionLineView);

            _pawnSpawner = new PawnSpawner(_settings, CreatePawnAt);
            _pawnSpawner.SpawnInitialPawns();
        }

        private void Update()
        {
            _connectionManager.Tick();
        }

        private PawnView CreatePawnAt(Vector3 position)
        {
            var pawnView = Instantiate(_pawnPrefab, position, Quaternion.identity, _pawnsRoot);

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

            pawnView.PawnDragged += p => HandleDrag(p, pawnView);
            pawnView.MouseUp += p => HandleUp(p, pawnView);

            return pawnView;
        }

        private void HandleUp(Vector3 pos, PawnView view)
        {
            var isInside = _boardBounds.IsInside(pos);
            view.SetMarkedForDeletion(!isInside);

            if (!isInside)
            {
                // todo: это не работает, надо кешировать чтобы отписаться
                view.PawnDragged -= p => HandleDrag(p, view);
                view.MouseUp -= p => HandleUp(p, view);
                OnPawnDeleteRequested(view);
            }
        }

        private void HandleDrag(Vector3 pos, PawnView view)
        {
            var isInside = _boardBounds.IsInside(pos);
            view.SetMarkedForDeletion(!isInside);
        }

        private ConnectionLineView CreateConnectionLineView()
        {
            var view = Instantiate(_linePrefab, transform);
            view.Init();

            return view;
        }

        private void OnPawnDeleteRequested(PawnView pawnView)
        {
            if (!_pawnControllers.TryGetValue(pawnView, out var pawnController))
            {
                return;
            }

            _connectionManager.RemoveConnectionsForPawn(pawnView);
            _pawnControllers.Remove(pawnView);
            pawnController.Dispose();
            Destroy(pawnView.gameObject);
        }
    }
}