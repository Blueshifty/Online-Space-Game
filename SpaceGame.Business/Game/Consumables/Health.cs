namespace SpaceGame.Business.Game.Consumables
{
    public class Health : Consumable
    {
        public Health(int posX, int posY, int amount) : base(posX, posY, amount)
        {
            ConsumableIndex = 1;
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