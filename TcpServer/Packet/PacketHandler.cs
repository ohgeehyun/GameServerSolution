using Google.Protobuf;
using Protocol;
using NetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TcpServer.Packet
{
    public class PacketHandler
    {
        public static void C_MoveHandler(PacketSession session, IMessage packet)
        {
            C_MOVE? chatPacket = packet as C_MOVE;
            ClientSession serverSession = session as ClientSession;

            Console.WriteLine(chatPacket?.PosInfo);
        }

        public static void C_SkillHandler(PacketSession session, IMessage packet)
        {
            C_MOVE? chatPacket = packet as C_MOVE;
            ClientSession serverSession = session as ClientSession;

            Console.WriteLine(chatPacket?.PosInfo);
        }

        public static void C_VerifyHandler(PacketSession session, IMessage packet)
        {
            C_MOVE? chatPacket = packet as C_MOVE;
            ClientSession serverSession = session as ClientSession;

            Console.WriteLine(chatPacket?.PosInfo);
        }

        public static void C_CreateRoomHandler(PacketSession session, IMessage packet)
        {
            C_MOVE? chatPacket = packet as C_MOVE;
            ClientSession serverSession = session as ClientSession;

            Console.WriteLine(chatPacket?.PosInfo);
        }

        public static void C_RoomListHandler(PacketSession session, IMessage packet)
        {
            C_MOVE? chatPacket = packet as C_MOVE;
            ClientSession serverSession = session as ClientSession;

            Console.WriteLine(chatPacket?.PosInfo);
        }

        public static void C_EnterGameHandler(PacketSession session, IMessage packet)
        {
            C_MOVE? chatPacket = packet as C_MOVE;
            ClientSession serverSession = session as ClientSession;

            Console.WriteLine(chatPacket?.PosInfo);
        }

        public static void C_MessageHandler(PacketSession session, IMessage packet)
        {
            C_MOVE? chatPacket = packet as C_MOVE;
            ClientSession serverSession = session as ClientSession;

            Console.WriteLine(chatPacket?.PosInfo);
        }

        public static void C_LeaveGameHandler(PacketSession session, IMessage packet)
        {
            C_MOVE? chatPacket = packet as C_MOVE;
            ClientSession serverSession = session as ClientSession;

            Console.WriteLine(chatPacket?.PosInfo);
        }
    }
}
