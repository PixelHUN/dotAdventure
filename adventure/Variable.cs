using System;
using System.Collections.Generic;
using System.Text;

namespace adventure
{
    public class Variable
    {
        public string name { get; set; }
        public string val { get; set; }

        public Variable()
        {
            
        }

        public void Set(string varname, string varval)
        {
            name = varname;
            val = varval;
        }
    }
}
