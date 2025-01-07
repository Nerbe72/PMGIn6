using UnityEngine;

public class Enemy : MovingObject
{
    protected override bool AttemptMove<T>(int _xDir, int _yDir)
    {
        return true;
    }

    protected override void OnCantMove<T>(T component)
    {

    }
}
