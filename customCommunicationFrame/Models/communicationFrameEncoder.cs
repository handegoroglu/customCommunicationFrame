using System;
using static customCommunicationFrame.Models.communicationFrame;
using System.Net.Sockets;

namespace customCommunicationFrame.Models
{
    /// <summary>
    /// Represents an encoder for a custom communication frame.
    /// </summary>
    public class communicationFrameEncoder : communicationFrame
    {
        /// <summary>
        /// Gets or sets the packet number.
        /// </summary>
        private UInt32 packetNumber_ { get; set; }

        /// <summary>
        /// Gets or sets the byte array representing the communication packet.
        /// </summary>
        public byte[] packet { get; set; }

        /// <summary>
        /// Initializes a new instance of the communicationFrameEncoder class.
        /// </summary>
        /// <param name="isSecure">Indicates whether the communication frame is secure.</param>
        /// <param name="info">The information section of the communication frame.</param>
        /// <param name="content">The content of the communication frame.</param>
        /// <param name="deviceID">The device ID.</param>
        /// <param name="packetNumber">The optional packet number. If not provided, it will be auto-incremented.</param>
        public communicationFrameEncoder(bool isSecure, infoSection info, byte[] content,byte deviceID,UInt32 ?packetNumber=null)
		{

            // Create a byte array to hold the communication packet size based on whether it's secure or not
            packet = new byte[(isSecure ? 17 : 5) + content.Length];

            // Initialize the index for tracking the current position in the packet array
            int packetIndex = 0;

            // Check if the communication frame is secure
            if (isSecure == true)
            {
                #region Start Byte
                // Set the start byte in the packet and increment the packet index
                secure.startByte = packet[packetIndex++] = 0xFF;
                #endregion

                #region Total Data Len
                // Calculate and set the total data length in the secure section
                secure.totalDataLen = Convert.ToUInt16(packet.Length);
                // Get the bytes representing the total data length
                var totalDataLenBytes = BitConverter.GetBytes(secure.totalDataLen);
                // Reverse the byte order if the system is little-endian
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(totalDataLenBytes);
                }
                // Copy the total data length bytes into the packet at the current index
                Array.Copy(totalDataLenBytes, 0, packet, packetIndex, totalDataLenBytes.Length);
                // Increment the packet index by the length of the total data length bytes
                packetIndex += totalDataLenBytes.Length;
                #endregion

                #region Device ID
                // Set the device ID in the packet and increment the packet index
                secure.deviceID = packet[packetIndex++] = deviceID;
                #endregion
            }

            #region Packet Number
            // Convert packetNumber to UInt32, assign to packetNumber if null, and get the bytes
            packetNumber = Convert.ToUInt32((packetNumber == null ? packetNumber_ : packetNumber));
            var packetNumberBytes = BitConverter.GetBytes(Convert.ToUInt32(packetNumber));
            // Reverse the byte order if the system is little-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(packetNumberBytes);
            }
            // Copy the packet number bytes into the packet at the current index
            Array.Copy(packetNumberBytes, 0, packet, packetIndex, packetNumberBytes.Length);
            // Increment the packet index by the length of the packet number bytes
            packetIndex += packetNumberBytes.Length;
            // Set the packetNumber property based on the provided value or auto-increment if null
            if (packetNumber == null)
            {
                this.packetNumber = packetNumber_++;
            }
            else
            {
                this.packetNumber = (UInt32)packetNumber;
            }
            #endregion

            #region Info Byte
            // Set the info byte based on the values of the infoSection struct
            byte infoByte = SetInfoByte(info.highDataPriority, info.waitForAnswer, info.readOrSend, info.info);
            // Set the info byte in the packet and increment the packet index
            packet[packetIndex++] = infoByte;
            #endregion

            // Check if the communication frame is secure
            if (isSecure == true)
            {
                #region Content Size
                // Calculate and set the content size in the secure section
                secure.contentSize = Convert.ToUInt16(content.Length);
                // Get the bytes representing the content size
                var contentSizeBytes = BitConverter.GetBytes(secure.contentSize);
                // Reverse the byte order if the system is little-endian
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(contentSizeBytes);
                }
                // Copy the content size bytes into the packet at the current index
                Array.Copy(contentSizeBytes, 0, packet, packetIndex, contentSizeBytes.Length);
                // Increment the packet index by the length of the content size bytes
                packetIndex += contentSizeBytes.Length;
                #endregion

                #region Header Checksum
                // Calculate and set the header checksum in the secure section
                secure.headerChecksum = packet[packetIndex++] = CalculateChecksum(packet, 11);
                #endregion
            }
            else
            {
                // If the frame is not secure, set content size based on the length of the content array
                secure.contentSize = Convert.ToUInt16(packet.Length - 5);
            }

            #region Content
            // Copy the content array into the packet at the current index
            this.content = new byte[content.Length];
            Array.Copy(this.content, content, content.Length);
            Array.Copy(this.content, 0, packet, packetIndex, content.Length);
            // Increment the packet index by the length of the content array
            packetIndex += this.content.Length;
            #endregion

            // Check if the communication frame is secure
            if (isSecure == true)
            {
                #region CRC
                // Calculate and set the CRC in the secure section
                secure.crc = CalculateCRC32(packet, packet.Length - 5);
                // Get the bytes representing the CRC
                var crcBytes = BitConverter.GetBytes(Convert.ToUInt32(secure.crc));
                // Reverse the byte order if the system is little-endian
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(crcBytes);
                }
                // Copy the CRC bytes into the packet at the current index
                Array.Copy(crcBytes, 0, packet, packetIndex, crcBytes.Length);
                // Increment the packet index by the length of the CRC bytes
                packetIndex += crcBytes.Length;
                #endregion

                #region Stop Byte
                // Set the stop byte in the packet and increment the packet index
                secure.stopByte = packet[packetIndex++] = 0xFE;
                #endregion
            }

        }

    }
}

