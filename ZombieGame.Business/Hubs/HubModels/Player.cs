namespace ZombieGame.Business.Hubs.HubModels
{
    public class Player
    {
        public string Name { get; set; }
        public int PosX { get; set;}
        public int PosY { get; set; }
        public Towards Towards { get; set;}

        public Player(string name)
        {
            Towards = Towards.NOWHERE;
            Name = name;
        }
    }


    public enum Towards
    {
        NOWHERE,
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }
}