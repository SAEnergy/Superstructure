using Core.Interfaces.Components.Logging;
using Core.Logging.LogDestinationConfigs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Logging.LogDestinations
{
    public sealed class FileLogDestination : LogDestinationBase
    {
        #region Fields

        private const int BYTES_IN_MEGABYTE = 1024 * 1024;
        private const int RETRY_COUNT = 10;
        private TimeSpan _retryWait = new TimeSpan(0, 0, 0, 0, 500);

        private readonly FileLogDestinationConfig _config;

        private Tuple<StreamWriter, int, string> _currentLogFileInfo;
        private string _expandedDirectory;

        #endregion

        #region Constructor

        public FileLogDestination(FileLogDestinationConfig config)
        {
            if (config != null)
            {
                _config = config;
            }
            else
            {
                throw new ArgumentNullException("config");
            }
        }

        #endregion

        #region Public Methods

        public override void ReportMessages(List<LogMessage> messages)
        {
            if (_currentLogFileInfo == null)
            {
                _expandedDirectory = EnsureDirectoryExists();

                _currentLogFileInfo = OpenFile(_expandedDirectory, GetFirstFileCounter(_expandedDirectory));
            }

            if (_currentLogFileInfo != null)
            {
                if(IsFileTooLarge(_currentLogFileInfo.Item3))
                {
                    CloseFile(_currentLogFileInfo.Item1);

                    _currentLogFileInfo = OpenFile(_expandedDirectory, _currentLogFileInfo.Item2 + 1);
                }

                if (_currentLogFileInfo != null)
                {
                    foreach (var message in messages)
                    {
                        _currentLogFileInfo.Item1.WriteLine(_config.LogMessageFormatter.Format(message));
                    }

                    _currentLogFileInfo.Item1.Flush();
                }
            }
        }

        public override void ShutDownDestination()
        {
            if (_currentLogFileInfo != null)
            {
                CloseFile(_currentLogFileInfo.Item1);
            }
        }

        #endregion

        #region Private Methods

        private bool IsFileTooLarge(string filename)
        {
            FileInfo info = new FileInfo(filename);

            return info.Length > _config.MaxLogFileSize * BYTES_IN_MEGABYTE;
        }

        private void CloseFile(StreamWriter writer)
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Close();
            }
        }

        private int GetFirstFileCounter(string directory)
        {
            int currentFileNumber = -1;

            var files = Directory.GetFiles(directory, string.Format("{0}_*.{1}", _config.LogFilePrefix, _config.LogFileExtension), SearchOption.TopDirectoryOnly);

            //try to preserve files, and resume on the next file
            if (files.Length >= _config.MaxLogFileCount)
            {
                Tuple<string, DateTime> oldestFile = null;

                //recycle oldest file
                foreach (string file in files)
                {
                    DateTime writeTime = File.GetLastWriteTime(file);

                    if (oldestFile == null || oldestFile.Item2 > writeTime)
                    {
                        oldestFile = new Tuple<string, DateTime>(file, writeTime);
                    }
                }

                currentFileNumber = TryGetFileNumber(oldestFile.Item1);
            }
            else
            {
                currentFileNumber = files.Length;
            }

            return currentFileNumber;
        }

        private int TryGetFileNumber(string fileName)
        {
            int retVal = 0;

            string itemToProcess = fileName.Substring(0, fileName.LastIndexOf('.'));

            string substring = itemToProcess.Substring(itemToProcess.LastIndexOf('_') + 1);

            try
            {
                int parsedNumber = Convert.ToInt32(substring);

                retVal = parsedNumber < _config.MaxLogFileCount ? parsedNumber : 0; //reset if the older filenumber is greater than logfilecount
            }
            catch(Exception ex)
            {
                _logger.HandleLoggingException(string.Format("Error in FileLogDestination \"{0}\"", ex.Message));
            }

            return retVal;
        }

        private string EnsureDirectoryExists()
        {
            //expand environment variables first, then get full path
            string expandedDirectory = Path.GetFullPath(Environment.ExpandEnvironmentVariables(_config.LogDirectory));

            if (!Directory.Exists(expandedDirectory))
            {
                //_logger.Log(string.Format("Creating directory \"{0}\" to store logs.", _config.LogDirectory));

                Directory.CreateDirectory(expandedDirectory);
            }

            return expandedDirectory;
        }

        private Tuple<StreamWriter, int, string> OpenFile(string directory, int currentNumber)
        {
            Tuple<StreamWriter, int, string> retVal = null;

            currentNumber = currentNumber < _config.MaxLogFileCount ? currentNumber : 0;

            //we're trying to log, if we can't log, we should loop forever, at least while we're still running
            while (retVal == null && (IsRunning || !_destinationQueue.IsQueueEmpty))
            {
                string fileName = GetFileName(directory, currentNumber);

                StreamWriter stream = TryCreateAndLockNewFile(fileName);

                if (stream != null)
                {
                    //_logger.Log(string.Format("FileLogDestination now logging to \"{0}\".", fileName));

                    retVal = new Tuple<StreamWriter, int, string>(stream, currentNumber, fileName);
                    break;
                }

                currentNumber = currentNumber + 1 < _config.MaxLogFileCount ? currentNumber + 1 : 0;
            }

            return retVal;
        }

        private StreamWriter TryCreateAndLockNewFile(string fileName)
        {
            StreamWriter retVal = null;

            bool successful = false;
            int retryCount = 0;

            while (!successful && retryCount < RETRY_COUNT && (IsRunning || !_destinationQueue.IsQueueEmpty))
            {
                retryCount++;

                try
                {
                    FileStream fileStream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);

                    retVal = new StreamWriter(fileStream);

                    successful = true;
                    retVal.WriteLine(_config.LogMessageFormatter.GetHeader());
                    retVal.Flush();
                }
                catch (Exception ex)
                {
                    _logger.HandleLoggingException(string.Format("Error in FileLogDestination \"{0}\"", ex.Message));
                }

                if (!successful)
                {
                    _logger.HandleLoggingException(string.Format("Failed to open file \"{0}\".  Retry number {1}/{2}", fileName, retryCount, RETRY_COUNT));
                    Thread.Sleep(_retryWait);
                }
            }

            return retVal;
        }

        private string GetFileName(string directory, int number)
        {
            int digits = (int)Math.Floor(Math.Log10(_config.MaxLogFileCount) + 1) + 1; //find number of digits in file count, and add one more

            string fileNumber = number.ToString().PadLeft(digits, '0');

            return string.Format("{0}\\{1}_{2}.{3}", directory, _config.LogFilePrefix, fileNumber, _config.LogFileExtension);
        }

        #endregion
    }
}
