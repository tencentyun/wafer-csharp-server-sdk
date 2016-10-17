using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    public class TunnelInvalidInfo
    {
        public string TunnelId { get; internal set; }

        public TunnelInvalidType Type { get; set; }
    }
}
