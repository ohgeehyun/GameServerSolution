using Google.Protobuf;
using NetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Packet;
using TcpServer.Session;

namespace TcpServer
{
    #region ## notice
     //RoomServer와 연결된 세션
    #endregion
    public class RoomSession : PacketSession
    {
        public void Send(IMessage packet)
        {

     
        }

        public override void OnConnected(EndPoint endPoint)
        {
            if (endPoint is IPEndPoint ipEndPoint)
            {
                Console.WriteLine($"룸서버 와 연결됨 - IP: {ipEndPoint.Address}, 포트: {ipEndPoint.Port}");
            }
        }

        public override async void OnRecvPacket(ArraySegment<byte> buffer)
        {

        }

        public override void OnSend(int numOfBytes)
        {
            //클라이언트에게 전송 하고 난 뒤 이벤트 가 필요할시 여기에 작성
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
          
        }
    }
}
