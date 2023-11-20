using System;
using static customCommunicationFrame.Models.communicationFrame;
using System.Net.Sockets;

namespace customCommunicationFrame.Models
{
    /// <summary>
    /// Represents a decoder for a custom communication frame.
    /// </summary>
    /// <remarks>
    /// This class is responsible for decoding the components of a communication frame.
    /// </remarks>
    public class communicationFrameDecoder : communicationFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="communicationFrameDecoder"/> class.
        /// </summary>
        /// <param name="packet">The byte array representing the communication packet.</param>
        /// <param name="isSecure">Indicates whether the communication frame is secure.</param>
        public communicationFrameDecoder(byte[] packet, bool isSecure)
        {
            // Initialize the index for tracking the current position in the packet array
            int packetIndex = 0;

            #region Start Byte
            // Retrieve the start byte from the packet and increment the packet index
            secure.startByte = packet[packetIndex++];
            #endregion

            #region Total Data Len
            // Retrieve the total data length from the packet using BitConverter
            secure.totalDataLen = BitConverter.ToUInt16(GetBytes(packet, packetIndex, sizeof(UInt16)), 0);
            // Increment the packet index by the size of UInt16 (2 bytes)
            packetIndex += sizeof(UInt16);
            #endregion

            // Check if the communication frame is secure
            if (isSecure)
            {
                #region Device ID
                // Retrieve the device ID from the packet and increment the packet index
                secure.deviceID = packet[packetIndex++];
                #endregion
            }

            #region Packet Number
            // Retrieve the packet number from the packet using BitConverter
            packetNumber = BitConverter.ToUInt32(GetBytes(packet, packetIndex, sizeof(UInt32)), 0);
            // Increment the packet index by the size of UInt32 (4 bytes)
            packetIndex += sizeof(UInt32);
            #endregion

            #region Info Byte
            // Retrieve the info byte from the packet
            byte infoByte_ = packet[packetIndex++];

            // Extract individual bits from the info byte and assign them to the infoSection struct
            infoByte = new infoSection
            {
                info = (byte)(infoByte_ & 0x1f),
                highDataPriority = GetBit(infoByte_, 5),
                waitForAnswer = GetBit(infoByte_, 6),
                readOrSend = GetBit(infoByte_, 7)
            };
            //
            #endregion

            // Check if the communication frame is secure
            if (isSecure)
            {
                #region Content Size
                // Retrieve the content size from the packet using BitConverter
                secure.contentSize = BitConverter.ToUInt16(GetBytes(packet, packetIndex, sizeof(UInt16)), 0);
                // Increment the packet index by the size of UInt16 (2 bytes)
                packetIndex += sizeof(UInt16);
                #endregion

                #region Header Checksum
                // Retrieve the header checksum from the packet
                secure.headerChecksum = packet[packetIndex++];
                #endregion
            }
            else
            {
                // If the frame is not secure, set content size based on the length of the packet array
                secure.contentSize = Convert.ToUInt16(packet.Length - 5);
            }

            #region Content
            // Create a content array and copy content from the packet at the current index
            content = new byte[secure.contentSize];
            Array.Copy(packet, packetIndex, content, 0, secure.contentSize);
            // Increment the packet index by the size of the content array
            packetIndex += secure.contentSize;
            #endregion

            // Check if the communication frame is secure
            if (isSecure)
            {
                #region CRC
                // Retrieve the CRC from the packet using BitConverter
                secure.crc = BitConverter.ToUInt32(GetBytes(packet, packetIndex, sizeof(UInt32)), 0);
                // Increment the packet index by the size of UInt32 (4 bytes)
                packetIndex += sizeof(UInt32);
                #endregion

                #region Stop Byte
                // Retrieve the stop byte from the packet and increment the packet index
                secure.stopByte = packet[packetIndex++];
                #endregion
            }

        }

        /// <summary>
        /// Gets the specified number of bytes from the given array starting at the specified index.
        /// </summary>
        /// <param name="array">The source array.</param>
        /// <param name="index">The starting index in the source array.</param>
        /// <param name="length">The number of bytes to retrieve.</param>
        /// <returns>A byte array containing the specified bytes.</returns>
        private byte[] GetBytes(byte[] array, int index, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(array, index, result, 0, length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(result);
            }
            return result;
        }

        /// <summary>
        /// Gets the value of the specified bit at the given bit index in a byte.
        /// </summary>
        /// <param name="value">The byte containing the bit.</param>
        /// <param name="bitIndex">The index of the bit to retrieve (0 to 7).</param>
        /// <returns>True if the bit is set, false otherwise.</returns>
        private bool GetBit(byte value, int bitIndex)
        {
            return (value & (1 << bitIndex)) != 0;
        }
    }
}


