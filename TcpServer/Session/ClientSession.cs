using NetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TcpServer.UserData;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpServer
{
    public class ClientSession : PacketSession
    { 
        public User user { get; set; }

        public int SessionId { get; set; }

        public void Send(char[] message)
        {

        }

        public override void OnConnected(EndPoint endPoint)
        {
            if (endPoint is IPEndPoint ipEndPoint)
            {
                Console.WriteLine($"클라이언트 연결됨 - IP: {ipEndPoint.Address}, 포트: {ipEndPoint.Port}");
            }
        }

        public override async void OnRecvPacket(ArraySegment<byte> buffer)
        {
            string data = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"받은 패킷 데이터 : {data}");

            Send(buffer);

            var client = new HttpClient();
            var response = await client.GetAsync("http://api:5251/api/alive");
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Api 응답:{result}");
           
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
