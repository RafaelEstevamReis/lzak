namespace FTS.Core
{
    public enum AlarmReasons
    {
        // 1XX Move and position related
        UnkownCurrentLocation = 101,
        MoveOutOfBounds = 102,

        // 9xx connection-related
        ControllerDisconnected = 901
    }
}