using System.Net;
using System.Net.Sockets;

namespace NetworkCore
{
    public enum DisconnectReason
    {
        Graceful,           // 명시적 종료
        Error,              // 소켓 오류
        ProtocolError,      // 프로토콜 위반
        RemoteClosed,       // 상대방 종료
        Manual,             // 수동 종료
    }

    public abstract class PacketSession : Session
    {
        //TODO : 패킷 구조 따른 음 헤더 사이즈는 변경 필수 인듯
        public static readonly int HeaderSize = 2;


        //인자로 CompleteRecv에서 recvbuffer의 read커서에서 write커서까지의 데이터 부분을 전달
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            int packetCount = 0;

            while (true)
            {
                //헤더는 파싱 할수 있어야한다.
                if (buffer.Count < HeaderSize)
                    break;

                //패킷의 앞부분 2바이트를 읽어 패킷의 데이터 부분 크기와 읽어야할 버퍼의 크기를 비교 datasize가 더 크다면 말도 안되는 상황
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                //패킷 조립
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;

                //처리한 데이터의 사이즈(패킷하나)만큼 길이 저장  처리한 버퍼안의 데이터 만큼 처리
                processLen += dataSize;
                buffer = new ArraySegment<byte>(
                    buffer.Array,
                    buffer.Offset + dataSize,
                    buffer.Count - dataSize
                    );

            }

            if (packetCount > 1)
                Console.WriteLine($"패킷 모아보내기 : {packetCount}");
            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, buffer.Count));
            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    #region ##Notice
    // Socket을 감싸는 Session클래서 하나의 소켓을 Session 클래스로 감싸서 관리
    #endregion
    public abstract class Session
    {
        Socket? _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(65535);

        object _lock = new object();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
       

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        void Clear()
        {
            lock(_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(List<ArraySegment<byte>> sendBuffList)
        {
            if (sendBuffList.Count == 0)
                return;

            lock (_lock)
            {
                foreach (ArraySegment<byte> sendBuff in sendBuffList)
                    _sendQueue.Enqueue(sendBuff);

                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect(DisconnectReason reason = DisconnectReason.Graceful, SocketAsyncEventArgs? args = null)
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            try
            {
                if (_socket?.RemoteEndPoint is IPEndPoint ip)
                {
                    Console.WriteLine($"[Disconnect] IP: {ip.Address}, Port: {ip.Port}, Reason: {reason}, SocketError: {args?.SocketError}");
                }

                OnDisconnected(_socket?.RemoteEndPoint);

                try { _socket?.Shutdown(SocketShutdown.Both); } catch { }
                _socket?.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Disconnect Exception] Error: {e}");
            }

            Clear();
        }



        #region 송수신 관련 함수
        void RegisterSend()
        {
            //이미 연결이 다른 곳에서 DisConnect를 호출하여 연결 종료 실행중인 상태
            if (_disconnected == 1)
                return;

            while(_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }
            //send 비동기 이벤트 버퍼리스트에 연결
            _sendArgs.BufferList = _pendingList;

            try
            {
                //Socket의 SendAsync 비동기 send이벤트 호출 후 반환값이 true면 아직 처리중 false면 바로 처리 되었다는 뜻이라 바로 완료 함수 호출.
                //send event가 처리 되었는 결과 에 대한 true false가 아님.. 동기적으로 실행되었는가 비동기적으로 실행되었는가 에 대한 true false임
                //내부적으로 동기적으로 바로처리가능하면 false 반환 동기적으로 실행되었다는 뜻. 고로 다음 코드실행시점에선 이미 작업이 완료되었다는뜻.
                //비동기일경우는 true를 반환 바로 OnSendCompleted를 호출하는게 아니라 start()함수에서  _sendArgs.Completed 등록해둔 에 완료 이벤트를 등록해준 곳에서 완료시 호출 해줄 것이다.
                bool pending = _socket.SendAsync(_sendArgs);
                if (pending == false)
                    OnSendCompleted(null, _sendArgs);
            }
            catch(Exception e)
            {
                Console.WriteLine($"RegisterSendFailed : {e}");
            }
        }


        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                //처리한 바이트 수가 0 이상이고 SocketError에서 success일때 로직실행 
                //이름 때문에 헷갈렸는데 error코드만 아니라 성공도 여기서 정의함.
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        //보내야 할 데이터 버퍼를 초기화 전송이 완료된것이기때문에 pendingList 초기화
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        //자식 클래스에서 OnSend 호출 인자로 처리한 데이터 수
                        OnSend(_sendArgs.BytesTransferred);

                        //sendqueue가 아직 비어있지 않다면 다시 RegisterSend를 등록 하여 보낼 것을 처리
                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed : {e}");
                    }
                }
                else
                {
                    //여기서 sucess가아닌 Socket에 대한 에러가 뜨면 처리 상세하게 처리 해주면 좋을듯 한데 코드 길어지면 if else 제거하는 방향
                    Disconnect();
                }
            }
        }

        void RegisterRecv()
        {
            if (_disconnected == 1)
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            //writeSegment로 받은 현재 사용가능한 버퍼를 _recvArgs  의 버퍼로 등록
            _recvArgs.SetBuffer(segment.Array,segment.Offset,segment.Count);

            try
            {
                bool  pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false)
                    OnRecvCompleted(null, _recvArgs);

            }
            catch(Exception e)
            {
                Console.WriteLine($"OnRecvCompleted Failed : {e}");
            }

        }

        void OnRecvCompleted(object sender,SocketAsyncEventArgs args)
        {
            //여기서 BytesTransferred는 응용 층 에서의 처리를 말하는 것 os레벨에서 붙는 헤더나 연결 유지용 하트비트 데이터는 포함되지 않는다.
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    //처리한 바이트수가 buffer의 크기를 넘어 설 수 없음 당연히 예외발생.
                    if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    //상속받아 사용하는 Session에서 OnRecv를 호출하여 얼마나 데이터를 처리했는지 주고 받는다
                    //음수 일 수 없음 또한 버퍼에저장된 데이터영역 보다 많은 부분을 처리 할 리가 없다. 예외발생
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if (processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    // Read 커서 이동
                    //문제없이 다 처리 하였다면 컨텐츠가 사용한 만큼 recv커서 당겨줄 것
                    if (_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed : {e}");
                }
            }
            else
            {
                Disconnect(DisconnectReason.RemoteClosed, args);
            }
        }


        #endregion
    }
}
