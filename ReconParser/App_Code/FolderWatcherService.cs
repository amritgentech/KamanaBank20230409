using ReconParser.App_Code.Recon.Ej.Wincor.Neps;
using ReconParser.App_Code.Recon.Ej.Wincor.NPN;
using ReconParser.App_Code.Recon.Ej.Wincor.SCT;
using ReconParser.App_Code.Recon.Ej.Diebold.Neps;
using ReconParser.App_Code.Recon.Ej.Diebold.NPN;
using ReconParser.App_Code.Recon.Ej.Diebold.SCT;
using ReconParser.App_Code.Recon.Ej.Ncr.Neps;
using ReconParser.App_Code.Recon.Visa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReconParser.App_Code.Recon.CBS.Flexcube;
using ReconParser.App_Code.Recon.Npn;
using ReconParser.App_Code.Recon.Sct;
using ReconParser.App_Code.Recon.HBL;
using ReconParser.App_Code.Recon.Visa.EP705;
using ReconParser.App_Code.Recon.Visa.EP707;
using ReconParser.App_Code.Recon.Visa.EP725;
using ReconParser.App_Code.Recon.Visa.EP727;
using ReconParser.App_Code.Recon.Visa.NEPS.EP745;
using ReconParser.App_Code.Recon.CBS.Pumori;
using ReconParser.App_Code.Recon.CBS.T24;
using ReconParser.App_Code.Recon.Ej.Diebold.Kumari;
using ReconParser.App_Code.Recon.Ej.Ncr.Kumari;
using ReconParser.App_Code.Recon.Ej.Ncr.NPN;
using ReconParser.App_Code.Recon.Ej.Wincor.Kumari;
using ReconParser.App_Code.Recon.MasterCard.Npn;
using ReconParser.App_Code.Recon.Neps;
using ReconParser.App_Code.Recon.Visa.NPN.EP745;
using ReconParser.App_Code.Recon.DigitalBanking.Esewa;
using ReconParser.App_Code.Recon.DigitalBanking.Nostro;
using ReconParser.App_Code.Recon.DigitalBanking.Mirror;
using ReconParser.App_Code.Recon.DigitalBanking.InternetIbft;
using ReconParser.App_Code.Recon.DigitalBanking.InternetTopup;
using ReconParser.App_Code.Recon.DigitalBanking.MobileTopup;
using ReconParser.App_Code.Recon.DigitalBanking.MobileIbft;
using ReconParser.App_Code.Recon.DigitalBanking.Cbs;
using ReconParser.App_Code.Recon.DigitalBanking.Cbs.EsewaParking;
using ReconParser.App_Code.Recon.DigitalBanking.Cbs.TopupParking;
using ReconParser.App_Code.Recon.DigitalBanking.Cbs.FonepayIbftParking;

namespace ReconParser.App_Code
{

    public class FolderWatcherService
    {
        public static String ROOT_FOLDER_LOCATION = ConfigurationManager.AppSettings["ROOT_FOLDER_LOCATION"].ToString();

        private static String VISA_EP707_FOLDER = ROOT_FOLDER_LOCATION + @"VISA\EP707";
        private static String VISA_EP727_FOLDER = ROOT_FOLDER_LOCATION + @"VISA\EP727";
        private static String VISA_EP705_FOLDER = ROOT_FOLDER_LOCATION + @"VISA\EP705";
        private static String VISA_EP725_FOLDER = ROOT_FOLDER_LOCATION + @"VISA\EP725";
        private static String VISA_NEPS_EP745_FOLDER = ROOT_FOLDER_LOCATION + @"VISA\EP745\NEPS";
        private static String VISA_NPN_EP745_FOLDER = ROOT_FOLDER_LOCATION + @"VISA\EP745\NPN";
        private static String SCT_FOLDER = ROOT_FOLDER_LOCATION + @"SCT";
        private static String NPN_FOLDER = ROOT_FOLDER_LOCATION + @"NPN";
        private static String HBL_FOLDER = ROOT_FOLDER_LOCATION + @"HBL";
        private static String NEPS_FOLDER = ROOT_FOLDER_LOCATION + @"NEPS";
        private static String EJ_WINCOR_KUMARI_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\WINCOR\KUMARI";
        private static String EJ_WINCOR_NEPS_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\WINCOR\NEPS";
        private static String EJ_WINCOR_NPN_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\WINCOR\NPN";
        private static String EJ_WINCOR_SCT_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\WINCOR\SCT";

