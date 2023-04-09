using System;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections;

namespace ReadWriteFiles
{
    public class NTShareFileDownloadApi
    {
        string ntServerIP;
        string ntUserID;
        string ntPassword;

        public NTShareFileDownloadApi(string serverIp, string userName, string password)
        {
            ntServerIP = serverIp;
            ntUserID = userName;
            ntPassword = password;
        }

        public string GetNtServerIp()
        {
            return ntServerIP;
        }

        public string GetNtUserId()
        {
            return ntUserID;
        }

        public string GetNtPassword()
        {
            return ntPassword;
        }

        /// <summary>
        /// Check Connection with Specified Nt.
        /// </summary>
        /// <returns>bool=> True for successfully connection else false.</returns>
        public bool NtValidConnection()
        {
            try
            {
                string uri = "\\\\" + ntServerIP + "\\";
                var credentials = new NetworkCredential(ntUserID, ntPassword);
                
                using (var netCon = new NetworkConnection(uri, credentials))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(uri);
                    if (dirInfo.Exists)
                    {
                        FileInfo[] filesInfo = dirInfo.GetFiles();
                    }
                    else {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get list of File from ShareFloder
        /// </summary>
        /// <returns>List of File in ArrayList</returns>
        public ArrayList GetFilesDetailList()
        {
            try
            {
                string uri = "\\\\" + ntServerIP + "\\";
                var credentials = new NetworkCredential(ntUserID, ntPassword);

                ArrayList arrylist = new ArrayList();
                using (var netCon = new NetworkConnection(uri, credentials))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(uri);
                    if (dirInfo.Exists)
                    {
                        FileInfo[] filesInfo = dirInfo.GetFiles();
                        arrylist.AddRange(filesInfo);
                        DirectoryInfo[] dirInfoColl = dirInfo.GetDirectories();
                        arrylist.AddRange(dirInfoColl);
                    }
                }
                return arrylist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ArrayList GetFilesDetailListwithOutPassword()
        {
            string uri = "\\\\" + ntServerIP + "\\";
            ArrayList arrylist = new ArrayList();
            DirectoryInfo dirInfo = new DirectoryInfo(uri);

            FileInfo[] filesInfo = dirInfo.GetFiles();
            arrylist.AddRange(filesInfo);
            DirectoryInfo[] dirInfoColl = dirInfo.GetDirectories();
            arrylist.AddRange(dirInfoColl);

            return arrylist;
        }

        public ArrayList GetFilesWithUserNameAndPassword(String username, String password)
        {
            string uri = "\\\\" + ntServerIP + "\\";
            NetworkCredential credentials = new NetworkCredential(username, password);
            ArrayList arrylist = new ArrayList();
            NetworkConnection conenction = new NetworkConnection(uri, credentials);
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(uri);

                FileInfo[] filesInfo = dirInfo.GetFiles();
                arrylist.AddRange(filesInfo);
                DirectoryInfo[] dirInfoColl = dirInfo.GetDirectories();
                arrylist.AddRange(dirInfoColl);
                return arrylist;
            }
            catch
            {
                throw;
            }
            finally {
                if (conenction != null)
                    conenction.Dispose();
            }
        }

        public ArrayList GetFileDetail(string fileName)
        {
            try
            {
                string uri = "\\\\" + ntServerIP + "\\" + fileName;
                var credentials = new NetworkCredential(ntUserID, ntPassword);

                ArrayList arrylist = new ArrayList();
                using (var netCon = new NetworkConnection(uri, credentials))
                {
                    if (File.Exists(uri)) {
                        FileInfo fileInfo = new FileInfo(uri);
                        arrylist.Add(fileInfo);
                    }
                }
                return arrylist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to download the specified file from ntserver to the specified path
        /// </summary>
        /// <param name="filePath">path where file downloaded</param>
        /// <param name="fileName">file to be downloaded</param>
        public void Download(string filePath, string fileName)
        {
            try
            {
                string sourceFilePath = "\\\\" + ntServerIP + "\\" + fileName;
                string destinationFilePath = filePath;
                var credentials = new NetworkCredential(ntUserID, ntPassword);
                using (var netCon = new NetworkConnection(sourceFilePath, credentials))
                {
                    File.Copy(sourceFilePath, destinationFilePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class NetworkConnection : IDisposable
    {
        string _networkName;

        public NetworkConnection(string networkName,
            NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            var result = WNetAddConnection2(
                netResource,
                credentials.Password,
                credentials.UserName,
                0);

            if (result != 0)
            {
                new Win32Exception(result, "Error connecting to remote share (Error Code " + result.ToString() + ")");
            }
        }

        ~NetworkConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        public ResourceScope Scope;
        public ResourceType ResourceType;
        public ResourceDisplaytype DisplayType;
        public int Usage;
        public string LocalName;
        public string RemoteName;
        public string Comment;
        public string Provider;
    }

    public enum ResourceScope : int
    {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    };

    public enum ResourceType : int
    {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8,
    }

    public enum ResourceDisplaytype : int
    {
        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }
}
