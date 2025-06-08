using Google.Protobuf;
using Protocol;
using NetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomServer.Manager;
using System.Security.Claims;
using System.Reflection.Metadata;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using RoomServer.Manager.Auth;


namespace RoomServer.Packet
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
            //패킷이 null일 경우에는 서버 구조나 프로토버퍼에 문제가 있다. 차라리 crash나오는게 편하다 생각.
            C_VERIFY? VerifyPacket = packet as C_VERIFY;

            //캐스팅 후  null 검사
            if (session is not ClientSession clientSession)
                return;

            if (VerifyPacket == null || string.IsNullOrEmpty(VerifyPacket.Jwt))
                return;

            //JWT 검증
            var principal = GlobalManager.Instance.jwtValidator.Validatetoken(VerifyPacket.Jwt);

            if (principal == null)
            {
                Console.WriteLine("JWT 검증 실패. 연결 종료.");
                clientSession.Disconnect();
                return;
            }

            //검증 성공: ClaimsPrincipal에서 사용자 정보 추출 가능
            //필요시 토큰의 아이디 를 사용한 값 저장 따로 매핑 시켜주는 부분을 작성하여 사용중 그로 인해 추가 자료구조가 생기긴 하였음 jwt에 몇개 들어가지 않는다면 하드코딩이 효율은 더 좋아보임..

            //clientSession.user.userId = string.IsNullOrEmpty(userId) ? string.Empty : userId;
            //clientSession.user.userNick = string.IsNullOrEmpty(nickname) ? string.Empty : nickname;
            //clientSession.user.jwtId = string.IsNullOrEmpty(jti) ? string.Empty : jti;

            JwtUtils.MapClaimsToUser(clientSession.user, principal);

            S_VERIFY resultPacket = new S_VERIFY();
            resultPacket.Result = true;
            resultPacket.Userid = clientSession.user.userId;
            resultPacket.Nickname = clientSession.user.userNick;
   
            clientSession.Send(resultPacket);

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
