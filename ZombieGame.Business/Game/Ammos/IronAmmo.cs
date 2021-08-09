using Microsoft.Win32.SafeHandles;

namespace ZombieGame.Business.Game.Ammos
{
    public class IronAmmo: Ammo
    {
        public IronAmmo()
        {
            Name = "Iron Ammo";
            Level = 1;
            Price = 0;
            Damage = 20;
            Speed = 40;
            Sprite = "";
        }
    }
}