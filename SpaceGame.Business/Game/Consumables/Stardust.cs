namespace SpaceGame.Business.Game.Consumables
{
    public class Stardust : Consumable
    {        
        public Stardust(int posX, int posY, int amount) : base(posX, posY, amount)
        {
            ConsumableIndex = 0;
        }

        public override void Consume(Player player)
        {
            player.Stardust += Amount;
        }
    }
}