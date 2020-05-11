using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RegClient
{
    class Client
    {
        static void StartClient()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Loopback;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}", socket.RemoteEndPoint.ToString());

                    NetworkStream stream = new NetworkStream(socket);
                    StreamReader sr = new StreamReader(stream);
                    StreamWriter sw = new StreamWriter(stream) { NewLine = "\r\n", AutoFlush = true };
                    Console.WriteLine("Insert command:>>");
                    String line = "";
                    while (!line.Contains("ShutDown")) {
                        line = Console.ReadLine();
                        sw.WriteLine(line);
                        
                        while (!(line = sr.ReadLine()).Equals("END"))
                        {
                            Console.WriteLine(line); 
                        }
                        if (line.Contains("ShutDown"))
                        {
                            break;
                        }
                    }
                    Console.WriteLine("ShutDown");

                    sr.Close();
                    sw.Close();
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Main(String[] args)
        {
            StartClient();
            Console.Write("Press any key...");
            Console.ReadKey();
        }
    }
}
