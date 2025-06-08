using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomServer.UserData
{
    public class User
    {
        public string jwtId { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public string userNick { get; set; } = string.Empty;
    }
}
