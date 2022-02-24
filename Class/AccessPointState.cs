using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorInterfaceControl.Class
{
    public class AccessPointState
    {
        public int id { get; set; }
        public int accesspointID { get; set; }
        public string openStatus { get; set; }
        public string lockStatus { get; set; }
        public string state { get; set; }
    }
}
