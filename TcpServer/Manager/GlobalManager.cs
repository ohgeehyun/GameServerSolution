using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Manager.Auth;

namespace TcpServer.Manager
{
    public class GlobalManager
    {
        private static GlobalManager? _instance;
        private static readonly object _lock = new();

        public readonly JwtValidator jwtValidator;

        private GlobalManager()
        {
            // 실제 매니저들 초기화
            jwtValidator = JwtValidator.Instance;
            
        }

        public static GlobalManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new GlobalManager();
                        }
                    }
                }
                return _instance;
            }
        }


    }
}
