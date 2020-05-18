using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegServer
{
    class Server
    {
        public static List<string> users = new List<string>();

        static void StartListening()
        {
            IPAddress ipAddress = IPAddress.Loopback;
            IPEndPoint localEndpoint = new IPEndPoint(ipAddress, 11000);

            Socket serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {



                serverSocket.Bind(localEndpoint);
                serverSocket.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection ...");
                    Socket socket = serverSocket.Accept();

                    Task<int> t = Task.Factory.StartNew(() => Run(socket));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        private static int Run(Socket socket)
        {
            NetworkStream stream = new NetworkStream(socket);
            StreamReader sr = new StreamReader(stream);
            StreamWriter sw = new StreamWriter(stream) { NewLine = "\r\n", AutoFlush = true };

            Console.WriteLine("Receiving data ...");
            string request;
            String result = "";
            while (!(request = sr.ReadLine()).Equals("ShutDown"))
            {
                Console.WriteLine("Recieved from client: {0}", request);

                result = Message(request);

                result += "END";
                Console.WriteLine("Recieved from client:\n" + result);

                sw.WriteLine(result);
            }
            
            Console.WriteLine("Stiglo od klijenta: {0}", request);
            Console.WriteLine("Server stopped.");

            result = request + "END";
            sw.WriteLine(result);

            sw.Close();
            sr.Close();
            stream.Close();

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();


            return 0;
        }

        private static String Message(String request)
        {
            String result = "";
            if (request.StartsWith("ADD"))
            {
                String usernames = request.Split(new[] { "ADD " }, StringSplitOptions.None)[1].Replace(" ","");
                if (usernames.Contains(","))
                {
                    string[] list = usernames.Split(new[] { "," }, StringSplitOptions.None);
                    foreach(var user in list)
                    {
                        result += AddUser(user);
                    }
                }
                else
                {
                    result += AddUser(usernames);
                }
                result += "\n";
            }
            else if (request.StartsWith("LIST"))
            {
                result = ListAllUsers();
            }
            else if (request.StartsWith("REMOVE"))
            {
                String username = request.Split(new[] { "REMOVE" }, StringSplitOptions.None)[1].Replace(" ", "");
                result = RemoveUser(username);
            }
            else if(request.StartsWith("FIND"))
            {
                String username = request.Split(new[] { "FIND" }, StringSplitOptions.None)[1].Replace(" ", "");
                result = FindUser(username);
            }
            else
            {
                result += $"Command {result} does not exist\n";
            }
            return result;
        }

        private static String AddUser(String username)
        {
            if (NotInList(username))
            {
                users.Add(username);
                return $"User {username} added.\n";
            }
            else
            {
                return $"User {username} already exists.\n";
            }
        }

        static Boolean NotInList(String username)
        {
            foreach (string user in users)
            {
                if (user.Equals(username))
                {
                    return false;
                }
            }
            return true;
        }

        static string ListAllUsers()
        {
            string result = "Users: \n";
            if(users.Count == 0)
            {
                return "List is empty. \n";
            }
            foreach (var user in users)
            {
                result += $"{user} \n";
            }
            return result + "\n";
        }

        static string RemoveUser(string username)
        {
            string result = $"Remove user: {username}. \n";

            result += users.Remove(username) ? "User removed.\n" : "User does not exist.\n";

            return result + "\n";
        }

        static string FindUser(string username)
        {
            string result = $"Find user: {username}. \n";
            result += users.Contains(username) ? "User exists in list. \n" : "User does not exist in list.\n";
            return result + "\n";
        }

        static void Main(string[] args)
        {
            StartListening();
        }
    }
}
