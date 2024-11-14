using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum MoveDirFlags
{
    None=0,
    Up=1<<0,
    Down=1<<1,
    Left=1<<2,
    Right=1<<3,
}

[System.Flags]
public enum CollisionFlags
{
    None=0,
    Wall=1<<0,
}

public class FlagManager : MonoBehaviour
{
    MoveDirFlags moveDirFlags;

    public void InitFlags(MoveDirFlags flags)
    {
        moveDirFlags = flags;
    }

    public void ActiveOnly(MoveDirFlags flags)
    {
        moveDirFlags = flags;
    }

    public void ReverseOnly(MoveDirFlags flags)
    {
        moveDirFlags ^= flags;
    }

    public void ReverseAll(MoveDirFlags flags)
    {
        moveDirFlags^=flags;
    }

    public void AddFlags(MoveDirFlags flags)
    {
        moveDirFlags |= flags;
    }

    public void RemoveFlags(MoveDirFlags flags)
    {
        moveDirFlags &= ~flags;
    }

    public bool HasFlag(MoveDirFlags flags)
    {
        return (moveDirFlags & flags) == flags;
    }

    public MoveDirFlags GetMoveFlags()
    {
        return moveDirFlags;
    }
}
