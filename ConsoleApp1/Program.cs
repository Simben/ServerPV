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
    class HttpServer
    {
        static int PVj1 = 20;
        static int PVj2 = 20;

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
                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }
                else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/"))
                {
                    byte[] data = (System.IO.File.ReadAllBytes("toto.html"));
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/test"))
                {
                    byte[] data = (System.IO.File.ReadAllBytes("test.html"));
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }

                else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/Player/LifePoint/1"))
                {
                    byte[] data = Encoding.UTF8.GetBytes(PVj1.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    resp.Headers.Add("Access-Control-Allow-Origin: *");
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/Player/LifePoint/2"))
                {
                    byte[] data = Encoding.UTF8.GetBytes(PVj2.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "PUT") && (req.Url.AbsolutePath == "/Player/LifePoint/1/increment"))
                {
                    PVj1++;
                    byte[] data = Encoding.UTF8.GetBytes(PVj1.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "PUT") && (req.Url.AbsolutePath == "/Player/LifePoint/2/increment"))
                {
                    PVj2++;
                    byte[] data = Encoding.UTF8.GetBytes(PVj2.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "PUT") && (req.Url.AbsolutePath == "/Player/LifePoint/1/decrement"))
                {
                    PVj1--;
                    byte[] data = Encoding.UTF8.GetBytes(PVj1.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "PUT") && (req.Url.AbsolutePath == "/Player/LifePoint/2/decrement"))
                {
                    PVj2--;
                    byte[] data = Encoding.UTF8.GetBytes(PVj2.ToString());
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                else if (req.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                // Write the response info
                else
                { 
                    string disableSubmit = !runServer ? "disabled" : "";
                    byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                // Write out to the response stream (asynchronously), then close it
                
                resp.Close();
            }
        }


        static bool debug = false;
        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);
            Console.WriteLine($"For Score Keep Go To : {url}");
            Console.WriteLine($"For results Go To : {url}/test");

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}