using ZombieGame.Business.Game;

namespace ZombieGame.Business.DTOs.Request
{
    public class MoveDto
    {
        public Towards Towards { get; set; }
        public bool KeyState { get; set; }
    }
}