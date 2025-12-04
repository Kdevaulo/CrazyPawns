using System;
using System.Collections.Generic;

using UnityEngine;

namespace CrazyPawn
{
    public class PawnView : MonoBehaviour
    {
        public event Action<Vector3> PawnDragged;
        public event Action<Vector3> MouseUp;
        public IReadOnlyList<ConnectorView> ConnectorViews => _connectorViews;

        [SerializeField] private ConnectorView[] _connectorViews;
        [SerializeField] private Renderer[] _renderers;

        [SerializeField] private Transform _parentTransform;

        private Material[] _defaultMaterials;

        private Material _deleteMaterial;
        private Vector3 _dragOffset;
        private Camera _camera;
        private Plane _dragPlane;

        private bool _isMarkedForDeletion;
        private bool _isOutsideBoard;
        private bool _isDragging;

        public void Initialize(Material deleteMaterial)
        {
            _defaultMaterials = new Material[_renderers.Length];
            _deleteMaterial = deleteMaterial;
            _dragPlane = new Plane(Vector3.up, Vector3.zero);
            _camera ??= Camera.main;

            for (var i = 0; i < _renderers.Length; i++)
            {
                _defaultMaterials[i] = _renderers[i].material;
            }
        }

        public void SetMarkedForDeletion(bool isMarked)
        {
            var changed = _isMarkedForDeletion != isMarked;
            if (!changed)
                return;

            _isMarkedForDeletion = isMarked;

            for (var i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material = isMarked
                    ? _deleteMaterial
                    : _defaultMaterials[i];
            }
        }

        private void OnMouseDown()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (_dragPlane.Raycast(ray, out var enter))
            {
                var hitPoint = ray.GetPoint(enter);
                _dragOffset = _parentTransform.position - hitPoint;
                _isDragging = true;
            }
        }

        private void OnMouseDrag()
        {
            if (!_isDragging)
                return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!_dragPlane.Raycast(ray, out var enter))
                return;

            var hitPoint = ray.GetPoint(enter);
            var targetPosition = hitPoint + _dragOffset;
            targetPosition.y = 0f;

            _parentTransform.position = targetPosition;
            PawnDragged?.Invoke(targetPosition);
        }

        private void OnMouseUp()
        {
            if (!_isDragging)
                return;

            _isDragging = false;

            MouseUp?.Invoke(_parentTransform.position);
        }
    }
}