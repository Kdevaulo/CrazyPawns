using System;

namespace CrazyPawn
{
    /// <summary>
    /// Логическая связь между двумя коннекторами + ссылка на вью линии.
    /// </summary>
    public sealed class ConnectionLine
    {
        private readonly ConnectorController _a;
        private readonly ConnectorController _b;
        private readonly ConnectionLineView _view;

        public ConnectorController A => _a;
        public ConnectorController B => _b;

        public ConnectionLine(ConnectorController a, ConnectorController b, ConnectionLineView view)
        {
            _a = a ?? throw new ArgumentNullException(nameof(a));
            _b = b ?? throw new ArgumentNullException(nameof(b));
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        /// <summary>
        /// Обновляет позицию линии по текущим позициям коннекторов.
        /// </summary>
        public void Tick()
        {
            var aView = _a.View;
            var bView = _b.View;

            if (aView == null || bView == null)
                return;

            var aPos = aView.transform.position;
            var bPos = bView.transform.position;

            _view.SetPositions(aPos, bPos);
        }

        /// <summary>
        /// Чистит визуал.
        /// </summary>
        public void Dispose()
        {
            _view.DestroySelf();
        }
    }
}