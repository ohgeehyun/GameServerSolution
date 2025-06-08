using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using NetworkCore;
using Protocol;

namespace RoomServer.Packet
{
    class PacketManager
    {
        #region Singleton
        static PacketManager _instance = new PacketManager(); 
        public static PacketManager Instance { get { return _instance; } }
        #endregion

        Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
        Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

        PacketManager() 
        {
            Register();
        }

        public void Register()
        {
            //시작시 패킷정보에 에 대한 데이터 를 등록
            	
        _onRecv.Add((ushort)MsgId.CMove, MakePacket<C_MOVE>);
		_handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);	
        _onRecv.Add((ushort)MsgId.CSkill, MakePacket<C_SKILL>);
		_handler.Add((ushort)MsgId.CSkill, PacketHandler.C_SkillHandler);	
        _onRecv.Add((ushort)MsgId.CVerify, MakePacket<C_VERIFY>);
		_handler.Add((ushort)MsgId.CVerify, PacketHandler.C_VerifyHandler);	
        _onRecv.Add((ushort)MsgId.CCreateRoom, MakePacket<S_CREATE_ROOM>);
		_handler.Add((ushort)MsgId.CCreateRoom, PacketHandler.C_CreateRoomHandler);	
        _onRecv.Add((ushort)MsgId.CRoomList, MakePacket<C_ROOM_LIST>);
		_handler.Add((ushort)MsgId.CRoomList, PacketHandler.C_RoomListHandler);	
        _onRecv.Add((ushort)MsgId.CEnterGame, MakePacket<C_ENTER_GAME>);
		_handler.Add((ushort)MsgId.CEnterGame, PacketHandler.C_EnterGameHandler);	
        _onRecv.Add((ushort)MsgId.CMessage, MakePacket<C_MESSAGE>);
		_handler.Add((ushort)MsgId.CMessage, PacketHandler.C_MessageHandler);	
        _onRecv.Add((ushort)MsgId.CLeaveGame, MakePacket<C_LEAVE_GAME>);
		_handler.Add((ushort)MsgId.CLeaveGame, PacketHandler.C_LeaveGameHandler);

        }

        public void OnRecvPacket(PacketSession session,ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array!, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array!, buffer.Offset + count);
            count += 2;

            Action<PacketSession, ArraySegment<byte>, ushort> action = null;
            if (_onRecv.TryGetValue(id, out action))
                action!.Invoke(session,buffer,id);
        }

        void  MakePacket<T>(PacketSession Session,ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
        {
            T pkt = new T();
            pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count -4);
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                action.Invoke(Session, pkt);
        }

        public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
        {
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                return action;

            return null;
        }
        

    }
}
