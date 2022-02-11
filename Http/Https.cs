using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Https;

namespace Http
{
    public class Https
    {
        private HttpsClient _client;

        //Constructor for this class
        public Https()
        {
            _client = new HttpsClient();
            _client.Accept = "application/json";
            _client.KeepAlive = true;

            _client.HostVerification = false;   //keeping things simple
            _client.PeerVerification = false;   //keeping things simple

        }


        public void SendRequest(string url, RequestType requestType ,string payload)
        {
            var request = new HttpsClientRequest();
            request.Url.Parse(url);
            request.RequestType = requestType;
            request.Header.ContentType = "application/json";

            //adds the body of the message
            request.ContentString = payload;
            request.ContentSource = ContentSource.ContentString;

            //builds up the headers
            var headers = new HttpsHeaders();

            var authHeader = new HttpsHeader("Cache-Control", "no-cache");
            headers.AddHeader(authHeader);

            var customHeader = new HttpsHeader("MyCustomHeader", "foo");
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