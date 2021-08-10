namespace ZombieGame.Business.Game
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
        
        public int Health { get; set;}
        
        public int HealthLimit { get; set;}
        
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