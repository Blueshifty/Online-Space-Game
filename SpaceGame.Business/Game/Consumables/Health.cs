namespace SpaceGame.Business.Game.Consumables
{
    public class Health : Consumable
    {
        public static string Name = "Health";

        public static string Sprite = "";
        public Health(int posX, int posY, int amount) : base(posX, posY, amount)
        {
        }

        public override void Consume(Player player)
        {
            player.Health += Amount;

            if (player.Health > player.HealthLimit)
            {
                player.Health = player.HealthLimit;
            }
        }
    }
}