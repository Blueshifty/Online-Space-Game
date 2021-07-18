using ZombieGame.Business.Hubs.HubModels;

namespace ZombieGame.Business.DTOs.Request
{
    public class MoveDto
    {
        public Towards Towards { get; set; }
        public bool KeyState { get; set; }
    }
}