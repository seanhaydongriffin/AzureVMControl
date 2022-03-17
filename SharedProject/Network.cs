using System;
using System.IO;
using System.Net.NetworkInformation;

namespace SharedProject
{
    public static class Network
    {
        public static Ping myPing = new Ping();

        public static void PingUntilUnavailable(String host)
        {
            for (int retry_num = 1; retry_num <= 60; retry_num++)
            {
                try
                {
                    PingReply reply = myPing.Send(host, 1000);

                    if (reply != null)

                        Console.WriteLine("Ping " + host + " attempt " + retry_num + " of 60 status is " + reply.Status);

                    if (reply == null || reply.Status != IPStatus.Success)

                        break;

                }
                catch (Exception)
                {
                    Console.WriteLine("Ping " + host + " attempt " + retry_num + " of 60 status unreachable");
                    break;
                }

                System.Threading.Thread.Sleep(5000);
            }
        }

        public static void PingUntilAvailable(String host)
        {
            for (int retry_num = 1; retry_num <= 60; retry_num++)
            {
                try
                {
                    PingReply reply = myPing.Send(host, 1000);

                    if (reply != null)

                        Console.WriteLine("Ping " + host + " attempt " + retry_num + " of 60 status is " + reply.Status);

                    if (reply != null && reply.Status == IPStatus.Success)

                        break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Ping " + host + " attempt " + retry_num + " of 60 status unreachable");
                }

                System.Threading.Thread.Sleep(5000);
            }
        }



    }
}
