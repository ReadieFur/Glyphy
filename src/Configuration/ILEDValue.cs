using Glyphy.LED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glyphy.Configuration
{
    interface ILEDValue
    {
        public uint brightness { get; set; }
        public EAddressable led { get; set; }
    }
}
