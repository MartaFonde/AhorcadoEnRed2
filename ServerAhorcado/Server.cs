using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerAhorcado
{
    class Server
    {
        static readonly internal object l = new object();

        private static Socket s;
        private Socket sClient;
        private static List<Client> clients = new List<Client>();
        static Random random = new Random();
        static List<string> words = new List<string>();

        //comandos
        const string GET_WORD = "getword";
        const string SEND_WORD = "sendword";     //sendword word
        const string GET_RECORDS = "getrecords";
        const string SEND_RECORD = "sendrecord";        //sendrecord time ---- sendrecord time name ip
        const string CLOSE_SERVER = "closeserver";      //closeserver clave --- A clave será "clave"

        private static string psw = "clave";

        static string pathWords = "..\\..\\words.txt";
        static string pathRecords = "..\\..\\records.txt";
        static string pahtRecordsTemp = "..\\..\\recordsTemp.txt";
        static string pahtRecordsBackUp = "..\\..\\recordsTempBackUp.txt";
        

        public Server()
        {
            int port = 31416;
            bool portFree = false;

            //using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                while (!portFree)
                {
                    try
                    {
                        portFree = true;
                        IPEndPoint ie = new IPEndPoint(IPAddress.Any, port);
                        s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        s.Bind(ie);
                        s.Listen(5);

                        Console.WriteLine("Server waiting at port {0}", ie.Port);

                        while (true)  
                        {
                            sClient = s.Accept();
                            clients.Add(new Client(sClient));
                        }
                    }
                    catch (SocketException e) when (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
                    {
                        Console.WriteLine($"Port {port} in use");
                        portFree = false;
                        port++;
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
            }
        }

        internal static string serverAction(Socket sClient, string msg)
        {
            try
            {
                using (NetworkStream ns = new NetworkStream(sClient))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    if (msg != null && msg.Trim().Split(' ').Length > 0)
                    {
                        switch (msg.Split(' ')[0])
                        {
                            case SEND_WORD:
                                if (msg.Split(' ').Length == 2 && msg.Split(' ')[1].Length > 0)
                                {
                                    lock (l)
                                    {
                                        WriteFileWord(msg.Split(' ')[1]);
                                    }
                                }
                                break;
                            case GET_WORD:
                                if (msg.Split(' ').Length == 1)
                                {
                                    lock (l)
                                    {
                                        string word = GetWord();
                                        Console.WriteLine(word);
                                        return word;
                                    }
                                }
                                break;
                            case SEND_RECORD:
                                if (msg.Split(' ').Length == 2 || msg.Split(' ').Length == 4)
                                {
                                    int n;
                                    if (msg.Split(' ')[1].Length == 5 && msg.Split(' ')[1][2] == ':'
                                        && Int32.TryParse(msg.Split(' ')[1].Split(':')[0], out n)
                                        && Int32.TryParse(msg.Split(' ')[1].Split(':')[1], out n))
                                    {
                                        if (msg.Split(' ').Length == 2)    //record tiempo
                                        {
                                            lock (l)
                                            {
                                                if (ReadRecordsToIncorpore(msg.Split(' ')[1]))
                                                {
                                                    return "True";
                                                }
                                            }
                                        }
                                        else //record tiempo nombre ip
                                        {
                                            IPAddress ip;
                                            if (msg.Split(' ')[2].Length == 3 && IPAddress.TryParse(msg.Split(' ')[3], out ip))
                                            {
                                                lock (l)
                                                {
                                                    WriteRecord(msg);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case GET_RECORDS:
                                if (msg.Split(' ').Length == 1)
                                {
                                    lock (l)
                                    {
                                        return ReadRecordsToSend();
                                    }
                                }
                                break;
                            case CLOSE_SERVER:
                                if (msg.Split(' ').Length == 2)
                                {
                                    if (msg.Split(' ')[1] == psw)
                                    {
                                        lock (l)
                                        {
                                            foreach (Client c in clients)
                                            {
                                                c.s.Close();
                                            }
                                            s.Close();
                                        }
                                        return "True";
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                sClient.Close();
            }
            
            return null;            
        }

        private static bool ReadFileWords()
        {
            if (File.Exists(pathWords))
            {
                words.Clear();
                string line;
                try
                {
                    using (StreamReader sr = new StreamReader(pathWords))
                    {
                        line = sr.ReadLine();
                        while (line != null)
                        {
                            if(line.Length > 0)
                            {
                                words.Add(line);
                            }                            
                            line = sr.ReadLine();
                        }
                    }
                }
                catch (IOException)
                {
                    return false;
                }

                if(words.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            else
            {
                File.Create(pathWords);
                return false;
            }
        }

        private static string GetWord()
        {
            if (ReadFileWords())
            {
                return words[random.Next(0, words.Count - 1)];
            }
            else
            {
                return "";
            }
        }

        private static bool WriteFileWord(string arg)
        {
            if (File.Exists(arg))
            {
                string line;
                try
                {
                    using (StreamReader sr = new StreamReader(arg))
                    using (StreamWriter sw = new StreamWriter(pathWords, true))
                    {
                        line = sr.ReadLine();
                        while (line != null)
                        {
                            string[] newWords = line.Split(',');
                            foreach (string word in newWords)
                            {
                                if(word.Trim().Length > 0)
                                {
                                    sw.WriteLine(word.Trim());
                                }                                
                                line = sr.ReadLine();
                            }
                        }
                    }
                }
                catch (IOException)
                {
                    return false;
                }
                
                return true;
            }
            else
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(pathWords, true))
                    {
                        sw.WriteLine(arg.Trim());
                    }                    
                }
                catch (IOException)
                {
                    return false;
                }
                return true;
            }
        }

        private static bool ReadRecordsToIncorpore(string arg)
        {
            string[] time = arg.Split(':');
            string line;
            int numLines = 0;
            if (File.Exists(pathRecords))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(pathRecords))
                    {
                        line = sr.ReadLine();
                        while (line != null)
                        {
                            numLines++;
                            line = sr.ReadLine();
                        }
                    }
                    if (numLines < 10)       //writeline escribe retorno de carro != null vai haber unha liña máis
                    {
                        return true;
                    }
                    else if (numLines == 10)     //comprobamos a ver se podería entrar
                    {
                        using (StreamReader sr = new StreamReader(pathRecords))
                        {
                            line = sr.ReadLine();
                            while (line != null)
                            {
                                string[] timeRecord = line.Substring(0, 5).Split(':');

                                if (Convert.ToInt32(timeRecord[0]) > Convert.ToInt32(time[0]))
                                {
                                    return true;
                                }
                                else if (Convert.ToInt32(timeRecord[0]) == Convert.ToInt32(time[0]) &&
                                   Convert.ToInt32(timeRecord[1]) >= Convert.ToInt32(time[1]))
                                {
                                    return true;
                                }
                                line = sr.ReadLine();
                            }
                        }
                    }
                    return false; //non mellora ningún record
                }
                catch (IOException)
                {
                    return false;
                }                       
            }
            else
            {
                File.Create(pathRecords);
                return true;
            }
        }

        private static bool WriteRecord(string record)
        {
            string[] timeRecord = record.Split(' ')[1].Split(':');
            string line;
            int numLines = 0;
            bool anhadido = false;

            try
            {
                using (StreamReader sr = new StreamReader(pathRecords))
                using (StreamWriter sw = new StreamWriter(pahtRecordsTemp))
                {
                    line = sr.ReadLine();

                    if (line == null)        //se o fich está baleiro escribimos directamente
                    {
                        sw.WriteLine(record.Substring(SEND_RECORD.Length + 1));
                    }

                    else
                    {
                        while (line != null && numLines <= 9)       // 9 liñas fich + novo record
                        {
                            if (!anhadido)  //se non está engadido record ---- comprobacións
                            {
                                string[] timeFileRecord = line.Substring(0, 5).Split(':');

                                if (Convert.ToInt32(timeFileRecord[0]) < Convert.ToInt32(timeRecord[0]) &&
                                    Convert.ToInt32(timeFileRecord[1]) < Convert.ToInt32(timeRecord[1]))
                                {
                                    sw.WriteLine(line);
                                }
                                else if (Convert.ToInt32(timeFileRecord[0]) == Convert.ToInt32(timeRecord[0]))
                                {
                                    if (Convert.ToInt32(timeFileRecord[1]) < Convert.ToInt32(timeRecord[1]))
                                    {
                                        sw.WriteLine(line);
                                    }
                                    else
                                    {
                                        sw.WriteLine(record.Substring(SEND_RECORD.Length + 1));
                                        numLines++;
                                        anhadido = true;
                                        if (numLines < 10)
                                        {
                                            sw.WriteLine(line);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sw.WriteLine(line);
                            }

                            line = sr.ReadLine();
                            numLines++;

                            if (!anhadido && line == null && numLines <= 10)  //se non se escribiu o record ata a última linea --- ultimo posto
                            {
                                sw.WriteLine(record.Substring(SEND_RECORD.Length + 1));
                            }
                        }
                    }
                }
                File.Replace(pahtRecordsTemp, pathRecords, pahtRecordsBackUp);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private static string ReadRecordsToSend()
        {
            string records = "";
            try
            {
                using (StreamReader sr = new StreamReader(pathRecords))
                {
                    records = sr.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            return records;
        }
    }
}

