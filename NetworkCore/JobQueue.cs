using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{
    #region ##Notice
    //일감 단위로 함수를 패키징 하여 큐에 저장하여 큐에서 순차적으로 처리 해 비동기 환경에서 순서보장 및 무분별한 lock을 줄인다.
    #endregion
    public interface IJobQueue
    {
        void Push(Action job);
    }

    public class JobQueue : IJobQueue
    {
        Queue<Action> _jobQueue = new Queue<Action>();
        object _lock = new object();
        bool _flush = false;

        //IJobQueue의 구조 충족
        public void Push(Action job) 
        {
            bool flush = false;

            lock(_lock)
            {
                _jobQueue.Enqueue(job);
                if (_flush == false)
                    flush = _flush = true;
            }

            if (flush)
                Flush();
        }

        void Flush()
        {
            while (true)
            {
                // 큐에 들어있는 action을 순차적으로 처리
                Action action = Pop();
                if (action == null)
                    return;

                action.Invoke();
            }
        }

        Action Pop()
        {
            lock(_lock)
            {
                if(_jobQueue.Count == 0)
                {
                    _flush = false;
                    return null;
                }
                return _jobQueue.Dequeue();
            }
        }
    }
}
