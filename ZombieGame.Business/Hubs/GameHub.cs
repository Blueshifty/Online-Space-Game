using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ZombieGame.Business.DTOs.Request;
using ZombieGame.Business.DTOs.Response;
using ZombieGame.Business.Hubs.HubModels;
using ZombieGame.Business.Utilities.Mapper;

namespace ZombieGame.Business.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMapper _mapper;
        
        private static readonly Dictionary<string, string> PlayerRoomMap = new();

        private static readonly Dictionary<string, GameRoom> GameRooms = new();

        public static IHubContext<GameHub> HubContext;

        public static Timer Internal;

        public GameHub( IHubContext<GameHub> hubContext,IMapper mapper)
        {
            _mapper = mapper;
            HubContext = hubContext;
        }

        private static void GameLoop(object? state)
        {
            foreach (var gameRoom in GameRooms)
            {
                HubContext.Clients.Group(gameRoom.Key).SendAsync("update", gameRoom.Value.Players.Values);
            }
        }
        
        public async Task SendMove(MoveDto move)
        {
            try
            {
                var player = GameRooms[PlayerRoomMap[Context.ConnectionId]].Players[Context.ConnectionId];
                
                var oldPosX = player.PosX;

                var oldPosY = player.PosY;

                switch (move.Towards)
                {
                    case 1:
                        player.PosX--;
                        player.PosY--;
                        break;
                    case 2:
                        player.PosY -= 2;
                        break;
                    case 3:
                        player.PosX++;
                        player.PosY--;
                        break;
                    case 4:
                        player.PosX -= 2;
                        break;
                    case 6:
                        player.PosX += 2;
                        break;
                    case 7:
                        player.PosX--;
                        player.PosY++;
                        break;
                    case 8:
                        player.PosY += 2;
                        break;
                    case 9:
                        player.PosX++;
                        player.PosY++;
                        break;
                }
                
                if ( player.PosX is > 200 or < 0 || player.PosY is > 200 or < 0)
                {
                    player.PosX = oldPosX;
                    player.PosY = oldPosY;
                }
            }
            catch(Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "Error : " + ex.Message);
            }
        }
        
        public async Task JoinGame(JoinDto joinDto)
        {
            try
            {
                var player = new Player(joinDto.Name);

                PlayerRoomMap[Context.ConnectionId] = joinDto.RoomId;

                var gameRoom = GameRooms[joinDto.RoomId];
                
                if (gameRoom.CurrentPlayerCount < gameRoom.PlayerCount)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, joinDto.RoomId);

                    gameRoom.Players[Context.ConnectionId] = player;
                    
                    gameRoom.CurrentPlayerCount++;
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "You failed to join game room" + ex.Message);
            }
        }
        
        public async Task Leave(string roomId)
        {
            GameRooms[PlayerRoomMap[Context.ConnectionId]].Players.Remove(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
        public IEnumerable<RoomOnDashboardDto> GetGameRooms()
        {
            //Init rooms if not exists.
            if (GameRooms.Values.Count == 0)
            {
                for (var i = 1; i <= 10; ++i)
                {
                    var gameRoom = new GameRoom($"Game Room {i}", i * 10, i*10);
                    GameRooms[gameRoom.Id] = gameRoom;
                }
                Internal = new Timer(GameLoop, null, TimeSpan.FromMilliseconds(76), TimeSpan.FromMilliseconds(75));
            }
            return _mapper.Map<List<GameRoom>, List<RoomOnDashboardDto>>(GameRooms.Values.ToList());
        }
        
        private string GetDevice()
        {
            var device = Context.GetHttpContext().Request.Headers["Device"].ToString();
            if (!string.IsNullOrEmpty(device) && (device.Equals("Desktop") || device.Equals("Mobile")))
                return device;

            return "Web";
        }
    }
}