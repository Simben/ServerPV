// Filename:  HttpServer.cs        
// Author:    Benjamin N. Summerton <define-private-public>        
// License:   Unlicense (http://unlicense.org/)

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace HttpListenerExample
{
    public class Player
    {
        public string Name { get; set; }
        public int LifePoint { get; set; }
        public int PoisonCounter { get; set; }
    }

    class HttpServer
    {
        public static List<string> GetLocalIPAddress()
        {
            List<string> Result = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Result.Add(ip.ToString());
                }
            }
            return Result;
            //throw new Exception("No network adapters with an IPv4 address in the system!");
        }


        static Player PVj1 = new Player() { LifePoint = 20, Name = "Player_1" };
        static Player PVj2 = new Player() { LifePoint = 20, Name = "Player_2" };

        public static HttpListener listener;
        public static string url = "http://localhost:8080/";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";


        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;
            DirectoryInfo d = new DirectoryInfo(@"www"); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.*"); //Getting Text files
            List<string> WWW = Files.Select(p => p.Name).ToList();
            List<string> Pages = WWW.Select(p => "/" + p.ToLower()).ToList();
            string seekedURL = "";


            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                if (debug)
                {
                    // Print out some info about the request
                    Console.WriteLine("Request #: {0}", ++requestCount);
                    Console.WriteLine(req.Url.ToString());
                    Console.WriteLine(req.HttpMethod);
                    Console.WriteLine(req.UserHostName);
                    Console.WriteLine(req.UserAgent);
                    Console.WriteLine();
                }
                seekedURL = req.Url.AbsolutePath;
            StartSearch:
                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (seekedURL == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }
                else if ((req.HttpMethod == "GET") && (seekedURL == "/"))
                {
                    seekedURL = "/index.html";
                    goto StartSearch;
                }
                else if ((req.HttpMethod == "GET") && (Pages.Contains(seekedURL.ToLower())))
                {
                    foreach (var page in WWW)
                    {
                        if ("/" + page.ToLower() == seekedURL.ToLower())
                        {
                            byte[] data = (System.IO.File.ReadAllBytes(@"www/" + page));
                            resp.ContentType = "text/html";
                            resp.ContentEncoding = Encoding.UTF8;
                            resp.ContentLength64 = data.LongLength;
                            await resp.OutputStream.WriteAsync(data, 0, data.Length);
                            break;
                        }
                    }
                }
                /*else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/results"))
                {
                    byte[] data = (System.IO.File.ReadAllBytes("test.html"));
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }*/

                else if ((req.HttpMethod == "GET") && (seekedURL.ToLower() == "/player/lifepoint/1"))
                {
                    byte[] data = Encoding.UTF8.GetBytes(PVj1.LifePoint.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    resp.Headers.Add("Access-Control-Allow-Origin: *");
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "GET") && (seekedURL.ToLower() == "/player/lifepoint/2"))
                {
                    byte[] data = Encoding.UTF8.GetBytes(PVj2.LifePoint.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    resp.Headers.Add("Access-Control-Allow-Origin: *");
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "GET") && (seekedURL.ToLower() == "/player/name/1"))
                {
                    byte[] data = Encoding.UTF8.GetBytes(PVj1.Name.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    resp.Headers.Add("Access-Control-Allow-Origin: *");
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "GET") && (seekedURL.ToLower() == "/player/name/2"))
                {
                    byte[] data = Encoding.UTF8.GetBytes(PVj2.Name.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    resp.Headers.Add("Access-Control-Allow-Origin: *");
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "PUT") && (seekedURL.ToLower() == "/player/lifepoint/1/increment"))
                {
                    PVj1.LifePoint++;
                    byte[] data = Encoding.UTF8.GetBytes(PVj1.LifePoint.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "PUT") && (seekedURL.ToLower() == "/player/lifepoint/2/increment"))
                {
                    PVj2.LifePoint++;
                    byte[] data = Encoding.UTF8.GetBytes(PVj2.LifePoint.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "PUT") && (seekedURL.ToLower() == "/player/lifepoint/1/decrement"))
                {
                    PVj1.LifePoint--;
                    byte[] data = Encoding.UTF8.GetBytes(PVj1.LifePoint.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "PUT") && (seekedURL.ToLower() == "/player/lifepoint/2/decrement"))
                {
                    PVj2.LifePoint--;
                    byte[] data = Encoding.UTF8.GetBytes(PVj2.LifePoint.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "POST") && (seekedURL.ToLower() == "/player/lifepoint/reset"))
                {
                    PVj1.LifePoint = 20;
                    PVj2.LifePoint = 20;
                    byte[] data = Encoding.UTF8.GetBytes(PVj1.LifePoint.ToString() + PVj2.LifePoint.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }

                else if ((req.HttpMethod == "PATCH") && (seekedURL.ToLower() == "/player/1"))
                {
                    foreach (string key in req.Headers.AllKeys)
                    {
                        if (key.ToLower() == "name")
                            PVj1.Name = req.Headers[key] ?? "";
                        else if (key.ToLower() == "lifepoint")
                        {
                            int pv = 0;
                            if (int.TryParse(req.Headers[key] ?? "", out pv))
                                PVj1.LifePoint = pv;
                        }
                    }

                    byte[] data = Encoding.UTF8.GetBytes(PVj1.LifePoint.ToString() + PVj2.LifePoint.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }

                else if ((req.HttpMethod == "PATCH") && (seekedURL.ToLower() == "/player/2"))
                {
                    foreach (string key in req.Headers.AllKeys)
                    {
                        if (key.ToLower() == "name")
                            PVj2.Name = req.Headers[key] ?? "";
                        else if (key.ToLower() == "lifepoint")
                        {
                            int pv = 0;
                            if (int.TryParse(req.Headers[key] ?? "", out pv))
                                PVj2.LifePoint = pv;
                        }
                    }

                    byte[] data = Encoding.UTF8.GetBytes(PVj1.LifePoint.ToString() + PVj2.LifePoint.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                else //if (seekedURL != "/favicon.ico")
                {

                    resp.StatusCode = 404;
                    byte[] data = Encoding.UTF8.GetBytes("404 Not Found");
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }


                // Write the response info
                /*else
                {
                    string disableSubmit = !runServer ? "disabled" : "";
                    byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }*/
                // Write out to the response stream (asynchronously), then close it

                resp.Close();
            }
        }


        static bool debug = true;
        public static void Main(string[] args)
        {

            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            Console.WriteLine("version 0.0.3");
            Console.WriteLine("Listening for connections on {0}", url);

            var IPs = GetLocalIPAddress();
            for (int i = 0; i < IPs.Count; i++)
            {
                IPs[i] = $"http://{IPs[i]}:8080/";
                listener.Prefixes.Add(IPs[i]);
                Console.WriteLine("Listening for connections on {0}", IPs[i]);
            }

            listener.Start();
            Console.WriteLine("Server Started");
            Console.WriteLine("For Score Keep Go To : {Base_Address}/scorekeep");
            Console.WriteLine("For results Go To : {Base_Address}/results");

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}