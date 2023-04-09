using Db.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
    class Program
    {
        static void Main(string[] args)
        {
            Source source = Source.Find(3);
            Console.WriteLine(Source.Find(3));
        }
    }
}
