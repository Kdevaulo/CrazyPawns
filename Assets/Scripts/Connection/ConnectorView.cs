using System;

using UnityEngine;

namespace CrazyPawn
{
    public class ConnectorView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        private Material _defaultMaterial;
        private Material _activeMaterial;

        private bool _isHighlighted;
        private bool _isDragging;
        private bool _mouseDown;

        private Vector3 _mouseDownPosition;
        private Camera _camera;

        private const float DragThresholdSqr = 4f;

        public Renderer Renderer => _renderer;

        public event Action<ConnectorView> Clicked;

        public event Action<ConnectorView> DragStarted;

        public event Action<ConnectorView> DragEnded;

        public void Initialize(Material activeMaterial)
        {
            _defaultMaterial = _renderer.material;
            _activeMaterial = activeMaterial;
            SetHighlighted(false);
        }

        public void SetHighlighted(bool highlighted)
        {
            if (_isHighlighted == highlighted)
                return;

            _isHighlighted = highlighted;

            _renderer.material = _isHighlighted
                ? _activeMaterial
                : _defaultMaterial;
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
                    DragStarted?.Invoke(this);
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
                DragEnded?.Invoke(target);
            }
            else
            {
                Clicked?.Invoke(this);
            }

            _isDragging = false;
        }

        private ConnectorView TryGetConnectorUnderMouse()
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
                return otherView;
            }

            return null;
        }
    }
}