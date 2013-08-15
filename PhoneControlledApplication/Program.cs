using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneControlledApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            PhoneServer phoneServer_ = PhoneServer.Instance;
            phoneServer_.getConnection();

            
        }
    }
}
