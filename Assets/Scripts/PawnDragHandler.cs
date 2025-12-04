using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Обработка drag ЛКМ по телу фигуры.
    /// Перемещает PawnView по плоскости Y = 0 и
    /// сообщает о подсветке/удалении через PawnView + BoardBounds.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class PawnDragHandler : MonoBehaviour
    {
        [SerializeField] private PawnView _pawnView;

        private BoardBounds _boardBounds;
        private Camera _camera;
        private Plane _dragPlane;
        private Vector3 _dragOffset;
        private bool _isDragging;
        private bool _isOutsideBoard;

        /// <summary>
        /// Вызывается из GameEntryPoint после спавна фигуры.
        /// </summary>
        public void Initialize(BoardBounds boardBounds)
        {
            _boardBounds = boardBounds;

            if (_pawnView == null)
            {
                _pawnView = GetComponentInParent<PawnView>();
            }

            if (_camera == null)
            {
                _camera = Camera.main;
            }

            _dragPlane = new Plane(Vector3.up, Vector3.zero);
        }

        private void Awake()
        {
            if (_pawnView == null)
            {
                _pawnView = GetComponentInParent<PawnView>();
            }
        }

        private void OnMouseDown()
        {
            if (_boardBounds == null)
            {
                Debug.LogWarning("[PawnDragHandler] BoardBounds is not initialized.", this);
                return;
            }

            if (_pawnView == null)
            {
                Debug.LogWarning("[PawnDragHandler] PawnView is not assigned.", this);
                return;
            }

            if (_camera == null)
            {
                _camera = Camera.main;

                if (_camera == null)
                {
                    Debug.LogWarning("[PawnDragHandler] No camera found.", this);
                    return;
                }
            }

            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (_dragPlane.Raycast(ray, out var enter))
            {
                var hitPoint = ray.GetPoint(enter);
                _dragOffset = _pawnView.transform.position - hitPoint;
                _isDragging = true;
            }
        }

        private void OnMouseDrag()
        {
            if (!_isDragging || _camera == null || _pawnView == null)
                return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (_dragPlane.Raycast(ray, out var enter))
            {
                var hitPoint = ray.GetPoint(enter);
                var targetPosition = hitPoint + _dragOffset;

                // Все фигуры должны лежать на Y = 0
                targetPosition.y = 0f;

                _pawnView.transform.position = targetPosition;

                UpdateDeleteState();
            }
        }

        private void OnMouseUp()
        {
            if (!_isDragging)
                return;

            _isDragging = false;

            UpdateDeleteState();

            // Если при отпускании ЛКМ центр вне доски — запрашиваем удаление фигуры.
            if (_isOutsideBoard)
            {
                _pawnView?.RequestDelete();
            }
        }

        /// <summary>
        /// Проверяет положение фигуры относительно BoardBounds
        /// и включает/выключает режим "удаления".
        /// </summary>
        private void UpdateDeleteState()
        {
            if (_boardBounds == null || _pawnView == null)
                return;

            var inside = _boardBounds.IsInside(_pawnView.transform.position);
            var shouldBeMarkedForDeletion = !inside;

            if (_isOutsideBoard == shouldBeMarkedForDeletion)
                return;

            _isOutsideBoard = shouldBeMarkedForDeletion;
            _pawnView.SetMarkedForDeletion(_isOutsideBoard);
        }
    }
}