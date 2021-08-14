namespace SpaceGame.Business.Game.Consumables
{
    public class Health : Consumable
    {
        public static string Name = "Health";

        public static string Sprite = "";
        public Health(int posX, int posY, int amount) : base(posX, posY, amount)
        {
        }
    }
}