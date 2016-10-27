﻿using Core.Interfaces.Components.Logging;
using Core.Models.Persistent;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Core.Scheduler.Jobs
{
    public sealed class RunProgramJob : JobBase
    {
        #region Fields

        private TimeSpan _processWaitTimer = TimeSpan.FromSeconds(1);

        #endregion

        #region Constructor

        public RunProgramJob(ILogger logger, JobConfiguration config) : base(logger, config)
        {

        }

        #endregion

        #region Public Methods

        public override bool Execute(CancellationToken ct)
        {
            bool rc = false;

            var proc = CreateProcess();

            if (proc != null)
            {
                proc.Start();
                StartCapturingOutput(proc);

                while (!proc.WaitForExit((int)_processWaitTimer.TotalMilliseconds))
                {
                    if (ct.IsCancellationRequested)
                    {
                        //if we are not going to kill the proc, then leave it to do it's thing
                        if (Configuration.KillProcOnCancel)
                        {
                            _logger.Log(string.Format("Job \"{0}\" is configured to kill process on cancel.  Killing process...", Configuration.Name), LogMessageSeverity.Error);

                            proc.Kill();
                        }

                        StopCapturingOutput(proc);

                        ct.ThrowIfCancellationRequested();
                    }
                }

                proc.WaitForExit(); //according to MSDN call this even after the timeout above and returned true

                StopCapturingOutput(proc);

                rc = proc.ExitCode == 0;
            }
            else
            {
                _logger.Log(string.Format("Unable to start process for job \"{0}\".", Configuration.Name), LogMessageSeverity.Error);
            }

            return rc;
        }

        #endregion

        #region Private Methods

        private Process CreateProcess()
        {
            Process proc = null;

            string path = Path.GetFullPath(Environment.ExpandEnvironmentVariables(Configuration.WorkingDirectory));

            string fileName = string.Format("{0}\\{1}", path, Configuration.FileName);

            if (File.Exists(fileName))
            {
                var info = new ProcessStartInfo();
                info.FileName = fileName;
                info.WorkingDirectory = path;
                info.Arguments = Configuration.Arguments;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardOutput = Configuration.CaptureOutput;
                info.RedirectStandardError = Configuration.CaptureOutput;

                proc = new Process();
                proc.StartInfo = info;

                if (Configuration.CaptureOutput)
                {
                    proc.OutputDataReceived += (sender, args) =>
                    {
                        _logger.Log(string.Format("Job \"{0}\": {1}", Configuration.Name, args.Data));
                    };

                    proc.ErrorDataReceived += (sender, args) =>
                    {
                        _logger.Log(string.Format("Job \"{0}\": {1}", Configuration.Name, args.Data), LogMessageSeverity.Error);
                    };
                }
            }
            else
            {
                _logger.Log(string.Format("Job \"{0}\" misconfigured.  Unable to locate file \"{1}\" in directory \"{2}.", Configuration.Name, Configuration.FileName, path), LogMessageSeverity.Error);
            }

            return proc;
        }

        private void StartCapturingOutput(Process proc)
        {
            if (Configuration.CaptureOutput)
            {
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
            }
        }

        private void StopCapturingOutput(Process proc)
        {
            if (Configuration.CaptureOutput)
            {
                proc.CancelOutputRead();
                proc.CancelErrorRead();
            }
        }

        #endregion
    }
}
