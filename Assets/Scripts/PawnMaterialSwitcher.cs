using System;

using UnityEngine;

namespace CrazyPawn
{
    /// <summary>
    /// Чистый класс, хранящий исходные материалы фигуры и материал "удаления".
    /// Не знает о Renderer, только о наборах материалов.
    /// </summary>
    public sealed class PawnMaterialSwitcher
    {
        private readonly Material _deleteMaterial;
        private readonly Material[] _defaultMaterials;

        public bool IsMarkedForDeletion { get; private set; }

        public int Count => _defaultMaterials.Length;

        public PawnMaterialSwitcher(Material deleteMaterial, Material[] defaultMaterials)
        {
            _deleteMaterial = deleteMaterial;
            _defaultMaterials = defaultMaterials ?? Array.Empty<Material>();
        }

        /// <summary>
        /// Меняет внутреннее состояние. Возвращает true, если состояние действительно изменилось.
        /// </summary>
        public bool SetMarkedForDeletion(bool value)
        {
            if (IsMarkedForDeletion == value)
                return false;

            IsMarkedForDeletion = value;
            return true;
        }

        /// <summary>
        /// Возвращает материал для указанного индекса Renderer
        /// в зависимости от текущего состояния.
        /// </summary>
        public Material GetMaterialForIndex(int index)
        {
            if (index < 0 || index >= _defaultMaterials.Length)
                return _deleteMaterial;

            return IsMarkedForDeletion ? _deleteMaterial : _defaultMaterials[index];
        }
    }
}