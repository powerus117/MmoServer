﻿namespace MmoShared.Messages
{
    public enum MessageId : ushort
    {
        LoginNotify = 10,
        LoginResultSync = 11,
        RegisterNotify = 12,
        RegisterResultSync = 13,
        LoadedSync = 14,
        PlayerMoveNotify = 20,
        PlayerMovedSync = 21
    }
}