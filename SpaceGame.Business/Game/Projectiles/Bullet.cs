namespace SpaceGame.Business.Game.Projectiles
{
    public class Bullet : GameEntity
    {
        public string ShooterId { get; set; }
        
        public int Speed { get; set; }
        
        public Towards Towards { get; set; }
        
        public int Damage { get; set; }
        
        public int TowardAngle { get; set; }
        
        public string Sprite { get; set;}
        
        public Bullet(string shooterId, int speed, int damage, int towardAngle, Towards towards, string sprite, int posX, int posY) : base(posX, posY)
        {
            ShooterId = shooterId;

            Speed = speed;

            Damage = damage;

            TowardAngle = towardAngle;
            
            Towards = towards;

            Sprite = sprite;
        }
    }
}