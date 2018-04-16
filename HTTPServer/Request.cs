using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        string[] requestLine;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
            headerLines = new Dictionary<string, string>();
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            int blankPos = -1;
            //TODO: parse the receivedRequest using the \r\n delimeter   
            requestLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length > 2)
            {
                // Parse Request line
                requestLine = requestLines[0].Split();
                relativeURI = requestLine[1].TrimStart('/');
                if (!ParseRequestLine(requestLine))
                    return false;

                // Validate blank line exists
                if (!ValidateBlankLine())
                    return false;

                // Load header lines into HeaderLines dictionary
                for (int i = 0; i < requestLines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(requestLines[i]))
                    {
                        blankPos = i;
                        break;
                    }
                }

                if (!LoadHeaderLines(blankPos))
                    return false;
            }

            else
                return false;



            return true;
        }

        private bool ParseRequestLine(string[] requestLine)
        {
            //request method parsing
            if (requestLine[0] == "GET")
                method = RequestMethod.GET;
            else if (requestLine[0] == "POST")
                method = RequestMethod.POST;
            else if (requestLine[0] == "HEAD")
                method = RequestMethod.HEAD;
            else
                return false;

            //parse URI
            if (!ValidateIsURI(requestLine[1]))
                return false;

            //parse HTTP version
            if (requestLine[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else if (requestLine[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (requestLine[2] == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else
                return false;

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines(int blankPos)
        {

            string[] headers = requestLines[1].Split(':');
            if (headers[0] != "Host")
                return false;

            for (int i = 1; i < blankPos; i++)
            {
                headers = requestLines[i].Split(':');
                /*if (headers.Length != 2)
                    return false;*/

                headerLines.Add(headers[0], headers[1]);
            }

            return true;
        }

        private bool ValidateBlankLine()
        {
            int blankPos = -1;
            for (int i = 0; i < requestLines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(requestLines[i]))
                {
                    blankPos = i;
                    break;
                }
            }

            if (blankPos == -1)
                return false;

            return true;
        }

    }
}
