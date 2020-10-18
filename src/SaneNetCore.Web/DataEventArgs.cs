using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace scanner
{
    public class DataEventArgs : EventArgs
    {
        public DataEventArgs()
        {

        }
        public DataEventArgs(string data)
        {
            Data = data;
        }

        public DataEventArgs(DataReceivedEventArgs e) {
            Data = e.Data;
        }
        public string Data { get; set; }
    }
}