        private static String EJ_DIEBOLD_KUMARI_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\DIEBOLD\KUMARI";
        private static String EJ_DIEBOLD_NEPS_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\DIEBOLD\NEPS";
        private static String EJ_DIEBOLD_NPN_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\DIEBOLD\NPN";
        private static String EJ_DIEBOLD_SCT_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\DIEBOLD\SCT";

        private static String EJ_NCR_KUMARI_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\NCR\KUMARI";
        private static String EJ_NCR_NEPS_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\NCR\NEPS";
        private static String EJ_NCR_NPN_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\NCR\NPN";
        private static String EJ_NCR_SCT_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\NCR\SCT";

        private static String EJ_VORTEX_FOLDER = @ROOT_FOLDER_LOCATION + @"EJOURNAL\VORTEX\NPN";

        private static String CBS_Pumori_FOLDER = @ROOT_FOLDER_LOCATION + @"CBS\Pumori";
        private static String CBS_FlexCube_FOLDER = @ROOT_FOLDER_LOCATION + @"CBS\FlexCube";
        private static String CBS_T24_FOLDER = @ROOT_FOLDER_LOCATION + @"CBS\T24";

        private static String MASTER_CARD_NPN = @ROOT_FOLDER_LOCATION + @"MasterCard\NPN";
        //Digital Banking
        private static String DIGITALBANKING__ESEWA_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\ESEWA";
        private static String DIGITALBANKING__NOSTRO_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\NOSTRO";
        private static String DIGITALBANKING__MIRROR_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\MIRROR";
        private static String DIGITALBANKING__INTERNETIBFT_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\INTERNETIBFT";
        private static String DIGITALBANKING__INTERNETTOPUP_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\INTERNETTOPUP";
        private static String DIGITALBANKING__MOBILEIBFT_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\MOBILEIBFT";
        private static String DIGITALBANKING__MOBILETOPUP_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\MOBILETOPUP";
        private static String DIGITALBANKING__CBS_EESWAPARKING_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\CBS\ESEWAPARKING";
        private static String DIGITALBANKING__CBS_TOPUPPARKING_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\CBS\TOPUPPARKING";
        private static String DIGITALBANKING__CBS_FONEPAYIBFTPARKING_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\CBS\FONEPAYIBFTPARKING";
        private static String DIGITALBANKING__CBS_FOLDER = @ROOT_FOLDER_LOCATION + @"DIGITALBANKING\CBS";

        public Int32 TotalNoOfFoldersToWatch = -1;
        private FileSystemWatcher[] watcher;

        private int index = 0;
        private int FileCount = 0;

        public FolderWatcherService(int FileCount)
        {
            this.FileCount = FileCount;
            TotalNoOfFoldersToWatch = CountFolders(null);
            watcher = new FileSystemWatcher[TotalNoOfFoldersToWatch];
        }
        public void BindFolderWatcher(String folder)
        {
            if (folder == null)
            {
                folder = ROOT_FOLDER_LOCATION;
            }

            IEnumerable<String> folders = Directory.EnumerateDirectories(folder);

            foreach (String _file in folders)
            {
                if (Directory.Exists(_file))
                {
                    Console.WriteLine("Folder detected {0}. Now binding file watcher service", _file);
                    WatchRecon(_file, index);
                    BindFolderWatcher(_file);
                    index++;
                }
            }
        }

        private int index1 = 0;
        public int CountFolders(String folder)
        {
            if (folder == null)
            {
                folder = ROOT_FOLDER_LOCATION;
            }

            IEnumerable<String> folders = Directory.EnumerateDirectories(folder);

            foreach (String _file in folders)
            {
                if (Directory.Exists(_file))
                {
                    Console.WriteLine("Folder detected {0}. Now binding file watcher service", _file);
                    CountFolders(_file);
                    index1++;
                }
            }

            return index1;
        }

        public void init()
        {
            BindFolderWatcher(null);
        }

        public void WatchRecon(String folder, int index)
        {
            watcher[index] = new FileSystemWatcher(folder);
            watcher[index].NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher[index].Created += new FileSystemEventHandler(OnReconFileAdded);
            watcher[index].EnableRaisingEvents = true;
            Console.WriteLine("Folder watcher binded {0}", folder);
        }

