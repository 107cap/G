using System;

public static class Constants
{
   
}


/// <summary>
/// 서버에서 클라로 명령하는 이벤트
/// </summary>
public enum EventType
{
    NONE = 0,
    JOIN_GAME = 1, 
    //EXIT_GAME = 2,
    START_RACE = 3,
    END_RACE = 4,
    ADD_PLAYER = 5
}

public enum PacketType
{
    NONE = 0,
    SEND = 1,
    RECEIVE = 2,
    ERROR = 3,

    //---------
    PLAYER = 4,
    EVENT = 5
}
