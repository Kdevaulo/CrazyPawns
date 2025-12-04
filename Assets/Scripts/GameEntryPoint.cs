using UnityEngine;

namespace CrazyPawn
{
    public sealed class GameEntryPoint : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private CrazyPawnSettings _settings;

        [Header("Prefabs")]
        [SerializeField] private PawnView _pawnPrefab;

        [Header("Scene References")]
        [SerializeField] private Transform _pawnsRoot;

        public SettingsProvider SettingsProvider { get; private set; }
        public BoardBounds BoardBounds { get; private set; }
        public ConnectionManager ConnectionManager { get; private set; }

        private PawnSpawner _pawnSpawner;

        private void Awake()
        {
            if (_settings == null)
            {
                Debug.LogError("[GameEntryPoint] CrazyPawnSettings is not assigned.", this);
                enabled = false;
                return;
            }

            if (_pawnPrefab == null)
            {
                Debug.LogError("[GameEntryPoint] Pawn prefab is not assigned.", this);
                enabled = false;
                return;
            }

            if (_pawnsRoot == null)
            {
                _pawnsRoot = transform;
            }

            SettingsProvider = new SettingsProvider(_settings);
            BoardBounds = new BoardBounds(SettingsProvider);

            ConnectionManager = new ConnectionManager(CreateConnectionLineView);

            _pawnSpawner = new PawnSpawner(SettingsProvider, CreatePawnAt);
            _pawnSpawner.SpawnInitialPawns();
        }

        private PawnView CreatePawnAt(Vector3 position)
        {
            var pawnInstance = Instantiate(_pawnPrefab, position, Quaternion.identity, _pawnsRoot);

            if (pawnInstance == null)
            {
                Debug.LogError("[GameEntryPoint] Pawn prefab instance does not contain PawnView component.", this);
                return null;
            }

            var pawnController = new PawnController(pawnInstance);

            pawnInstance.Initialize(pawnController, SettingsProvider.DeleteMaterial);
            pawnInstance.DeleteRequested += OnPawnDeleteRequested;

            var connectorViews = pawnInstance.ConnectorViews;

            if (connectorViews == null || connectorViews.Count == 0)
            {
                Debug.LogWarning("[GameEntryPoint] PawnView has no ConnectorViews assigned or found.", pawnInstance);
            }
            else
            {
                foreach (var connectorView in connectorViews)
                {
                    if (connectorView == null)
                        continue;

                    var connectorController = new ConnectorController(pawnController, connectorView, ConnectionManager);

                    connectorView.Initialize(connectorController, SettingsProvider.ActiveConnectorMaterial);

                    pawnController.AddConnector(connectorController);
                    ConnectionManager.RegisterConnector(connectorController);
                }
            }

            var dragHandler = pawnInstance.GetComponentInChildren<PawnDragHandler>(true);

            if (dragHandler != null)
            {
                dragHandler.Initialize(BoardBounds);
            }
            else
            {
                Debug.LogWarning("[GameEntryPoint] Pawn prefab has no PawnDragHandler attached.", pawnInstance);
            }

            return pawnInstance;
        }

        private ConnectionLineView CreateConnectionLineView()
        {
            var go = new GameObject("ConnectionLine");
            go.transform.SetParent(transform, false);

            var view = go.AddComponent<ConnectionLineView>();
            view.Init();

            return view;
        }

        private void OnPawnDeleteRequested(PawnController pawn)
        {
            if (pawn == null)
                return;

            ConnectionManager.RemoveConnectionsForPawn(pawn);

            if (pawn.View != null)
            {
                Destroy(pawn.View.gameObject);
            }
        }

        private void Update()
        {
            ConnectionManager.Tick();
        }
    }
}