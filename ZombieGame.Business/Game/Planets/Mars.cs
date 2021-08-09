using System;
using System.Collections.Generic;
using ZombieGame.Business.Game.Consumables;

namespace ZombieGame.Business.Game.Planets
{
    public class Mars : Planet
    {
        public static int Count = 40;

        public static string Name = "Mars";

        public static string Sprite = "";
        
        public Mars(int posX, int posY ) : base(posX, posY)
        {
            Health = 300;
            Consumables = new List<Consumable>();

            var random = new Random();
            
            Consumables.Add(new Ammo(posX,posY,random.Next(10,25)));
            Consumables.Add(new Ammo(posX,posY,random.Next(10,25)));
            
            Consumables.Add(new Stardust(posX,posY,random.Next(25,35)));
            Consumables.Add(new Stardust(posX,posY,random.Next(25,35)));
            Consumables.Add(new Stardust(posX,posY,random.Next(25,35)));
            
            Consumables.Add(new Health(posX,posY,random.Next(15,40)));
        }
    }
}