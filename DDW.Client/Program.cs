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
            DocumentsReceiver documentsReceiver = new DocumentsReceiver();
            documentsReceiver.DocumentsReady += DocumentsReceiver_DocumentsReady;
            documentsReceiver.TimeOut += DocumentsReceiver_TimeOut;

            documentsReceiver.Start(pathDirectory, 30000);

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
