using DDW.Document;
using System;
using System.IO;

namespace DDW.Client
{
    class Program
    {
        const string folderName = "TargetDirectory";
        
        static bool stopReceiveDocuments = false;

        static void Main(string[] args)
        {

            string pathTargetDirectory = Path.Combine(Environment.CurrentDirectory, folderName);

            //Время приемки документов в милисекундах
            double timeReceiver = 20000;
            TimeSpan interval = TimeSpan.FromMilliseconds(timeReceiver);

            //DocumentsReceiver documentsReceiver = new DocumentsReceiver();
            using var documentsReceiver = new DocumentsReceiver();
            
            documentsReceiver.DocumentsReady += DocumentsReceiver_DocumentsReady;
            documentsReceiver.TimeOut += DocumentsReceiver_TimeOut;
            documentsReceiver.Start(pathTargetDirectory, timeReceiver);
                      

            Console.WriteLine($"Запущена процедура приема документов... {interval.Seconds} сек.");          

            while(!stopReceiveDocuments)
            {
                
            }          

            foreach (var doc in documentsReceiver.GetDocuments())
                Console.WriteLine($"\t{doc}");
          
            Console.ReadLine();
            
        }

        private static void DocumentsReceiver_TimeOut(object sender, DocumentsArgs e)
        {           
            Console.WriteLine($"Время для приема документов вышло");
            stopReceiveDocuments = true;
        }

        private static void DocumentsReceiver_DocumentsReady(object sender, DocumentsArgs e)
        {          
            Console.WriteLine($"\nВсего загружено документов {e.CountFiles}\n");
            stopReceiveDocuments = true;
        }
    }
}
