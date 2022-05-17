public class Player
{
    public int Id { get; }

    public int GameNumber { get; private set; }

    public Player(int id)
    {
        Id = id;
    }

    public void DrawNumber()
    {
        var random = new Random();
        GameNumber = random.Next();
    }
}