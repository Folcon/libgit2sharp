using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace Follow_test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Repository repo = new Repository(@"c:\projects\git\follow-test"))
            {
                //under-test
                var test = repo.History("under-test.txt").Reverse().ToArray();

                Console.WriteLine("under-test.txt");
                Console.WriteLine("-------------------");
                foreach (var commit in test)
                {
                    Console.WriteLine("{0} {1}", commit.Id.ToString(7), commit.MessageShort);
                }
                Console.WriteLine();

                //untouched
                test = repo.History("untouched.txt").Reverse().ToArray();

                Console.WriteLine("untouched.txt");
                Console.WriteLine("-------------------"); 
                foreach (var commit in test)
                {
                    Console.WriteLine("{0} {1}", commit.Id.ToString(7), commit.MessageShort);
                }
                Console.WriteLine();

                //so-renamed.txt
                test = repo.History("so-renamed.txt").Reverse().ToArray();

                Console.WriteLine("so-renamed.txt");
                Console.WriteLine("-------------------"); 
                foreach (var commit in test)
                {
                    Console.WriteLine("{0} {1}", commit.Id.ToString(7), commit.MessageShort);
                }

            }

            Console.ReadKey();
        }
    }
}
