using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Enum
{
    public enum MemberStateType
    {
        //이메일 미인증
        Pending = 0,
        //정상
        Normal = 1,
        //마킹
        Marking = 2,
        //정지
        Stop = 3,
        //탈퇴
        Leave = 4

        //미인증 = 0,
        //정상 = 1,
        //마킹 = 2,
        //정지 = 3, 
        //탈퇴 = 4
    }
}
