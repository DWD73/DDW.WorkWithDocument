using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DDW.Document
{
    public class DocumentsReceiver
    {
        FileSystemWatcher fileSystemWatcher;
        List<string> documents;
        //public IReadOnlyList<string> documentsRead { get; }


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
            //DocumentsArgs args = new DocumentsArgs(e.Name);

            if (!documents.Contains(e.Name) && documents.Count < 3)
            {
                documents.Add(e.Name);               
            }

            if(documents.Count == 3)
            {
                OnDocumentsReady(new DocumentsArgs(e.Name, documents.Count));
            }

            
        }

        public void Start(string path, double waitingInterval)
        {
            fileSystemWatcher.Path = path;
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

        protected virtual void OnDocumentsReady(DocumentsArgs e)
        {
            DocumentsReady?.Invoke(this, e);
        }

        protected virtual void OnTimeOut(DocumentsArgs e)
        {
            TimeOut?.Invoke(this, e);
        }
    }

    
    public class DocumentsArgs : EventArgs
    {
        /// <summary>
        /// Имя загружаемого документа
        /// </summary>
        public string FileName { get; }

        public int CountFiles { get; }


        public DocumentsArgs(string fileName, int countFiles)
        {
            FileName = string.IsNullOrEmpty(fileName) ? string.Empty : fileName;
            CountFiles = countFiles;
        }

        
    }
}
