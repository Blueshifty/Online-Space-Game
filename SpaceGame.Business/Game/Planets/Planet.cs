using System.Collections.Generic;
using SpaceGame.Business.Game.Consumables;

namespace SpaceGame.Business.Game.Planets
{
    public class Planet : GameEntity
    {
        public List<Consumable> Consumables { get; set; }
        public  int Health { get; set; }

        protected Planet(int posX, int posY) : base(posX, posY)
        {
        }
    }
}