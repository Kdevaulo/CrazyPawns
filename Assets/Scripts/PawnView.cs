using System;
using System.Collections.Generic;

using UnityEngine;

namespace CrazyPawn
{
    public sealed class PawnView : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private Renderer _bodyRenderer;

        [Header("Connectors")]
        [SerializeField] private ConnectorView[] _connectorViews;

        private PawnMaterialSwitcher _materialSwitcher;
        private Renderer[] _allRenderers;

        public PawnController Controller { get; private set; }
        public Renderer BodyRenderer => _bodyRenderer;
        public IReadOnlyList<ConnectorView> ConnectorViews => _connectorViews;

        public event Action<PawnController> DeleteRequested;

        public void Initialize(PawnController controller, Material deleteMaterial)
        {
            Controller = controller;

            if (_bodyRenderer == null)
            {
                _bodyRenderer = GetComponentInChildren<Renderer>();
            }

            if (_connectorViews == null || _connectorViews.Length == 0)
            {
                _connectorViews = GetComponentsInChildren<ConnectorView>(true);
            }

            BuildRenderersAndMaterials(deleteMaterial);
        }

        private void BuildRenderersAndMaterials(Material deleteMaterial)
        {
            var renderers = new List<Renderer>();

            if (_bodyRenderer != null)
            {
                renderers.Add(_bodyRenderer);
            }

            if (_connectorViews != null)
            {
                for (var i = 0; i < _connectorViews.Length; i++)
                {
                    var cv = _connectorViews[i];

                    if (cv != null && cv.Renderer != null)
                    {
                        renderers.Add(cv.Renderer);
                    }
                }
            }

            _allRenderers = renderers.ToArray();

            var defaultMaterials = new Material[_allRenderers.Length];

            for (var i = 0; i < _allRenderers.Length; i++)
            {
                defaultMaterials[i] = _allRenderers[i].material;
            }

            _materialSwitcher = new PawnMaterialSwitcher(deleteMaterial, defaultMaterials);
        }

        public void SetMarkedForDeletion(bool isMarked)
        {
            if (_materialSwitcher == null || _allRenderers == null)
                return;

            var changed = _materialSwitcher.SetMarkedForDeletion(isMarked);
            if (!changed)
                return;

            for (var i = 0; i < _allRenderers.Length; i++)
            {
                var renderer = _allRenderers[i];
                if (renderer == null)
                    continue;

                renderer.material = _materialSwitcher.GetMaterialForIndex(i);
            }
        }

        public void RequestDelete()
        {
            if (Controller == null)
            {
                Debug.LogWarning("[PawnView] Delete requested, but Controller is null.", this);
                return;
            }

            DeleteRequested?.Invoke(Controller);
        }

        private void Reset()
        {
            if (_bodyRenderer == null)
            {
                _bodyRenderer = GetComponentInChildren<Renderer>();
            }

            if (_connectorViews == null || _connectorViews.Length == 0)
            {
                _connectorViews = GetComponentsInChildren<ConnectorView>(true);
            }
        }
    }
}