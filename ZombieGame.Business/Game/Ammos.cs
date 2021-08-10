namespace ZombieGame.Business.Game
{
    public static class Ammos
    {
        public static class IronAmmo
        {
            public static string Name = "Iron Ammo";
            public static int Level = 1;
            public static int Price = 0;
            public static int Damage = 20;
            public static int Speed = 40;
            public static string Sprite = "";
        }
        
        public class PiercingAmmo
        {
            public static string Name = "Piercing Ammo";
            public static int Level = 2;
            public static int Price = 500;
            public static int Damage = 30;
            public static int Speed = 40;
            public static string Sprite = "";
        }
        
        
        public class LazerAmmo
        {
            public static string Name = "Lazer Ammo";
            public static int Level = 3;
            public static int Price = 1000;
            public static int Damage = 35;
            public static int Speed = 50;
            public static string Sprite = "";
        }
    }
}