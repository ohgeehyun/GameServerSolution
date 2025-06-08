using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomServer.Session
{
    #region ##Notice
    //세션들을 관리해줄 세션 매니저
    #endregion
    internal class SessionManager
    {
        static SessionManager _Instance = new SessionManager();

        public static SessionManager Instance { get { return _Instance; } }

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int,ClientSession>();
       
       private object _lock = new object();

        //세션 생성 후 세션에 정보 등록
        public ClientSession Generator()
        {
            lock (_lock)
            {
                int sessionId = ++_sessionId;

                ClientSession session = new ClientSession();
                session.SessionId = sessionId;
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Connected : sessionId_{sessionId}");

                return session;
            }
        }

        //null을 값이 없으면 반환 해주기 때문에 사용하는 쪽에서는 null검사하여 사용
        public ClientSession? Find(int id)
        {
            lock(_lock)
            {
                ClientSession? session = null;
                _sessions.TryGetValue(id,out session);

                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock(_lock)
            {
                _sessions.Remove(session.SessionId);
            }
        }

        
    }
}
