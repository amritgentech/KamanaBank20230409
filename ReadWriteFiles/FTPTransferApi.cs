using System;
using System.Text;
using System.IO;
using System.Net;

namespace ReadWriteFiles
{
    public class FTPTransferApi
    {
        string ftpServerIP;
        string ftpUserID;
        string ftpPassword;

        public FTPTransferApi(string serverIp, string userName, string password)
        {
            ftpServerIP = serverIp;
            ftpUserID = userName;
            ftpPassword = password;
        }

        public string GetFtpServerIp()
        {
            return ftpServerIP;
        }

        public string GetFtpUserId()
        {
            return ftpUserID;
        }

        public string GetFtpPassword()
        {
            return ftpPassword;
        }

        /// <summary>
        /// Method to upload the specified file to the specified FTP Server
        /// </summary>
        /// <param name="filename">file full name to be uploaded</param>
        public void Upload(string filename)
        {
            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
            FtpWebRequest reqFTP;

            // Create FtpWebRequest object from the Uri provided
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

            // Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.UsePassive = false;
            // By default KeepAlive is true, where the control connection is not closed
            // after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInf.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
            FileStream fs = fileInf.OpenRead();

            try
            {
                // Stream to which the file to be upload is written
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // Close the file stream and the Request Stream
                strm.Close();
                fs.Close();
                reqFTP.Abort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to upload the specified file in specified directory to the specified FTP Server
        /// </summary>
        /// <param name="filename">file full name to be uploaded</param>
        /// <param name="dirName">directory where file full name to be uploaded</param>
        public void Upload(string filename,string dirName)
        {
            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + ftpServerIP + "/" + dirName + "/" + fileInf.Name;
            FtpWebRequest reqFTP;

            // Create FtpWebRequest object from the Uri provided
            reqFTP = (FtpWebRequest) FtpWebRequest.Create(new Uri(uri));

            // Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.UsePassive = false;
            // By default KeepAlive is true, where the control connection is not closed
            // after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInf.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
            FileStream fs = fileInf.OpenRead();

            try
            {
                // Stream to which the file to be upload is written
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // Close the file stream and the Request Stream
                strm.Close();
                fs.Close();
                reqFTP.Abort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to delete the specified file from ftp server.
        /// </summary>
        /// <param name="fileName">filename to be deleted</param>
        public void DeleteFTP(string fileName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + fileName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileName));

                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.UsePassive = false;

                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
                reqFTP.Abort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to get all list of files from ftpserver.
        /// </summary>
        /// <returns>return array of string for file detail</returns>
        public string[] GetFilesDetailList()
        {
            //string[] downloadFiles;
            WebResponse response = null;
            StreamReader reader = null;
            FtpWebRequest ftp = null;
            try
            {
                StringBuilder result = new StringBuilder();
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/"));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftp.UsePassive = false;
                response = ftp.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }

                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                ftp.Abort();
                return result.ToString().Split('\n');
                //MessageBox.Show(result.ToString().Split('\n'));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        /// Method to get file list.
        /// </summary>
        /// <returns>array of string</returns>
        /// 
        public string[] GetFileList()
        {
            //string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/"));
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                //MessageBox.Show(reader.ReadToEnd());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                //MessageBox.Show(response.StatusDescription);
                reqFTP.Abort();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        //public string[] GetFileList1()
        //{
        //    //string[] downloadFiles;
        //    StringBuilder result = new StringBuilder();
        //    FtpWebRequest reqFTP;
        //    try
        //    {
        //        reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/"));
        //        reqFTP.UseBinary = true;
        //        reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        //        reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                
        //        WebResponse response = reqFTP.GetResponse();
        //        StreamReader reader = new StreamReader(response.GetResponseStream());

        //        string line = reader.ReadLine();
                
        //        while (line != null)
        //        {

        //            FtpWebRequest  reqSize = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + line));
        //            reqSize.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        //            reqSize.Method = WebRequestMethods.Ftp.GetFileSize;
        //            FtpWebResponse respSize = (FtpWebResponse)reqSize.GetResponse();

        //            long size = GetFileSize(line);// respSize.ContentLength;
                
        //            result.Append(line);
        //            result.Append("†");
        //            result.Append(size);
        //            result.Append('‡');
        //            line = reader.ReadLine();
        //        }
        //        result.Remove(result.ToString().LastIndexOf('‡'), 1);
        //        reader.Close();
        //        response.Close();

        //        return result.ToString().Split('‡');
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// Method to download the specified file from ftpserver to the specified path
        /// </summary>
        /// <param name="filePath">path where file downloaded</param>
        /// <param name="fileName">file to be downloaded</param>
        public void Download(string filePath, string rFileName, string oFileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                //filePath = <<The full path where the file is to be created.>>, 
                //fileName = <<Name of the file to be created(Need not be the name of the file on FTP server).>>
                FileStream outputStream = new FileStream(filePath + rFileName, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + oFileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
                reqFTP.Abort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to get size of file.
        /// </summary>
        /// <param name="filename">full file name</param>
        /// <returns>long as size</returns>
        public long GetFileSize(string filename)
        {
            FtpWebRequest reqFTP;
            long fileSize = 0;
            FtpWebResponse response = null;
            Stream ftpStream = null;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + filename));
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;

                reqFTP.Abort();
                return fileSize;
            }
            catch (Exception ex)
            {
                throw ex;
            }finally
            {
                ftpStream.Close();
                response.Close();
            }
        }

        /// <summary>
        /// Method to Rename file in ftpserver.
        /// </summary>
        /// <param name="currentFilename">current file name</param>
        /// <param name="newFilename">new file name</param>
        public void Rename(string currentFilename, string newFilename)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + currentFilename));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();
                reqFTP.Abort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to create specified directory in ftp server.
        /// </summary>
        /// <param name="dirName">dir name to be created</param>
        public void MakeDir(string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                // dirName = name of the directory to create.
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + dirName));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();
                reqFTP.Abort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Chect Connection with Specified FTP.
        /// </summary>
        /// <returns>bool=> True for successfully connection else false.</returns>
        public bool FtpValidConnection()
        {
            try
            {
                string uri = "ftp://" + ftpServerIP;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.UsePassive = false;
                request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                request.GetResponse();
                request.Abort();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public WebResponse GetFtpConnestionStatus() {
            try
            {
                string uri = "ftp://" + ftpServerIP;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.UsePassive = false;
                request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                WebResponse webR = request.GetResponse();
                request.Abort();

                return webR;
            }
            catch
            {
                throw;
            }
        }

        public string[] GetOnlyFileList()
        {
            //string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                reqFTP.UsePassive = false;
                WebResponse response = reqFTP.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //MessageBox.Show(reader.ReadToEnd());
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (!(line.IndexOf("<DIR>") >= 0))
                    {
                        string[] strSpit = line.Split(Convert.ToChar(13));
                        result.Append(strSpit[strSpit.Length - 1]);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                reqFTP.Abort();
                //MessageBox.Show(response.StatusDescription);
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public System.Collections.Generic.List<string> GetFilesList()
        {
            //string[] downloadFiles;
            System.Collections.Generic.List<string> listFiles = new System.Collections.Generic.List<string>();

            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                reqFTP.UsePassive = false;
                WebResponse response = reqFTP.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //MessageBox.Show(reader.ReadToEnd());
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (!(line.IndexOf("<DIR>") >= 0))
                    {
                        string[] strSpit = line.Split(Convert.ToChar(13));
                        result.Append(strSpit[strSpit.Length - 1]);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                //MessageBox.Show(response.StatusDescription);
                listFiles.AddRange(result.ToString().Split('\n'));
                reqFTP.Abort();
                return listFiles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public System.Collections.Generic.List<string> GetFilesList(string fileName)
        {
            //string[] downloadFiles;
            System.Collections.Generic.List<string> listFiles = new System.Collections.Generic.List<string>();

            StringBuilder result = new StringBuilder();
            //FtpWebRequest reqFTP;
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                //MessageBox.Show(reader.ReadToEnd());
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (!(line.IndexOf("<DIR>") >= 0))
                    {
                        if (line.Contains(fileName))
                        {
                            string[] strSpit = line.Split(Convert.ToChar(13));
                            result.Append(strSpit[strSpit.Length - 1]);
                            result.Append("\n");
                        }
                    }
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);

                //MessageBox.Show(response.StatusDescription);
                listFiles.AddRange(result.ToString().Split('\n'));
                reqFTP.Abort();
                return listFiles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }
    }
}
