using DDW.Document;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDW.Client
{
    class Program
    {
        const string pathDirectory = @"C:\Users\User\Desktop\TargetDirectory";
        
        static bool stopReceiveDocuments = false;

        static void Main(string[] args)
        {
            //Время приемки документов в милисекундах
            double timeReceiver = 20000;
            TimeSpan interval = TimeSpan.FromMilliseconds(timeReceiver);

            DocumentsReceiver documentsReceiver = new DocumentsReceiver();
            documentsReceiver.DocumentsReady += DocumentsReceiver_DocumentsReady;
            documentsReceiver.TimeOut += DocumentsReceiver_TimeOut;

            Console.WriteLine($"Запущена процедура приема документов... {interval.Seconds} сек.");

            documentsReceiver.Start(pathDirectory, timeReceiver);

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
