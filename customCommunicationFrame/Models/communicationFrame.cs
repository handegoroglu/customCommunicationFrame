using System;
namespace customCommunicationFrame.Models
{
    // <summary>
    /// Represents a communication frame structure with encoding and decoding functionality.
    /// </summary>
    public class communicationFrame
    {
        /// <summary>
        /// Represents the information section of the communication frame.
        /// </summary>
        public struct infoSection
        {
            /// <summary>
            /// Gets or sets a value indicating whether the data has high priority.
            /// </summary>
            public bool highDataPriority { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the frame is waiting for an answer.
            /// </summary>
            public bool waitForAnswer { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the operation is a read or send.
            /// </summary>
            public bool readOrSend { get; set; }

            /// <summary>
            /// Gets or sets the information byte.
            /// </summary>
            public byte info { get; set; }
        }

        /// <summary>
        /// Represents the secure section of the communication frame.
        /// </summary>
        public class Secure
        {
            /// <summary>
            /// Gets or sets the start byte.
            /// </summary>
            public byte startByte { get; set; }

            /// <summary>
            /// Gets or sets the total data length.
            /// </summary>
            public UInt16 totalDataLen { get; set; }

            /// <summary>
            /// Gets or sets the device ID.
            /// </summary>
            public byte deviceID { get; set; }

            /// <summary>
            /// Gets or sets the content size.
            /// </summary>
            public UInt16 contentSize { get; set; }

            /// <summary>
            /// Gets or sets the header checksum.
            /// </summary>
            public byte headerChecksum { get; set; }

            /// <summary>
            /// Gets or sets the CRC value.
            /// </summary>
            public UInt32 crc { get; set; }

            /// <summary>
            /// Gets or sets the stop byte.
            /// </summary>
            public byte stopByte { get; set; }
        }

        /// <summary>
        /// Gets or sets the secure section of the communication frame.
        /// </summary>
        public Secure secure { get; set; } = new Secure();

        /// <summary>
        /// Gets or sets the packet number.
        /// </summary>
        public UInt32 packetNumber { get; set; }

        /// <summary>
        /// Gets or sets the information section of the communication frame.
        /// </summary>
        public infoSection infoByte { get; set; } = new infoSection();

        /// <summary>
        /// Gets or sets the content of the communication frame.
        /// </summary>
        public byte[] content { get; set; }

        /// <summary>
        /// Sets or clears a bit at the specified index in a byte.
        /// </summary>
        /// <param name="data">The byte to modify.</param>
        /// <param name="index">The index of the bit to set or clear (0 to 7).</param>
        /// <param name="value">True to set the bit, false to clear it.</param>
        /// <returns>The modified byte.</returns>
        public byte SetBit(byte data, int index, bool value)
        {
            if (index < 0 || index > 7)
            {
                throw new ArgumentOutOfRangeException("Geçersiz bit indeksi. İndeks 0 ile 7 arasında olmalıdır.");
            }

            if (value)
            {
                // Belirli biti 1 yapmak için ilgili biti ayarlayın.
                data |= (byte)(1 << index);
            }
            else
            {
                // Belirli biti 0 yapmak için ilgili biti temizleyin.
                data &= (byte)~(1 << index);
            }

            return data;
        }
        /// <summary>
        /// Calculates the checksum of a byte array up to a specified length.
        /// </summary>
        /// <param name="byteArray">The byte array to calculate the checksum for.</param>
        /// <param name="len">The length of the byte array to consider.</param>
        /// <returns>The calculated checksum.</returns>
        public byte CalculateChecksum(byte[] byteArray, int len)
        {
            if (byteArray == null)
            {
                return 0;
            }

            uint checksum = 0;

            for (int i = 0; i < len; i++)
            {
                checksum += byteArray[i];
            }

            checksum = ((checksum ^ 255) + 1);

            // Little-endian mimariye göre kontrol etmek için BitConverter sınıfını kullanabilirsiniz.
            // BitConverter.IsLittleEndian özelliği, makinenin little-endian olup olmadığını kontrol eder.
            if (BitConverter.IsLittleEndian)
            {
                return (byte)(checksum & 0xFF);
            }
            else
            {
                return (byte)(checksum >> 24);
            }
        }
        /// <summary>
        /// Calculates the CRC32 checksum of a byte array up to a specified length.
        /// </summary>
        /// <param name="byteArray">The byte array to calculate the CRC32 for.</param>
        /// <param name="len">The length of the byte array to consider.</param>
        /// <returns>The calculated CRC32 checksum.</returns>
        public uint CalculateCRC32(byte[] byteArray, int len)
        {
            uint crc = 0xFFFFFFFF; // Başlangıçta CRC değeri 0xFFFFFFFF olarak ayarlanır.

            for (int i = 0; i < len; i++)
            {
                byte value = byteArray[i];

                for (int j = 0; j < 8; j++)
                {
                    uint b = (value ^ crc) & 1; // Verinin en sağdaki biti ile CRC'nin en sağdaki biti XOR işlemine tabi tutulur.
                    crc >>= 1; // CRC değeri bir bit sağa kaydırılır.
                    if (b != 0)
                    {
                        crc ^= 0xEDB88320; // XOR sonucuna göre belirli bir sabit değerle CRC değeri XOR işlemine tabi tutulur.
                    }
                    value >>= 1; // Veri bir bit sağa kaydırılır.
                }
            }

            return ~crc; // Sonuç, CRC değerinin tüm bitlerinin tersi olarak döndürülür.
        }
        /// <summary>
        /// Sets the bits in the information byte based on specified flags.
        /// </summary>
        /// <param name="isHighPriority">True if the data has high priority, false otherwise.</param>
        /// <param name="isWaitForAnswer">True if the frame is waiting for an answer, false otherwise.</param>
        /// <param name="readOrSend">True if the operation is a read, false if it is a send.</param>
        /// <param name="info">The original information byte.</param>
        /// <returns>The modified information byte.</returns>
        public byte SetInfoByte(bool isHighPriority, bool isWaitForAnswer, bool readOrSend, byte info)
        {
            // Ensure that only the last 5 bits of 'info' are considered
            info &= 0x1F;

            // Set the individual bits in the info byte
            byte infoByte = info;
            infoByte |= (byte)((isHighPriority ? 1 : 0) << 5);
            infoByte |= (byte)((isWaitForAnswer ? 1 : 0) << 6);
            infoByte |= (byte)((readOrSend ? 1 : 0) << 7);

            return infoByte;
        }

    }
}
