using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace db.service
{
    public enum ConnectionType
    {
        [Description("Develop")]
        DBCon,
        [Description("List")]
        DBConLive,
    }
}
