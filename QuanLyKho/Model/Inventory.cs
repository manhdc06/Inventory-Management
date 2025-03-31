using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Model
{
    
        public class Inventory
        {
        public int STT { get; set; }
        public Object Object { get; set; }
        public int Count { get; set; } // Tồn kho
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        }

}
