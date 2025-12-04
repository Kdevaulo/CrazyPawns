using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Корневой компонент сцены.
    /// Инициализирует настройки, границы доски, менеджер соединений и фигуры.
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

            // Инфраструктура
            SettingsProvider = new SettingsProvider(_settings);
            BoardBounds = new BoardBounds(SettingsProvider);

            // Менеджер соединений
            ConnectionManager = new ConnectionManager();

            // Спавнер фигур
            _pawnSpawner = new PawnSpawner(SettingsProvider, CreatePawnAt);
            _pawnSpawner.SpawnInitialPawns();
        }

        /// <summary>
        /// Фабрика для PawnSpawner: инстанцирует префаб, создаёт контроллеры и навешивает обработчики.
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

            // Инициализация вью: материалы и ссылки
            pawnInstance.Initialize(pawnController, SettingsProvider.DeleteMaterial);
            pawnInstance.DeleteRequested += OnPawnDeleteRequested;

            // Коннекторы
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

                    // Важно: передаём ActiveConnectorMaterial из настроек
                    connectorView.Initialize(connectorController, SettingsProvider.ActiveConnectorMaterial);

                    pawnController.AddConnector(connectorController);
                    ConnectionManager.RegisterConnector(connectorController);
                }
            }

            // Drag по телу фигуры
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

        /// <summary>
        /// Обработка запроса удаления фигуры (при отпускании мыши вне доски).
        /// </summary>
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
            // Пока Tick пустой, но цепочка уже настроена — на этапе 5 добавим обновление линий.
            ConnectionManager.Tick();
        }
    }
}