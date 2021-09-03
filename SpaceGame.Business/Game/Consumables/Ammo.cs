namespace SpaceGame.Business.Game.Consumables
{
    public class Ammo : Consumable
    {

        public static string Name = "Ammo";

        public static string Sprite = "";
        
        public Ammo(int posX, int posY, int amount) : base(posX, posY,amount)
        {
        }

        public override void Consume(Player player)
        {
            player.AmmoCount += Amount;
            if (player.AmmoCount > player.AmmoLimit)
            {
                player.AmmoCount = player.AmmoLimit;
            }
        }
    }
}