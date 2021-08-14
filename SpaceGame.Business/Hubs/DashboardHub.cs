#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SpaceGame.Business.Application;
using SpaceGame.Business.DTOs.Event;
using SpaceGame.Business.DTOs.Request;
using SpaceGame.Business.Utilities;
using static SpaceGame.Business.Game.Dictionaries;

namespace SpaceGame.Business.Hubs
{
    public class DashboardHub: Hub
    {

        private readonly IHubContext<DashboardHub> _hubContext;
        private ApplicationContext ApplicationContext { get; }
        public DashboardHub(IHubContext<DashboardHub> hubContext, ApplicationContext context)
        {
            _hubContext = hubContext;
            ApplicationContext = context;
            context.ListenEvent(typeof(JoinOrLeaveEvent), EventHandle);
        }

        private async void EventHandle(object? sender, EventArgs e)
        {
            // var @event = (JoinOrLeaveEvent) e;  Kullanacagımız senaryolarda cast ederek kullanabiliriz.
            await this.OnSend();
        }

        public async Task OnSend()
        {
            await _hubContext.Clients.All.SendAsync("onInit", StatisticUtil.Statistics(GameRooms, PlayerRoomMap));
        }

        public void OnDisconnectUser(DisconnectUser user)
        {
            ApplicationContext.FireEvent(this, new DisconnectUserEvent(user));
        }
    

        public override async Task OnConnectedAsync()
        {
            await this.OnSend();
        }
    }
}