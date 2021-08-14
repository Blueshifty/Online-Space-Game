using System;
using System.Collections.Generic;
using SpaceGame.Business.Game.Consumables;

namespace SpaceGame.Business.Game.Planets
{
    public class Venus: Planet
    {

        public static int Count = 50;

        public static string Name = "Venus";

        public static string Sprite = "";
        
        public Venus(int posX, int posY): base(posX,posY)
        {
            Health = 200;
            Consumables = new List<Consumable>();
            
            var random = new Random();
            
            Consumables.Add(new Ammo(posX,posY,random.Next(5,20)));
            Consumables.Add(new Ammo(posX,posY,random.Next(5,20)));
            
            Consumables.Add(new Stardust(posX,posY,random.Next(10,30)));
            Consumables.Add(new Stardust(posX,posY,random.Next(15,45)));
            Consumables.Add(new Stardust(posX,posY,random.Next(20,50)));
            
            Consumables.Add(new Health(posX,posY,random.Next(20,50)));
        }
    }
}