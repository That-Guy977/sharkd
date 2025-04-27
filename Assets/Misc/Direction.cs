using UnityEngine;

enum Direction {
    Right, Left
}

static class DirectionExtensions {
    public static Vector2 AsVector(this Direction dir) {
        return dir switch {
            Direction.Right => Vector2.right,
            Direction.Left => Vector2.left,
        };
    }

    public static int AsScale(this Direction dir) {
        return dir switch {
            Direction.Right => 1,
            Direction.Left => -1,
        };
    }
}
