using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{
    #region ##notice
    //[r][][][][w][][][][][] 순환 버퍼 또는 원형 버퍼라고 불리는 방식 사용 
    //Session마다 가지게 되는 Recv용 버퍼
    #endregion
    class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        //생성자
        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize],0, bufferSize);
        }

        //버퍼에 들어 있는 데이터 사이즈 반환
        public int DataSize { get { return _writePos - _readPos; } }

        //버퍼의 남은 공간 반환
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        public ArraySegment<byte> ReadSegment
        {
            //생성자에서 _buffer를 초기화해주기 때문에 null아닌데 컴파일러가 null일수 있다는 조언을 주어 !처리 
            get { return new ArraySegment<byte>(_buffer.Array!, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array!, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean()
        {
            // TODO : 로직적으로는 readPos 및 writePos의 위치가 맞다면 이상이 없다.
            // 하지만 만약 진짜 일어 날 수 없는 확률로 커서가 잘못되어 _buffer의 array보다 복사할 데이터의 길이가 크다면? 런타임 에러 
            // 혹여나 진짜 뜬다면 검증 및 예외처리 추가
            int dataSize = DataSize;
            if(dataSize == 0)
            {
                //남은 데이터가 없으니 read , write 둘다 0번위치로 옮기기 read랑 write커서의 위치가 같다는 뜻
                _readPos = _writePos = 0;
            }
            else
            {
                //남은 데이터가 있으면 시작 위치로 복사
                //복사할 버퍼 , 복사 시작할 위치, 복사가 되는 버퍼, 복사가 되는 위치 , 복사할 길이 
                Array.Copy(_buffer.Array!,_buffer.Offset + _readPos, _buffer.Array!, _buffer.Offset, dataSize);
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }
    }


}
