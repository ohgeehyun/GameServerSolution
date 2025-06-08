using Google.Protobuf;
using NetworkCore;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using RoomServer.Packet;
using RoomServer.Session;
using RoomServer.UserData;


namespace RoomServer
{
    #region ## notice
    //클라이언트는 서버 오픈시 연결완료 이후 JWT를 줄 것이고 검증이 통과되면 연결을 지속하지만 아닐 경우 close를 호출 할 것이다.
    #endregion
    public class ClientSession : PacketSession
    { 
        public User user { get; set; } = new User();

        public int SessionId { get; set; }

        public void Send(IMessage packet)
        {
          
            MsgId msgId = Enum.Parse<MsgId>(Utils.TransPacketNameForPacketEnum(packet.Descriptor.Name));
            ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
			Send(new ArraySegment<byte>(sendBuffer));
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

            PacketManager.Instance.OnRecvPacket(this, buffer);

            //string data = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            //Console.WriteLine($"받은 패킷 데이터 : {data}");

            //Send(buffer);

            //var client = new HttpClient();
            //var response = await client.GetAsync("http://api:5251/api/test/alive");
            //var result = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"Api 응답:{result}");
           
        }

        public override void OnSend(int numOfBytes)
        {
            //클라이언트에게 전송 하고 난 뒤 이벤트 가 필요할시 여기에 작성
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }
    }
}