        private void OnReconFileAdded(object sender, FileSystemEventArgs e)
        {
            try
            {
                String fullPath = e.FullPath;
                Console.WriteLine("Detected new file. {0}", e.FullPath);

                Task.Delay(60000);

                if (fullPath.Contains(EJ_WINCOR_NEPS_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_WINCOR_NEPS_FOLDER);
                    NEPSWincor wincor = new NEPSWincor(fullPath, FileCount);
                    wincor.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", wincor.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_WINCOR_NPN_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_WINCOR_NPN_FOLDER);
                    NPNWincor wincor = new NPNWincor(fullPath, FileCount);
                    wincor.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", wincor.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_WINCOR_KUMARI_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_WINCOR_NPN_FOLDER);
                    KumariYcsWincor wincor = new KumariYcsWincor(fullPath, FileCount);
                    wincor.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", wincor.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_WINCOR_SCT_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_WINCOR_SCT_FOLDER);
                    SCTWincor wincor = new SCTWincor(fullPath, FileCount);
                    wincor.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", wincor.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_DIEBOLD_NEPS_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_DIEBOLD_NEPS_FOLDER);
                    NepsDiebold diebold = new NepsDiebold(fullPath, FileCount);
                    diebold.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", diebold.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_DIEBOLD_KUMARI_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_DIEBOLD_KUMARI_FOLDER);
                    KumariYcsDiebold diebold = new KumariYcsDiebold(fullPath, FileCount);
                    diebold.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", diebold.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_DIEBOLD_NPN_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_DIEBOLD_NPN_FOLDER);
                    NPNDiebold diebold = new NPNDiebold(fullPath, FileCount);
                    diebold.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", diebold.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_DIEBOLD_SCT_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_DIEBOLD_SCT_FOLDER);
                    SCTDiebold diebold = new SCTDiebold(fullPath, FileCount);
                    diebold.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", diebold.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_NCR_NEPS_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_NCR_NEPS_FOLDER);
                    NepsNcr ncr = new NepsNcr(fullPath, FileCount);
                    ncr.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", ncr.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_NCR_NPN_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_NCR_NPN_FOLDER);
                    NPNNcr ncr = new NPNNcr(fullPath, FileCount);
                    ncr.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", ncr.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_NCR_KUMARI_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_NCR_KUMARI_FOLDER);
                    KumariYcsNcr ncr = new KumariYcsNcr(fullPath, FileCount);
                    ncr.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", ncr.Transactions.Count);
                }
                else if (fullPath.Contains(NPN_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", NPN_FOLDER);
                    NPN _NPN = new NPN(fullPath, FileCount);
                    _NPN.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _NPN.Transactions.Count);
                }
                else if (fullPath.Contains(MASTER_CARD_NPN))
                {
                    Console.WriteLine("Inside: {0}", MASTER_CARD_NPN);
                    MasterCardNpn _MasterCard = new MasterCardNpn(fullPath, FileCount);
                    _MasterCard.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _NPN.Transactions.Count);
                }
                else if (fullPath.Contains(SCT_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", SCT_FOLDER);
                    SCT _Sct = new SCT(fullPath, FileCount);
                    _Sct.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _Sct.Transactions.Count);
                }
                else if (fullPath.Contains(HBL_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", HBL_FOLDER);
                    HBL _HBL = new HBL(fullPath, FileCount);
                    _HBL.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _HBL.Transactions.Count);
                }
                else if (fullPath.Contains(VISA_EP705_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", VISA_EP705_FOLDER);
                    EP705Visa _EP705Visa = new EP705Visa(fullPath, FileCount);
                    _EP705Visa.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP705Visa.Transactions.Count);
                }
                else if (fullPath.Contains(VISA_EP707_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", VISA_EP707_FOLDER);
                    EP707Visa _EP707Visa = new EP707Visa(fullPath, FileCount);
                    _EP707Visa.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP707Visa.Transactions.Count);
                }
                else if (fullPath.Contains(VISA_EP725_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", VISA_EP725_FOLDER);
                    EP725Visa _EP725Visa = new EP725Visa(fullPath, FileCount);
                    _EP725Visa.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP725Visa.Transactions.Count);
                }
                else if (fullPath.Contains(VISA_EP727_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", VISA_EP727_FOLDER);
                    EP727Visa _EP727Visa = new EP727Visa(fullPath, FileCount);
                    _EP727Visa.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP727Visa.Transactions.Count);
                }
                else if (fullPath.Contains(VISA_NEPS_EP745_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", VISA_NEPS_EP745_FOLDER);
                    EP745VisaNeps _EP745Visa = new EP745VisaNeps(fullPath, FileCount);
                    _EP745Visa.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP745Visa.Transactions.Count);
                }
                else if (fullPath.Contains(VISA_NPN_EP745_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", VISA_NPN_EP745_FOLDER);
                    EP745VisaNpn _EP745Visa = new EP745VisaNpn(fullPath, FileCount);
                    _EP745Visa.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP745Visa.Transactions.Count);
                }
                else if (fullPath.Contains(CBS_Pumori_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", CBS_Pumori_FOLDER);
                    Pumori _Pumori = new Pumori(fullPath, FileCount);
                    _Pumori.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP745Visa.Transactions.Count);
                }
                else if (fullPath.Contains(CBS_FlexCube_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", CBS_FlexCube_FOLDER);
                    Flexcube _flexcube = new Flexcube(fullPath, FileCount);
                    _flexcube.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP745Visa.Transactions.Count);
                }
                else if (fullPath.Contains(NEPS_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", NEPS_FOLDER);
                    NEPS _NEPS = new NEPS(fullPath, FileCount);
                    _NEPS.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP745Visa.Transactions.Count);
                }
                else if (fullPath.Contains(CBS_T24_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", CBS_T24_FOLDER);
                    T24 _t24 = new T24(fullPath, FileCount);
                    _t24.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP745Visa.Transactions.Count);
                }
                else if (fullPath.Contains(EJ_VORTEX_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", EJ_VORTEX_FOLDER);
                    NPNVortex _vtx = new NPNVortex(fullPath, FileCount);
                    _vtx.Start();
                    FileCount -= 1;
                    //Console.WriteLine("Finished process file. Saved {0} Transactions.", _EP745Visa.Transactions.Count);
                }
                //Digital Banking
                else if (fullPath.Contains(DIGITALBANKING__ESEWA_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__ESEWA_FOLDER);
                    Esewa _Esewa = new Esewa(fullPath, FileCount);
                    _Esewa.Start();
                    FileCount -= 1;
                }
                else if (fullPath.Contains(DIGITALBANKING__NOSTRO_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__NOSTRO_FOLDER);
                    Nostro _Nostro = new Nostro(fullPath, FileCount);
                    _Nostro.Start();
                    FileCount -= 1;
                }
                else if (fullPath.Contains(DIGITALBANKING__MIRROR_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__MIRROR_FOLDER);
                    Mirror _Mirror = new Mirror(fullPath, FileCount);
                    _Mirror.Start();
                    FileCount -= 1;
                }
                else if (fullPath.Contains(DIGITALBANKING__INTERNETIBFT_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__INTERNETIBFT_FOLDER);
                    InternetIbft _InternetIbft = new InternetIbft(fullPath, FileCount);
                    _InternetIbft.Start();
                    FileCount -= 1;
                }
                else if (fullPath.Contains(DIGITALBANKING__INTERNETTOPUP_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__INTERNETTOPUP_FOLDER);
                    InternetTopup _InternetTopup = new InternetTopup(fullPath, FileCount);
                    _InternetTopup.Start();
                    FileCount -= 1;
                }
                else if (fullPath.Contains(DIGITALBANKING__MOBILEIBFT_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__MOBILEIBFT_FOLDER);
                    MobileIbft _MobileIbft = new MobileIbft(fullPath, FileCount);
                    _MobileIbft.Start();
                    FileCount -= 1;
                }
                else if (fullPath.Contains(DIGITALBANKING__MOBILETOPUP_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__MOBILETOPUP_FOLDER);
                    MobileTopup _MobileTopup = new MobileTopup(fullPath, FileCount);
                    _MobileTopup.Start();
                    FileCount -= 1;
                }
                else if (fullPath.Contains(DIGITALBANKING__CBS_TOPUPPARKING_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__CBS_TOPUPPARKING_FOLDER);
                    TopupParking _TopupParking = new TopupParking(fullPath, FileCount);
                    _TopupParking.Start();
                    FileCount -= 1;
                }
                else if (fullPath.Contains(DIGITALBANKING__CBS_EESWAPARKING_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__CBS_EESWAPARKING_FOLDER);
                    EsewaParking _EsewaParking = new EsewaParking(fullPath, FileCount);
                    _EsewaParking.Start();
                    FileCount -= 1;
                }
                else if (fullPath.Contains(DIGITALBANKING__CBS_FONEPAYIBFTPARKING_FOLDER))
                {
                    Console.WriteLine("Inside: {0}", DIGITALBANKING__CBS_FONEPAYIBFTPARKING_FOLDER);
                    FonepayIbftParking _FonepayIbftParking = new FonepayIbftParking(fullPath, FileCount);
                    _FonepayIbftParking.Start();
                    FileCount -= 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
