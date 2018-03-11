using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class NoticeCode
{
    public const short NotExistPlayer = -1;

    private static Dictionary<short, string> NoticeMap = new Dictionary<short, string>() {
        {NotExistPlayer, "角色不存在"},
    };

    public static string GetStr(short code)
    {
        if (!NoticeMap.ContainsKey(code)) {
            return null;
        }
        return NoticeMap[code];
    }
}
