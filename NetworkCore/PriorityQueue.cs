using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{

    #region ##Notice
    //우선 순위 큐 최소 최대 힙을 바꾸고 싶다면 compareTo의 부호를 바꾸면 된다. 현재는 최소 힙
    #endregion
    public class PriorityQueue<T> where T: IComparable<T>
    {
        List<T> _heap = new List<T>();

        public int Count { get { return _heap.Count; } }

        //0(logN)
        public void Push(T data)
        {
            //힙의 맨 끝에 새로운 데이터를 삽입한다.
            _heap.Add(data);

            int now = _heap.Count - 1; //맨끝 인덱스에서 시작
            //도장깨기를 시작
            while (now > 0)
            {
                //부모 노드의 위치 공식
                int next = (now - 1) / 2;
                if (_heap[now].CompareTo(_heap[next]) < 0)
                    break;

                //두 값을 교체
                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                //검사 위치를 이동한다
                now = next;
            }

        }
        public T Pop()
        {
            //반환할 데이터를 따로 저장 우선순위 큐 특성상 0번에 가장 큰값 또는 작은 값 
            //최소 힙인지 최대 힙 기반인지는 언어에 따라 다르다 c++랑 c#은 반대 였다.
            T ret = _heap[0];

            // 마지막 데이터를 루트로 이동한다. 최상위 노드에서 뺴버리면 트리구조가 망가지게 된다.
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            lastIndex--;

            //역으로 내려가는 도장깨기 시작
            int now = 0;
            while(true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;
                //왼쪽값이 현재값 보다크면 , 왼쪽으로 이동
                if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left;
                // 오른값이 현재값(왼쪽 이동 포함)보다 크면 , 오른쪽으로 이동
                if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right;

                // 왼쪽/오른쪽 모두 현재값보다 작으면 종료
                if (next == now)
                    break;

                // 두 값을 교체한다
                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;
                // 검사 위치를 이동한다
                now = next;
            }
            return ret;
        }

        public T Peek()
        {
            if (_heap.Count == 0)
                return default(T);//해당 타입의 기본 값 반환
            return _heap[0];
        }

    }
}
