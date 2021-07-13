namespace ZombieGame.Business.Hubs.HubModels
{
    public class Player
    {
        public string Name { get; set; }
        public int PosX { get; set;}
        public int PosY { get; set; }

        public Player(string name)
        {
            Name = name;
        }
    }
}