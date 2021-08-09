using System;
using System.Collections.Generic;
using ZombieGame.Business.Game.Consumables;

namespace ZombieGame.Business.Game.Planets
{
    public class Saturn : Planet
    {
        
        public static int Count = 30;
        
        public static string Name = "Saturn";

        public static string Sprite = "";
        
        public Saturn(int posX, int posY) : base(posX,posY)
        {
            Health = 500;
            Consumables = new List<Consumable>();

            var random = new Random();
            
            Consumables.Add(new Ammo(posX,posY,random.Next(10,25)));
            Consumables.Add(new Ammo(posX,posY,random.Next(10,25)));
            
            Consumables.Add(new Stardust(posX,posY,random.Next(25,50)));
            Consumables.Add(new Stardust(posX,posY,random.Next(35,60)));
            Consumables.Add(new Stardust(posX,posY,random.Next(45,70)));
            
            Consumables.Add(new Health(posX,posY,random.Next(35,75)));
        }
    }
}