using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace DDW.Document
{
    public sealed  class DocumentsReceiver : IDisposable
    {
        FileSystemWatcher fileSystemWatcher;
        public List<string> documents { get; private set; }
       
        Timer timer;

        public DocumentsReceiver()
        {
            documents = new List<string>();
            
            SetTimer();

            fileSystemWatcher = new FileSystemWatcher
            {               
                NotifyFilter = NotifyFilters.FileName,
                Filters = { "Паспорт.jpg", "Заявление.txt", "Фото.jpg" },
                EnableRaisingEvents = false,
                IncludeSubdirectories = false              
            };

            fileSystemWatcher.Created += FileSystemWatcher_Created;           
        }

        private void SetTimer()
        {          
            timer = new Timer();          
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

        public void Start(string _pathTargetDirectory, double waitingInterval)
        {
            if (!Directory.Exists(_pathTargetDirectory))
            Directory.CreateDirectory(_pathTargetDirectory);

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

        public void Dispose()
        {
                           
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
