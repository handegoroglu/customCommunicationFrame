using System;
using customCommunicationFrame.Models;

namespace ChecksumCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            
            byte[] byteArray = { 0x01, 0x02, 0x03, 0x04 };
            communicationFrame.infoSection info=new communicationFrame.infoSection();
            info.highDataPriority = true;
            info.readOrSend = true;
            info.waitForAnswer = true;
            info.info = 1;
            communicationFrameEncoder frameEncoder = new communicationFrameEncoder(true,info,byteArray,12);

            Console.WriteLine("handeeee");
        }

       
    }
}