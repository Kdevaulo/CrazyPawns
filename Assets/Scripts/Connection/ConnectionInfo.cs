namespace CrazyPawn
{
    public class ConnectionInfo
    {
        public readonly ConnectionLineView LineView;
        public readonly ConnectorInfo A;
        public readonly ConnectorInfo B;

        public ConnectionInfo(ConnectorInfo a, ConnectorInfo b, ConnectionLineView lineView)
        {
            LineView = lineView;
            A = a;
            B = b;
        }
    }
}