using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        IPEndPoint iep;
        public Server(int portNumber, string redirectionMatrixPath)
        {
            Configuration.RedirectionRules = new Dictionary<string, string>();
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.iep = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(iep);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSock = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSock.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            byte[] data;
            byte[] response;
            int receivedLength;
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    data = new byte[1024];
                    receivedLength = clientSock.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                        break;

                    // TODO: Create a Request object using received request string
                    string requestStr = Encoding.ASCII.GetString(data, 0, receivedLength);
                    Request newRequest = new Request(requestStr);
                    // TODO: Call HandleRequest Method that returns the response
                    Response newResponse = HandleRequest(newRequest);
                    // TODO: Send Response back to client
                    response = new byte[1024];
                    response = Encoding.ASCII.GetBytes(newResponse.ResponseString);
                    clientSock.Send(response);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSock.Close();
        }

        Response HandleRequest(Request request)
        {
            string content;
            string path;
            try
            {
                //TODO: check for bad request 
                if(!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", content, null);
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                path = Path.Combine(Configuration.RootPath, request.relativeURI);

                //TODO: check for redirect
                if (Configuration.RedirectionRules.ContainsKey(request.relativeURI))
                {
                    string redirPath = GetRedirectionPagePathIFExist(request.relativeURI);
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    return new Response(StatusCode.Redirect, "text/html", content, redirPath);
                }

                //TODO: check file exists
                if(!File.Exists(path))
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "text/html", content, null);
                }

                //TODO: read the physical file
                // Create OK response
                content = LoadDefaultPage(request.relativeURI);
                return new Response(StatusCode.OK, "text/html", content, null);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", content, null);
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                string redirPage = Configuration.RedirectionRules[relativePath].Remove(0, 1);
                string redirPath = Path.Combine(Configuration.RootPath, redirPage);
                return redirPath;
            }
                return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException();
                }
                else
                {
                    // else read file and return its content
                    string content = File.ReadAllText(filePath);
                    return content;
                }

            }
            catch(FileNotFoundException ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] lines = File.ReadAllLines(filePath);
                // then fill Configuration.RedirectionRules dictionary 
                for(int i=0; i<lines.Length; i++)
                {
                    string[] parts = lines[i].Split(',');
                    Configuration.RedirectionRules.Add(parts[0], parts[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
