using System.Collections.Generic;

namespace ZombieGame.Business.DTOs.Response
{
    public class StatisticsDto
    {
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        public int UserCount { get; set; }
        public List<UserDto> Users { get; set; }
        
    }

    public class UserDto
    {
        public string ConnectionId { get; set; }
        public string IpV4 { get; set; }
        public string Username { get; set; }
    }
}