using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RoomServer.Packet
{
    #region ##notice
    // 현재는 패킷의 이름 자체가 길지않기 떄문에 string으로 작업을하여도 큰 부하는 없지만
    // 초당 수만 건 이상 발생한다면 string이 아닌 stringbuilder나 stackalloc + Span<char>로 사용
    #endregion

    public static class Utils
    {
        public static string  TransPacketNameForPacketEnum(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            string[] parts = name.Split('_');
            if (parts.Length == 0)
                return string.Empty;

            string result = parts[0]; // 첫 단어는 그대로 사용

            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].Length == 0)
                    continue;

                string lower = parts[i].ToLower(); // 전체 소문자
                result += char.ToUpper(lower[0]) + lower.Substring(1);
            }

            return result;
        }

        public static string TransPacketNameForPacketEnum(ReadOnlySpan<char> name)
        {
            if (name.IsEmpty)
                return string.Empty;

            Span<char> buffer = stackalloc char[name.Length]; // 미리 고정 사이즈 선언
            int bufIndex = 0;
            bool capitalizeNext = false;


            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];

                if (c == '_')
                {
                    capitalizeNext = true;
                    continue; // _ 제거
                }

                if (bufIndex == 0)
                {
                    // 첫 글자는 대문자로 그대로 둠
                    buffer[bufIndex++] = c;
                    capitalizeNext = false;
                    continue;
                }

                if (capitalizeNext)
                {
                    // _ 다음 문자는 대문자로 변환 (이미 대문자지만 혹시 몰라서)
                    buffer[bufIndex++] = char.ToUpper(c);
                    capitalizeNext = false;
                }
                else
                {
                    // 그 외는 소문자로 변환
                    buffer[bufIndex++] = char.ToLower(c);
                }
            }


            return new string(buffer.Slice(0, bufIndex));
        }



    }
}
