using System;
using System.Collections.Generic;

namespace CrazyPawn
{
    /// <summary>
    /// Управляет выбором коннекторов, подсветкой и ConnectionLine (+ их Tick).
    /// </summary>
    public sealed class ConnectionManager
    {
        private readonly Func<ConnectionLineView> _createLineView;

        private readonly List<ConnectorController> _connectors = new List<ConnectorController>();
        private readonly List<ConnectionLine> _lines = new List<ConnectionLine>();

        private ConnectorController _firstSelected;

        public IReadOnlyList<ConnectorController> Connectors => _connectors;

        public ConnectionManager(Func<ConnectionLineView> createLineView)
        {
            _createLineView = createLineView ?? throw new ArgumentNullException(nameof(createLineView));
        }

        /// <summary>
        /// Регистрация коннектора, вызывается при создании фигур.
        /// </summary>
        public void RegisterConnector(ConnectorController connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));

            if (!_connectors.Contains(connector))
            {
                _connectors.Add(connector);
            }
        }

        /// <summary>
        /// Обработка клика по коннектору.
        /// </summary>
        public void OnConnectorClicked(ConnectorController connector)
        {
            if (connector == null)
                return;

            // ничего не выбрано — начинаем выбор
            if (_firstSelected == null)
            {
                StartSelection(connector);
                return;
            }

            // клик по тому же коннектору — отмена выбора
            if (ReferenceEquals(_firstSelected, connector))
            {
                ClearSelection();
                return;
            }

            // второй коннектор — проверяем валидность
            if (!CanConnect(_firstSelected, connector))
            {
                // невалидный второй клик — новый выбор с этого коннектора
                StartSelection(connector);
                return;
            }

            // валидная пара — создаём соединение
            CreateConnectionIfNotExists(_firstSelected, connector);
            ClearSelection();
        }

        private void StartSelection(ConnectorController connector)
        {
            _firstSelected = connector;
            HighlightAvailableConnectors();
        }

        /// <summary>
        /// Можно ли соединить два коннектора (без учёта уже существующих связей).
        /// </summary>
        private bool CanConnect(ConnectorController a, ConnectorController b)
        {
            if (a == null || b == null)
                return false;

            if (ReferenceEquals(a, b))
                return false;

            // Нельзя соединять коннекторы одной и той же фигуры.
            return !ReferenceEquals(a.Pawn, b.Pawn);
        }

        /// <summary>
        /// Создаёт ConnectionLine, если такой пары ещё нет.
        /// </summary>
        private void CreateConnectionIfNotExists(ConnectorController a, ConnectorController b)
        {
            if (!CanConnect(a, b))
                return;

            // Проверяем, что линии между этими коннекторами ещё нет.
            foreach (var line in _lines)
            {
                if (ReferenceEquals(line.A, a) && ReferenceEquals(line.B, b) ||
                    ReferenceEquals(line.A, b) && ReferenceEquals(line.B, a))
                {
                    return;
                }
            }

            var view = _createLineView?.Invoke();

            if (view == null)
            {
                // Нет возможности создать визуал — просто выходим.
                return;
            }

            var connectionLine = new ConnectionLine(a, b, view);
            _lines.Add(connectionLine);
        }

        /// <summary>
        /// Подсветить выбранный коннектор и все доступные цели.
        /// </summary>
        private void HighlightAvailableConnectors()
        {
            ClearHighlight();

            if (_firstSelected == null)
                return;

            _firstSelected.View?.SetHighlighted(true);

            foreach (var connector in _connectors)
            {
                if (connector == null || ReferenceEquals(connector, _firstSelected))
                    continue;

                if (CanConnect(_firstSelected, connector))
                {
                    connector.View?.SetHighlighted(true);
                }
            }
        }

        /// <summary>
        /// Снять подсветку со всех коннекторов.
        /// </summary>
        private void ClearHighlight()
        {
            foreach (var connector in _connectors)
            {
                connector?.View?.SetHighlighted(false);
            }
        }

        /// <summary>
        /// Очистить текущий выбор и подсветку.
        /// </summary>
        private void ClearSelection()
        {
            _firstSelected = null;
            ClearHighlight();
        }

        // Drag-соединения — этап 6.
        public void OnConnectorDragStart(ConnectorController connector)
        {
            // Будет реализовано на этапе 6.
        }

        public void OnConnectorDragEnd(ConnectorController connectorOrNull)
        {
            // Будет реализовано на этапе 6.
        }

        /// <summary>
        /// Обновление всех активных линий.
        /// </summary>
        public void Tick()
        {
            for (var i = 0; i < _lines.Count; i++)
            {
                _lines[i].Tick();
            }
        }

        /// <summary>
        /// Удаляет все коннекторы и линии, относящиеся к указанной фигуре.
        /// </summary>
        public void RemoveConnectionsForPawn(PawnController pawn)
        {
            if (pawn == null)
                return;

            // Если выбранный коннектор принадлежит этой фигуре — сбрасываем выбор.
            if (_firstSelected != null && ReferenceEquals(_firstSelected.Pawn, pawn))
            {
                ClearSelection();
            }

            // Удаляем все линии, где участвуют коннекторы этой фигуры.
            for (var i = _lines.Count - 1; i >= 0; i--)
            {
                var line = _lines[i];

                if (ReferenceEquals(line.A?.Pawn, pawn) || ReferenceEquals(line.B?.Pawn, pawn))
                {
                    line.Dispose();
                    _lines.RemoveAt(i);
                }
            }

            // Убираем из реестра коннекторов все коннекторы этой фигуры.
            for (var i = _connectors.Count - 1; i >= 0; i--)
            {
                var connector = _connectors[i];

                if (connector?.Pawn != null && ReferenceEquals(connector.Pawn, pawn))
                {
                    _connectors.RemoveAt(i);
                }
            }
        }
    }
}