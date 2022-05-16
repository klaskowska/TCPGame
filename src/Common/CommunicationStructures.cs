namespace Common;

public struct StartMessage
{
    public int PlayerId { get; set; }

    public string Message { get; set; }

    public StartMessage(int playerId, string message)
    {
        PlayerId = playerId;
        Message = message;
    }
}

public struct GameNumber
{
    public int Number { get; set; }

    public GameNumber(int number)
    {
        Number = number;
    }
}

public struct Result
{
    public int PlayerId { get; set; }

    public Result(int playerId)
    {
        PlayerId = playerId;
    }
}