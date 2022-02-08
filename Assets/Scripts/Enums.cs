
public enum Level : byte
{
    MainMenu,
    Game
}

public enum AudioArray : byte
{
    Null,
    UIHover,
    UIClick,
}

public enum PooledObject
{
    AudioSource
}

public enum GraphicsQualityLevels : byte
{
    Low,
    Medium,
    High,
    Ultra
}

public enum GameStage : byte
{
    MainMenu,
    Lobby,
    Game
}

public enum Direction : byte
{
    None,
    Forward,
    Backward,
    Left,
    Right,
}