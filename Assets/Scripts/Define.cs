public enum PlayerState
{
    Idle,
    Run,
    Jump,
    Slide,
    Hit,
    Dead,
    Fever,
}

public enum KeyType
{
    None = -1,
    Up,
    Right,
    Down,
    Left
}

public enum ObstacleContainerLevel
{
    Random,
    Low,
    Medium,
    High
}

public enum FlowState
{
    None,
    Lobby,
    Intro,
    Play,
    Ending,
    Credit
}