using System;

using UnityEngine;

namespace CrazyPawn
{
    public class PawnSubscriptions
    {
        public Action<Vector3> DragSub;
        public Action<Vector3> UpSub;
    }
}