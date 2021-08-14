namespace SpaceGame.Business.Game.Consumables
{
    public class Consumable : GameEntity
    {
        public int Amount { get; set; }

        protected Consumable(int posX, int posY, int amount) : base(posX, posY)
        {
            Amount = amount;
        }
    }
}