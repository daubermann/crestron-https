using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;

namespace Http
{
    public class Http
    {
        private HttpClient _client;

        //Constructor for this class
        public Http()
        {
            _client = new HttpClient();
            _client.Accept = "application/json";
            _client.KeepAlive = true;

        }


        public void SendRequest(string url, RequestType requestType ,string payload)
        {
            var request = new HttpClientRequest();
            request.Url.Parse(url);
            request.RequestType = requestType;
            request.Header.ContentType = "application/json";

            //adds the body of the message
            request.ContentString = payload;
            request.ContentSource = ContentSource.ContentString;

            //builds up the headers
            var headers = new HttpHeaders();

            var authHeader = new HttpHeader("Cache-Control", "no-cache");
            headers.AddHeader(authHeader);

            var customHeader = new HttpHeader("MyCustomHeader", "foo");
            headers.AddHeader(customHeader);

            request.Header = headers;

            //Dispatches it!
            var response = _client.Dispatch(request);

            CrestronConsole.PrintLine("Response is\n" + response.ContentString); 

        }


        public void Dispose()
        {
            _client.Dispose();
        }

    }
}