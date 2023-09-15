using System;
namespace customCommunicationFrame.Models
{
	public class communicationFrame
	{
        public struct infoSection
        {
            public bool highDataPriority { get; set; }
            public bool waitForAnswer { get; set; }
            public bool readOrSend { get; set; }
            public byte info { get; set; }
        }
        public class Secure
        {
            public byte startByte { get; set; }
            public UInt16 totalDataLen { get; set; }
            public byte deviceID { get; set; }
            public UInt16 contentSize { get; set; }
            public byte headerChecksum { get; set; }
            public UInt32 crc { get; set; }
            public byte stopByte { get; set; }
        }
        public Secure secure { get; set; } = new Secure();

        public UInt32 packetNumber { get; set; }
        public infoSection infoByte { get; set; }
        public byte[] content { get; set; }

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
        public uint CalculateCRC32(byte[] byteArray,int len)
        {
            uint crc = 0xFFFFFFFF; // Başlangıçta CRC değeri 0xFFFFFFFF olarak ayarlanır.

            for(int i=0; i <len; i++)
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
    }
}

