namespace ZombieGame.Business.Game.Consumables
{
    public class Ammo : Consumable
    {

        public static string Name = "Ammo";

        public static string Sprite = "";
        
        public Ammo(int posX, int posY, int amount) : base(posX, posY,amount)
        {
        }
    }
}