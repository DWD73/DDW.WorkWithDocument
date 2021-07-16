using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Timers;

namespace DDW.Document
{
    public sealed  class DocumentsReceiver : IDisposable
    {
        readonly FileSystemWatcher fileSystemWatcher;
        readonly Timer timer;
        public List<string> documents { get; private set; }
        private bool disposed = false;
             
        public DocumentsReceiver()
        {
            documents = new List<string>();          

            timer = new Timer();

            SetTimer();

            fileSystemWatcher = new FileSystemWatcher
            {               
                NotifyFilter = NotifyFilters.FileName,                     
                EnableRaisingEvents = false,
                IncludeSubdirectories = false              
            };

            fileSystemWatcher.Created += FileSystemWatcher_Created;           
        }

        private void SetTimer()
        {          
                  
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;           
        }

        public IReadOnlyList<string> GetDocuments()
        {
            return documents.AsReadOnly();
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {           
            if (!documents.Contains(e.Name) && documents.Count < 3)
            {
                documents.Add(e.Name);               
            }

            if(documents.Count == 3)
            {
                OnDocumentsReady(new DocumentsArgs(e.Name, documents.Count));
                timer.Stop();
            }            
        }

        public void Start(string _pathTargetDirectory, double waitingInterval, Collection<string> _collections)
        {
            if (!Directory.Exists(_pathTargetDirectory))
            Directory.CreateDirectory(_pathTargetDirectory);

            foreach (string file in _collections)
                fileSystemWatcher.Filters.Add(file);

            fileSystemWatcher.Path = _pathTargetDirectory;
            fileSystemWatcher.EnableRaisingEvents = true;

            timer.Interval = waitingInterval;
            timer.Start();           
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            OnTimeOut(new DocumentsArgs(null, documents.Count));
        }

        public event EventHandler<DocumentsArgs> DocumentsReady;
        public event EventHandler<DocumentsArgs> TimeOut;
        
        internal void OnDocumentsReady(DocumentsArgs e)
        {
            DocumentsReady?.Invoke(this, e);
        }
        
        internal void OnTimeOut(DocumentsArgs e)
        {
            TimeOut?.Invoke(this, e);
        }

        private void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    if(fileSystemWatcher != null)
                    {
                        fileSystemWatcher.Created -= FileSystemWatcher_Created;
                        fileSystemWatcher.Dispose();
                    }
                    if(timer != null)
                    {
                        timer.Elapsed -= OnTimedEvent;
                        timer.Dispose();
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    
    public class DocumentsArgs : EventArgs
    {
        /// <summary>
        /// Имя загружаемого документа
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Количество файлов
        /// </summary>
        public int CountFiles { get; }

        public DocumentsArgs(string fileName, int countFiles)
        {          
            FileName = string.IsNullOrEmpty(fileName) ? string.Empty : fileName;            
            CountFiles = countFiles;
        }       
    }
}
