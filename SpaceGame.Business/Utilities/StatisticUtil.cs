using System.Collections.Generic;
using System.Linq;
using SpaceGame.Business.DTOs.Response;
using SpaceGame.Business.Game;

namespace SpaceGame.Business.Utilities
{
    public class StatisticUtil
    {
        public static List<StatisticsDto> Statistics(Dictionary<string, GameRoom> gameRooms,
            Dictionary<string, string> playerRoomMap)
        {
            List<StatisticsDto> statistics = new List<StatisticsDto>();
            foreach (var (key, value) in gameRooms)
            {
                StatisticsDto statistic = new StatisticsDto();
                statistic.RoomId = key;
                statistic.RoomName = value.Name;
                statistic.UserCount = value.Players?.Count ?? 0;
                statistic.Users = value.Players?.Select((x) => new UserDto
                {
                    Username = x.Value.Name,
                    ConnectionId = x.Key,
                    IpV4 = null
                }).ToList();
                
                statistics.Add(statistic);
            }

            return statistics;
        }
    }
}