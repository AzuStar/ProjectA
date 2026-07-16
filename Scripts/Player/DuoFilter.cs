using System;

namespace ProjectA.Game.Player;

[Flags]
public enum DuoFilter
{
    PLAYER = 1 << 0,
    DRONE = 1 << 1,
    BOTH = PLAYER | DRONE,
}
