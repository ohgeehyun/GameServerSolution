using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
    class PacketFormat
    {
        //{0} 패킷 등록
        public static string managerFormat =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using NetworkCore;

namespace TcpServer.Packet
{{
    class PacketManager
    {{
        #region Singleton
        static PacketManager _instance = new PacketManager(); 
        public static PacketManager Instance {{ get {{ return _instance; }} }}
        #endregion

        Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
        Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

        PacketManager() 
        {{
            Register();
        }}

        public void Register()
        {{
            //시작시 패킷정보에 에 대한 데이터 를 등록
            {0}

        }}

        public void OnRecvPacket(PacketSession session,ArraySegment<byte> buffer)
        {{
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array!, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array!, buffer.Offset + count);
            count += 2;

            Action<PacketSession, ArraySegment<byte>, ushort> action = null;
            if (_onRecv.TryGetValue(id, out action))
                action!.Invoke(session,buffer,id);
        }}

        void  MakePacket<T>(PacketSession Session,ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
        {{
            T pkt = new T();
            pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count -4);
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                action.Invoke(Session, pkt);
        }}

        public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
        {{
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                return action;

            return null;
        }}
        

    }}
}}
";
        //{0} MsgId
        //{1} 패킷name
        public static string managerRegisterFormat =
@"	
        _onRecv.Add((ushort)MsgId.{0}, MakePacket<{1}>);
		_handler.Add((ushort)MsgId.{0}, PacketHandler.{1}Handler);";

    }
}
