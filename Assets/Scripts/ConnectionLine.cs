using System;

namespace CrazyPawn
{
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

        public void Dispose()
        {
            _view.DestroySelf();
        }
    }
}