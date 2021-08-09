using System;
using System.Collections.Generic;
using ZombieGame.Business.Game.Consumables;

namespace ZombieGame.Business.Game.Planets
{
    public class Jupiter : Planet
    {
        public static int Count = 20;

        public static string Name = "Jupiter";

        public static string Sprite = "";
        
        public Jupiter(int posX, int posY) : base(posX,posY)
        {
            Health = 1000;
            Consumables = new List<Consumable>();

            var random = new Random();
            
            Consumables.Add(new Ammo(posX,posY,random.Next(20,50)));
            Consumables.Add(new Ammo(posX,posY,random.Next(20,50)));
            
            Consumables.Add(new Stardust(posX,posY,random.Next(50,75)));
            Consumables.Add(new Stardust(posX,posY,random.Next(75,100)));
            Consumables.Add(new Stardust(posX,posY,random.Next(100,125)));
            
            Consumables.Add(new Health(posX,posY,random.Next(50,100)));
            Consumables.Add(new Health(posX,posY,random.Next(100,150)));
        }
    }
}