using UnityEngine;

namespace CrazyPawn
{
    public sealed class ConnectorView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        private Material _defaultMaterial;
        private Material _activeMaterial;
        private bool _isHighlighted;

        private Camera _camera;
        private bool _mouseDown;
        private bool _isDragging;
        private Vector3 _mouseDownPosition;

        private const float DragThresholdSqr = 4f;

        public ConnectorController Controller { get; private set; }
        public Renderer Renderer => _renderer;

        public void Initialize(ConnectorController controller, Material activeMaterial)
        {
            Controller = controller;

            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }

            if (_renderer != null)
            {
                _defaultMaterial = _renderer.material;
            }

            _activeMaterial = activeMaterial;
            SetHighlighted(false);
        }

        public void SetHighlighted(bool highlighted)
        {
            if (_renderer == null)
                return;

            if (_isHighlighted == highlighted)
                return;

            _isHighlighted = highlighted;

            if (_isHighlighted && _activeMaterial != null)
            {
                _renderer.material = _activeMaterial;
            }
            else if (_defaultMaterial != null)
            {
                _renderer.material = _defaultMaterial;
            }
        }

        private void Awake()
        {
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }
        }

        private void OnMouseDown()
        {
            _mouseDown = true;
            _isDragging = false;
            _mouseDownPosition = Input.mousePosition;

            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void OnMouseDrag()
        {
            if (!_mouseDown)
                return;

            if (!_isDragging)
            {
                var delta = Input.mousePosition - _mouseDownPosition;

                if (delta.sqrMagnitude >= DragThresholdSqr)
                {
                    _isDragging = true;
                    Controller?.OnDragStart();
                }
            }
        }

        private void OnMouseUp()
        {
            if (!_mouseDown)
                return;

            _mouseDown = false;

            if (_isDragging)
            {
                var target = TryGetConnectorUnderMouse();
                Controller?.OnDragEnd(target);
            }
            else
            {
                Controller?.OnClick();
            }

            _isDragging = false;
        }

        private ConnectorController TryGetConnectorUnderMouse()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_camera == null)
                return null;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 1000f))
            {
                var otherView = hit.collider.GetComponentInParent<ConnectorView>();

                if (otherView != null)
                {
                    return otherView.Controller;
                }
            }

            return null;
        }

        private void Reset()
        {
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }
        }
    }
}