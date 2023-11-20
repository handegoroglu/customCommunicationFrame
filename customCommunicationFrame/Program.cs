using System;
using customCommunicationFrame.Models;

namespace ChecksumCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Sample content array
            byte[] content = { 0x01, 0x02, 0x03, 0x04 };

            // Create an instance of the infoSection struct
            communicationFrame.infoSection info = new communicationFrame.infoSection();

            // Set values for the infoSection struct
            info.highDataPriority = true;
            info.readOrSend = true;
            info.waitForAnswer = false;
            info.info = 1;

            // Create an instance of communicationFrameEncoder to encode the communication frame
            communicationFrameEncoder frameEncoder = new communicationFrameEncoder(true, info, content, 12, 100);

            // Create an instance of communicationFrameDecoder to decode the encoded frame
            communicationFrameDecoder frameDecoder = new communicationFrameDecoder(frameEncoder.packet, true);

        }

    }
}