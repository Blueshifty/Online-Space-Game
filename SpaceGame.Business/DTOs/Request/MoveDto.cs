using SpaceGame.Business.Game;

namespace SpaceGame.Business.DTOs.Request
{
    public class MoveDto
    {
        public Towards Towards { get; set; }
        public bool KeyState { get; set; }
    }
}