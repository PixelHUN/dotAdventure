using System;
using System.Collections.Generic;
using System.Text;

namespace adventure
{
    public class Variable
    {
        public string name;
        public string val;

        public Variable(string varname, string varval)
        {
            name = varname;
            val = varval;
        }
    }
}
