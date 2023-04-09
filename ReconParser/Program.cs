using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using BAL;
using Helper.GlobalHelpers;
using ReconParser.App_Code;

namespace ReconParser
{
    class Program
    {
        public static String ROOT_FOLDER_LOCATION
        {
            get { return ConfigurationManager.AppSettings["ROOT_FOLDER_LOCATION"]; }
        }
        public static void Main(string[] args)
        {
            //T24 _T24 = new T24("");
            //_T24.StartAutomatic();


            try
            {
                var lstFolder = new List<string>
                {
                    ROOT_FOLDER_LOCATION + @"VISA\EP707",
                    ROOT_FOLDER_LOCATION + @"VISA\EP727",
                    ROOT_FOLDER_LOCATION + @"VISA\EP705",
                    ROOT_FOLDER_LOCATION + @"VISA\EP725",
                    ROOT_FOLDER_LOCATION + @"VISA\EP745\NEPS",
                    ROOT_FOLDER_LOCATION + @"VISA\EP745\NPN",
                    ROOT_FOLDER_LOCATION + @"SCT",
                    ROOT_FOLDER_LOCATION + @"NPN",
                    ROOT_FOLDER_LOCATION + @"HBL",
                    ROOT_FOLDER_LOCATION + @"NEPS",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\WINCOR\KUMARI",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\WINCOR\NEPS",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\WINCOR\NPN",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\WINCOR\SCT",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\DIEBOLD\KUMARI",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\DIEBOLD\NEPS",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\DIEBOLD\NPN",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\DIEBOLD\SCT",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\NCR\KUMARI",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\NCR\NEPS",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\NCR\NPN",
                    ROOT_FOLDER_LOCATION + @"EJOURNAL\NCR\SCT",
                    ROOT_FOLDER_LOCATION + @"CBS\FlexCube",
                    ROOT_FOLDER_LOCATION + @"CBS\Pumori",
                    ROOT_FOLDER_LOCATION + @"CBS\T24",
                    ROOT_FOLDER_LOCATION + @"MasterCard\NPN",
                    //Digital banking folder
                     ROOT_FOLDER_LOCATION + @"DIGITALBANKING\MIRROR",
                    ROOT_FOLDER_LOCATION + @"DIGITALBANKING\NOSTRO",
                    ROOT_FOLDER_LOCATION + @"DIGITALBANKING\ESEWA",
                    ROOT_FOLDER_LOCATION + @"DIGITALBANKING\INTERNETIBFT",
                    ROOT_FOLDER_LOCATION + @"DIGITALBANKING\MOBILEIBFT",
                    ROOT_FOLDER_LOCATION + @"DIGITALBANKING\INTERNETTOPUP",
                    ROOT_FOLDER_LOCATION + @"DIGITALBANKING\MOBILETOPUP",
                    ROOT_FOLDER_LOCATION + @"DIGITALBANKING\CBS\ESEWAPARKING",
                    ROOT_FOLDER_LOCATION + @"DIGITALBANKING\CBS\FONEPAYIBFTPARKING",
                    ROOT_FOLDER_LOCATION + @"DIGITALBANKING\CBS\TOPUPPARKING"
                  
    };
                foreach (var foldeLocation in lstFolder)
                {
                    Directory.CreateDirectory(foldeLocation);
                }
                // var Values = new Db.Recon().NPNCBSTransactions();


                int FileCount = 0;

                if (args.Length == 0) // if no argument..
                {
                    FileCount = 100;  //set default file count if we don't have an argument..
                    Console.WriteLine("args is null"); // Check for null array
                }
                else
                {
                    FileCount = Convert.ToInt32(args[0]);
                }
                new FolderWatcherService(FileCount).init();
                GlobalHelper.StepLogging(FileCount.ToString());
                Console.WriteLine("Press \'q\' to quit the application.");
                while (Console.Read() != 'q') ;
            }
            catch (Exception e)
            {
                ReconBAL.UpdateIsReconStarted("False");
                GlobalHelper.ErrorLogging(e);
                //send error to mail..
                var log = ApplicationErrorLog.error(e);
                Mail.SendMail("Recon App error", log + "---" + e.Message);
            }
        }
    }
}
