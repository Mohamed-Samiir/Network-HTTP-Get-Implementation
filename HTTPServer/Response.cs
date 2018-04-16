using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString = string.Empty;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines;
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            headerLines = new List<string>();
            this.code = code;
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(DateTime.Now.ToString());
            if (redirectoinPath != null)
                headerLines.Add(redirectoinPath);
            // TODO: Create the response string
            responseString += GetStatusLine(this.code) + "\r\n";

            for (int i = 0; i < headerLines.Count; i++)
                responseString += headerLines[i] + "\r\n";

            responseString += content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;

            if (code == StatusCode.OK)
                statusLine = "HTTP/1.1 " + StatusCode.OK.ToString() + " OK";
            else if(code == StatusCode.NotFound)
                statusLine = "HTTP/1.1 " + StatusCode.NotFound.ToString() + " Not Found";
            else if (code == StatusCode.BadRequest)
                statusLine = "HTTP/1.1 " + StatusCode.BadRequest.ToString() + " Bad Request";
            else if (code == StatusCode.InternalServerError)
                statusLine = "HTTP/1.1 " + StatusCode.InternalServerError.ToString() + " Internal Server Error";
            else if (code == StatusCode.Redirect)
                statusLine = "HTTP/1.1 " + StatusCode.Redirect.ToString() + " Redirect";

            return statusLine;
        }
    }
}
