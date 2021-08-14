namespace SpaceGame.Business.Game
{
    public class GameEntity
    { 
        public int PosX { get; set; }
        public int PosY { get; set; }

        protected GameEntity(int posX, int posY)
        {
            PosX = posX;
            PosY = posY;
        }
        
    }
}