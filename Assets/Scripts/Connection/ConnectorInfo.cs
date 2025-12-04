using System;

namespace CrazyPawn
{
    public class ConnectorInfo
    {
        public readonly ConnectorView View;
        public readonly PawnView Pawn;

        public ConnectorInfo(PawnView pawn, ConnectorView view)
        {
            Pawn = pawn;
            View = view;
        }
    }
}