using UnityEngine;

namespace CrazyPawn
{
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] private ConnectionLineView _linePrefab;
        [SerializeField] private CrazyPawnSettings _settings;
        [SerializeField] private BoardBounds _boardBounds;
        [SerializeField] private Transform _container;
        [SerializeField] private PawnView _pawnPrefab;

        private GameHandler _gameHandler;

        private void Awake()
        {
            _gameHandler = new GameHandler(_settings, _pawnPrefab, _linePrefab, _boardBounds, _container);
        }

        private void Start()
        {
            _gameHandler.BeginGame();
        }

        private void Update()
        {
            _gameHandler.Tick();
        }
    }
}