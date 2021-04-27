using System;

namespace Model
{
    public class Data
    {
        public Int32 ErrCode { get; set; }
        public String ErrMessage { get; set; }
        public Object Result { get; set; }

        public Data()
        { 
            ErrCode = 0;
            ErrMessage = "";
            Result = new Object();
        }
    }
}
