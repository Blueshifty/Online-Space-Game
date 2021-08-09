namespace ZombieGame.Business.Game.Ships
{
    public class BattleShip : Ship
    {
        public BattleShip()
        {
            Name = "Battle Ship";
            Level = 2;
            Price = 500;
            HitPoints = 250;
            Speed = 30;
            Sprite = "";
        }
    }
}