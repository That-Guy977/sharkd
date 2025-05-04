using UnityEngine;

enum Direction {
    Right = 1,
    Left = -1,
}

static class DirectionExtensions {
    public static int Value(this Direction dir) => (int)dir;

    public static Vector2 AsVector(this Direction dir) => Vector2.right * dir.Value();

    public static Direction Reverse(this Direction dir) => (Direction)(-dir.Value());
}
