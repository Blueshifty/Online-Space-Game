namespace ZombieGame.Business.Game.Projectiles
{
    public class Bullet : GameEntity
    {
        public Player Shooter { get; set; }
        
        public int Speed { get; set; }
        
        public Towards Towards { get; set; }
        
        public int Damage { get; set; }
        
        public int TowardAngle { get; set; }
        
        public string Sprite { get; set;}
        
        public Bullet(Player shooter, int speed, int damage, int towardAngle, Towards towards, string sprite, int posX, int posY) : base(posX, posY)
        {
            Shooter = shooter;

            Speed = speed;

            Damage = damage;

            TowardAngle = towardAngle;
            
            Towards = towards;

            Sprite = sprite;
        }
    }
}