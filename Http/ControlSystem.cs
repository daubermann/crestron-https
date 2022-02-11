using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;         	// For Generic Device Support
using Crestron.SimplSharp.Net;
using Crestron.SimplSharp.Net.Http;
using Crestron.SimplSharp.Net.Https;

namespace Http
{
    /// <summary>
    /// This program instantiates two types of http client: non-secure and secure.
    /// To make the tests easy, I created two console commands which create a GET request to httpbin.org. The console commands are: "getit" and "getitsecure"
    /// When typing "getit" (in the console), a non-secure GET request is sent to httpbin.org
    /// When typing "getitsecure" (in the console), a secure GET request is sent to httpbin.org
    /// 
    /// Both requests above seem to work (sort of), however the secure version created by Crestron does not include the Content-Length in the header.
    /// The httpbin.org service echoes back a json object that includes everything we sent on the request so we can easily troubleshoot and see each header that we sent.
    /// By analyzing this data, we can clearly see that when using SimplSharp.Net.Http, the Content-Length is properly generated and sent in the header (as expected).
    /// However, when using SimplSharp.Net.Https, the Content-Length header is NOT generated (this is unexpected). 
    /// (And if I try to manually add the Content-Length in the header, the Crestron program becomes unresponsive)
    /// 
    /// This creates a problem, because the other end does expect a Content-Lenght to function properly.
    /// 
    /// I am currently working on a project where I must include the Content-Lenght on a Https request.
    /// Any ideas on what I need to do to make Crestron's Https library to properly generate and send the Content-Length? 
    /// And if not, what do I need to do to manually add the Content-Length and not having the program crash?
    /// </summary>
    public class ControlSystem : CrestronControlSystem
    {
        Http _myHttpClient;     //non-secure version
        Https _myHttpsClient;   //secure version

        string _jsonSamplePayload = "{\"foo\":\"hello world\"}"; //a tiny piece of json to be used as a payload example

        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(ControlSystem_ControllerProgramEventHandler);

            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }


        public override void InitializeSystem()
        {
            try
            {
                //Adds a couple of custom console commands to be used to trigger the http request
                CrestronConsole.AddNewConsoleCommand(ConsoleCommandGetIt, "getit", "sends a non-secure GET request to httpbin with a small payload", ConsoleAccessLevelEnum.AccessAdministrator); //for tests
                CrestronConsole.AddNewConsoleCommand(ConsoleCommandGetItSecure, "getitsecure", "sends a secure GET request to httpbin with a small payload", ConsoleAccessLevelEnum.AccessAdministrator); //for tests

                _myHttpClient = new Http();
                _myHttpsClient = new Https();
            
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }




        #region Dispatch Http Client Examples
        private void ConsoleCommandGetIt(string cmdParameters)
        {
            //HTTP Non-Secure version!
            string url = "http://httpbin.org/anything"; 
            //Setting up a GET request
            Crestron.SimplSharp.Net.Http.RequestType requestType = Crestron.SimplSharp.Net.Http.RequestType.Get;
            //Dispatching it!
            _myHttpClient.SendRequest(url, requestType, _jsonSamplePayload);
        }


        private void ConsoleCommandGetItSecure(string cmdParameters)
        {
            //HTTPS Secure version!
            string url = "https://httpbin.org/anything";
            //Setting up a GET request
            Crestron.SimplSharp.Net.Https.RequestType requestType = Crestron.SimplSharp.Net.Https.RequestType.Get;
            //Dispatching it!
            _myHttpsClient.SendRequest(url, requestType, _jsonSamplePayload);
        }
        #endregion


        #region Proper Disposal
        void ControlSystem_ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {

                case (eProgramStatusEventType.Stopping):
                    _myHttpClient.Dispose(); //ensures the client terminates gracefully
                    _myHttpsClient.Dispose();//ensures the client terminates gracefully
                    break;
            }

        }
        #endregion
    }
}