namespace SpaceGame.Business.Game.Consumables
{
    public class Stardust : Consumable
    {
        public static string Name = "Stardust";

        public static string Sprite = "";
        
        public Stardust(int posX, int posY, int amount) : base(posX, posY, amount)
        {
        }

        public override void Consume(Player player)
        {
            player.Stardust += Amount;
        }
    }
}