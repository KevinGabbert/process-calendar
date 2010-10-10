using System;

namespace ProcessCalendar
{
    class Program
    {
        public const string VERSION = "Process Calendar v10.09.10 ";
        public const string CONFIG_FILE_PATH = @"..\..\ProcessToLog.xml";

        static void Main(string[] args)
        {
            Console.WriteLine(VERSION + DateTime.Now.ToShortTimeString());

            var userInfoForm = new Login_Form();
            userInfoForm.ShowDialog();  
            
            //*** Code Execution will stop at this point and wait until user has dismissed the Login form. ***//

            Log.LogVersion = VERSION;
            Log.Log_Watched_Processes(Config.GetWatchList(CONFIG_FILE_PATH), userInfoForm.User, userInfoForm.Password, userInfoForm.PostURI);
        } 
    }
}