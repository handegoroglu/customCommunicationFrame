using System;
namespace customCommunicationFrame.Models
{
	public class communicationFrameEncoder:communicationFrame
	{
		private UInt32 packetNumber_ { get; set; }
        public byte[] packet { get; set; }
        public communicationFrameEncoder(bool isSecure, infoSection info, byte[] content,byte deviceID,UInt32 ?packetNumber=null)
		{
			
		    packet = new byte[(isSecure?17:5)+content.Length];

			int packetIndex = 0;
            
            if (isSecure==true)
            {
                #region Start Byte
                packet[packetIndex++] = 0xFF;
                #endregion

                #region Total Data Len
                secure.totalDataLen = Convert.ToUInt16(packet.Length);
                var totalDataLenBytes = BitConverter.GetBytes(secure.totalDataLen);
                if(BitConverter.IsLittleEndian)
                {
                    Array.Reverse(totalDataLenBytes);
                }
                Array.Copy(totalDataLenBytes,0,packet,packetIndex,totalDataLenBytes.Length);
                packetIndex += totalDataLenBytes.Length;
                #endregion

                #region Device ID
                packet[packetIndex++] = deviceID;
                #endregion
            }

            #region Packet Number
            packetNumber = Convert.ToUInt32((packetNumber==null?packetNumber_:packetNumber));
            var packetNumberBytes = BitConverter.GetBytes(Convert.ToUInt32(packetNumber));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(packetNumberBytes);
            }
            Array.Copy(packetNumberBytes, 0, packet, packetIndex, packetNumberBytes.Length);
            packetIndex += packetNumberBytes.Length;
            if (packetNumber == null)
            {
                packetNumber_++;
            }
            #endregion

            #region Info Byte
            byte infoByte = info.info;
            infoByte = SetBit(infoByte, 0, info.highDataPriority);
            infoByte = SetBit(infoByte, 1, info.waitForAnswer);
            infoByte = SetBit(infoByte, 2, info.readOrSend);
            packet[packetIndex++] = infoByte;
            #endregion

            
            if (isSecure == true)
            {
                #region Content Size
                secure.contentSize = Convert.ToUInt16(content.Length);
                var contentSizeBytes = BitConverter.GetBytes(secure.contentSize);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(contentSizeBytes);
                }
                Array.Copy(contentSizeBytes, 0, packet, packetIndex, contentSizeBytes.Length);
                packetIndex += contentSizeBytes.Length;
                #endregion

                #region Header Checksum
                packet[packetIndex++] = CalculateChecksum(packet,11);
                #endregion

            }

            #region Content
            Array.Copy(content,0,packet, packetIndex, content.Length);
            packetIndex += content.Length;
            #endregion

            if (isSecure == true)
            {
                #region CRC
                UInt32 crc = CalculateCRC32(packet,packet.Length-5);
                var crcBytes = BitConverter.GetBytes(Convert.ToUInt32(crc));
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(crcBytes);
                }
                Array.Copy(crcBytes, 0, packet, packetIndex, crcBytes.Length);
                packetIndex += crcBytes.Length;
                #endregion

                #region Stop Byte
                packet[packetIndex++] = 0xFE;
                #endregion

            }


        }

    }
}

