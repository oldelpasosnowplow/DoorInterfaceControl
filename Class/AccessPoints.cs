using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorInterfaceControl.Class
{
    public class AccessPoints
    {
        public int ID { get; set; }
        public bool disabled { get; set; }
        public string macAddress { get; set; }
        public string name { get; set; }
        public string model { get; set; }
        public int areaID { get; set; }
        public int GroupID { get; set; }
        public string createdStartDate { get; set; }
        public string createdEndDate { get; set; }
        public string updatedStartDate { get; set; }
        public string updatedEndDate { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public string state { get; set; }
        public string lockStatus { get; set; }
    }
}
