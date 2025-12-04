using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Корневой компонент сцены.
    /// Инициализирует настройки, границы доски, менеджер соединений и спавнит фигуры.
    /// </summary>
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

            // Этап 1 — из прошлой части:
            SettingsProvider = new SettingsProvider(_settings);
            BoardBounds = new BoardBounds(SettingsProvider);

            // Этап 2:
            ConnectionManager = new ConnectionManager();
            _pawnSpawner = new PawnSpawner(SettingsProvider, CreatePawnAt);

            _pawnSpawner.SpawnInitialPawns();
        }

        /// <summary>
        /// Фабрика для PawnSpawner: инстанцирует префаб, создаёт контроллер и коннекторы.
        /// </summary>
        private PawnView CreatePawnAt(Vector3 position)
        {
            var pawnInstance = Instantiate(_pawnPrefab, position, Quaternion.identity, _pawnsRoot);
            if (pawnInstance == null)
            {
                Debug.LogError("[GameEntryPoint] Pawn prefab instance does not contain PawnView component.", this);
                return null;
            }

            // Контроллер фигуры
            var pawnController = new PawnController(pawnInstance);
            pawnInstance.Initialize(pawnController);

            // Коннекторы
            var connectorViews = pawnInstance.ConnectorViews;
            if (connectorViews == null || connectorViews.Count == 0)
            {
                Debug.LogWarning("[GameEntryPoint] PawnView has no ConnectorViews assigned or found.", pawnInstance);
                return pawnInstance;
            }

            foreach (var connectorView in connectorViews)
            {
                if (connectorView == null)
                    continue;

                var connectorController = new ConnectorController(pawnController, connectorView, ConnectionManager);
                connectorView.Initialize(connectorController);

                pawnController.AddConnector(connectorController);
                ConnectionManager.RegisterConnector(connectorController);
            }

            return pawnInstance;
        }

        private void Update()
        {
            // На этапах 5–6 здесь появится вызов ConnectionManager.Tick().
        }
    }
}