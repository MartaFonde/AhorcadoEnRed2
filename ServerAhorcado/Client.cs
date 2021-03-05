using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerAhorcado
{
    class Client
    {
        static readonly internal object l = new object();
        internal Socket s;
        IPEndPoint ie;
        internal int port;
        string word;
        internal bool connected = true;

        const string GET_WORD = "getword";
        const string SEND_WORD = "sendword";     //sendword word
        const string GET_RECORDS = "getrecords";
        const string SEND_RECORD = "sendrecord";        //sendrecord time ---- sendrecord time name ip
        const string CLOSE_SERVER = "closeserver";      //closeserver clave --- A clave será "clave"


        public Client(Socket socket)
        {
            s = socket;
            ie = (IPEndPoint)socket.RemoteEndPoint;
            port = ie.Port;            

            Thread t = new Thread(Game);
            t.Start();
        }        
        private void Game()
        {
            string msg  = null;
            try
            {
                using (NetworkStream ns = new NetworkStream(s))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    word = Server.serverAction(s, GET_WORD);
                    sw.WriteLine(word);
                    sw.Flush();

                    while (connected)
                    {
                        msg = sr.ReadLine();
                        if (msg != null)
                        {                                
                            switch (msg.Trim().Split(' ')[0])
                            {
                                case GET_WORD:
                                    sw.WriteLine(Server.serverAction(s, msg));
                                    sw.Flush();
                                    //Server.serverAction(s, msg);
                                    break;
                                case SEND_WORD:                                          
                                    Server.serverAction(s, msg);                                                                               
                                    break;

                                case SEND_RECORD:
                                    string record = Server.serverAction(s, msg);
                                    sw.WriteLine(record);
                                    sw.Flush();
                                    if (record == "True")
                                    {                                                                               
                                        Server.serverAction(s, msg);                                            
                                    }                                                                                                                                                                                                                                                                          
                                    break;

                                case GET_RECORDS:
                                    sw.WriteLine(Server.serverAction(s, msg));
                                    sw.Flush();
                                    break;

                                case CLOSE_SERVER:                                        
                                    sw.WriteLine(Server.serverAction(s, msg));
                                    sw.Flush(); 
                                    break;
                            }                            
                        } 
                    }
                }
            }
            catch (IOException)
            {
                connected = false;
                s.Close();
            }
            Console.ReadKey();
        }

        private string getIP()
        {
            string ipUser = "";
            string localhost = Dns.GetHostName();
            IPHostEntry hostInfo = Dns.GetHostEntry(localhost);
            foreach (IPAddress ip in hostInfo.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipUser = ip.ToString();
                }
            }
            return ipUser;
        }

    }
}
       
   
