using System;
using System.Collections.Generic;

namespace CrazyPawn
{
    /// <summary>
    /// Управляет режимом выбора коннекторов, подсветкой и логическими соединениями (без линий).
    /// </summary>
    public sealed class ConnectionManager
    {
        private sealed class ConnectionRecord
        {
            public ConnectorController A;
            public ConnectorController B;
        }

        private readonly List<ConnectorController> _connectors = new List<ConnectorController>();
        private readonly List<ConnectionRecord> _connections = new List<ConnectionRecord>();

        private ConnectorController _firstSelected;

        /// <summary>
        /// Все зарегистрированные коннекторы.
        /// </summary>
        public IReadOnlyList<ConnectorController> Connectors => _connectors;

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
        /// Логика:
        /// - нет выбранного → начинаем выбор, подсвечиваем доступные;
        /// - клик по тому же → отменяем выбор;
        /// - клик по несовместимому → начинаем новый выбор с этого коннектора;
        /// - клик по валидному → создаём соединение и выходим из режима выбора.
        /// </summary>
        public void OnConnectorClicked(ConnectorController connector)
        {
            if (connector == null)
                return;

            // ещё ничего не выбрано — начинаем выбор
            if (_firstSelected == null)
            {
                StartSelection(connector);
                return;
            }

            // клик по тому же коннектору — отмена режима
            if (ReferenceEquals(_firstSelected, connector))
            {
                ClearSelection();
                return;
            }

            // второй коннектор — проверяем, можно ли соединить
            if (!CanConnect(_firstSelected, connector))
            {
                // Невалидный второй клик: начинаем выбор заново с этого коннектора
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
        /// Проверка, можно ли соединить два коннектора (без учёта уже существующих связей).
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
        /// Создаёт логическую связь между двумя коннекторами, если такой ещё нет.
        /// </summary>
        private void CreateConnectionIfNotExists(ConnectorController a, ConnectorController b)
        {
            if (!CanConnect(a, b))
                return;

            foreach (var record in _connections)
            {
                if (ReferenceEquals(record.A, a) && ReferenceEquals(record.B, b) ||
                    ReferenceEquals(record.A, b) && ReferenceEquals(record.B, a))
                {
                    // Уже есть такая связь
                    return;
                }
            }

            _connections.Add(new ConnectionRecord { A = a, B = b });
        }

        /// <summary>
        /// Подсветить первый выбранный коннектор и все доступные цели.
        /// </summary>
        private void HighlightAvailableConnectors()
        {
            ClearHighlight();

            if (_firstSelected == null)
                return;

            // выбранный коннектор всегда подсвечиваем
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

        /// <summary>
        /// Заглушки под будущее drag-соединение (этап 6).
        /// </summary>
        public void OnConnectorDragStart(ConnectorController connector)
        {
            // Этап 6.
        }

        public void OnConnectorDragEnd(ConnectorController connectorOrNull)
        {
            // Этап 6.
        }

        /// <summary>
        /// Tick для обновления линий появится на этапе 5.
        /// </summary>
        public void Tick()
        {
            // Этап 5 — обновление ConnectionLine.
        }

        /// <summary>
        /// Удаляет все логические соединения и коннекторы, относящиеся к указанной фигуре.
        /// Линии будут чиститься дополнительно на этапе 7.
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

            // Удаляем все связи, где участвуют коннекторы этой фигуры.
            for (var i = _connections.Count - 1; i >= 0; i--)
            {
                var c = _connections[i];

                if (ReferenceEquals(c.A?.Pawn, pawn) || ReferenceEquals(c.B?.Pawn, pawn))
                {
                    _connections.RemoveAt(i);
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