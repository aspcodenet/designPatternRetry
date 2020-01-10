using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var ws = new ServiceReference1.GetCurrentlyBestPlayerSoapClient())
            {
                for(int i = 0; i < 10; i++)
                {
                    Console.WriteLine( ws.Hockey() );
                    Console.WriteLine(Soccer());
                }

            }
        }

        static string BestSoccerPlayer()
        {
            int retries = 5;
            int sleepWhenFailBeforeRetry = 3000;
            while(retries>0)
            {
                try
                {
                    using (var ws = new ServiceReference1.GetCurrentlyBestPlayerSoapClient())
                    {
                        return ws.Soccer();
                    }
                }
                catch
                {
                    System.Threading.Thread.Sleep(sleepWhenFailBeforeRetry);
                    retries--;
                }
            }
            return "ERROR";
        }

        static string Soccer()
        {
            return Policy.Handle<System.ServiceModel.FaultException>().
                Retry(38,(exeption,retries)=> {
                    System.Threading.Thread.Sleep(retries*10002);
                }).
                Execute(() =>
                {
                    using (var ws = new ServiceReference1.GetCurrentlyBestPlayerSoapClient())
                    {
                        return ws.Soccer();
                    }
                });
            
        }
    }
}
