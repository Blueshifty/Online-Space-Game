namespace ZombieGame.Business.DTOs.Request
{
    public class RoomActionDto
    {
        public string RoomId { get; set; }
        public string Info { get; set; }
        public GameRoomAction Action { get; set; }
        
        public enum GameRoomAction
        {
            Delete,
            Start,
            ChangeName,
            KickPlayer
        }
    }
}