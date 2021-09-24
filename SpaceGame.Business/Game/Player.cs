namespace SpaceGame.Business.Game
{
    public class Player
    {
        public string Name { get; set; }
        public int PosX { get; set;}
        public int PosY { get; set; }
        
        public Towards Towards { get; set;}
        
        public int TowardsAngle { get; set; }

        public int Speed { get; set; } = 10;
        
        public int SpeedLimit { get; set; }

        public int Health { get; set; } = 100;

        public int HealthLimit { get; set; } = 100;

        public int AmmoLimit { get; set; } = 300;

        public int AmmoCount { get; set; } = 100;

        public int Stardust { get; set; } = 0;

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