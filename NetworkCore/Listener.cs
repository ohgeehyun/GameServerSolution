using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{
    public class Listener
    {
        Socket? _listenSocket;
        Func<Session>? _SessionFactory;
        private bool _initialized = false;

        //listen 소켓 초기화
        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _SessionFactory += sessionFactory;

            //liten소켓  바인딩
            _listenSocket.Bind(endPoint);

            //listen(대기 상태)
            _listenSocket.Listen(backlog);

            //초기화 완료 
            _initialized = true;

            //register로 미리 비동기 accept이벤트 를 만들어서 등록
            for (int i = 0; i<register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        //accept 이벤트 등록
        void RegisterAccept(SocketAsyncEventArgs args)
        {
            if (_initialized == false)
                Environment.FailFast("listener 소켓이 초기화가 되지 않음.");

            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null,args);
        }

        //accept 처리후 호출 될 함수
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (_initialized == false)
                Environment.FailFast("listener 소켓이 초기화가 되지 않음.");

            //소켓 비동기 이벤트가 성공일때 해당 completed 로직 실행
            if (args.SocketError == SocketError.Success)
            {
                Session session = _SessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.Write(args.SocketError.ToString());

            RegisterAccept(args);
        }
    }
}
