namespace SpaceGame.Business.Game.Consumables
{
    public class Consumable : GameEntity
    {
        public int Amount { get; set; }

        public byte ConsumableIndex {get; set;}

        protected Consumable(int posX, int posY, int amount) : base(posX, posY)
        {
            Amount = amount;
        }

        public virtual void Consume(Player player)
        {
            /*
             * What happens when player consumes it ?
             */
        }
        
    }
}