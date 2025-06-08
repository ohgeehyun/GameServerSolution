using NetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Manager
{
    public class RoomServerManager
    {
        private readonly RoomServerLauncher _launcher;
        private readonly Connector _connector;

        public RoomServerManager(RoomServerLauncher launcher, Connector connector)
        {
            _launcher = launcher;
            _connector = connector;
        }

        public async Task CreateAndConnectRoom(string roomId)
        {
            var podName = await _launcher.LaunchRoomServerAsync(roomId);
            await _launcher.WaitForRoomReadyAsync(podName);

            var ip = "125.137.11.149";
            var endPoint = new IPEndPoint(IPAddress.Parse(ip), 31005);

            _connector.Connect(endPoint, () => new RoomSession());
        }
    }
}
