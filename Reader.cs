using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.ComponentModel;

namespace AGB_Cartridge_Reader
{
    class Reader
    {
        private const int maxRetries = 3;
        private const int maxPayloadSize = 4096;

        private enum XferType
        {
            REQ_ROM_SEEK,
            REQ_ROM_READ,
            REQ_ROM_WRITE,
            REQ_SRAM_SEEK,
            REQ_SRAM_READ,
            REQ_SRAM_WRITE,
            REQ_FLASH_ERASE,
            REQ_FLASH_SEEK,
            REQ_FLASH_READ,
            REQ_FLASH_WRITE,
            REQ_FLASH_BANK,
            REQ_EEPROM512_SEEK,
            REQ_EEPROM512_READ,
            REQ_EEPROM512_WRITE,
            REQ_EEPROM8K_SEEK,
            REQ_EEPROM8K_READ,
            REQ_EEPROM8K_WRITE,
            REQ_UART_SYNC,

            REPL_ROM_SEEK,
            REPL_ROM_READ,
            REPL_ROM_WRITE,
            REPL_SRAM_SEEK,
            REPL_SRAM_READ,
            REPL_SRAM_WRITE,
            REPL_FLASH_ERASE,
            REPL_FLASH_SEEK,
            REPL_FLASH_READ,
            REPL_FLASH_WRITE,
            REPL_FLASH_BANK,
            REPL_EEPROM512_SEEK,
            REPL_EEPROM512_READ,
            REPL_EEPROM512_WRITE,
            REPL_EEPROM8K_SEEK,
            REPL_EEPROM8K_READ,
            REPL_EEPROM8K_WRITE,
            REPL_UART_SYNC,

            REPL_ERR,

            UNKNOWN
        }

        static XferType ushortToXferType(ushort type)
        {
            switch (type)
            {
                case 0x40:
                    return XferType.REQ_ROM_SEEK;
                case 0x41:
                    return XferType.REQ_ROM_READ;
                case 0x42:
                    return XferType.REQ_ROM_WRITE;
                case 0x50:
                    return XferType.REQ_SRAM_SEEK;
                case 0x51:
                    return XferType.REQ_SRAM_READ;
                case 0x52:
                    return XferType.REQ_SRAM_WRITE;
                case 0x60:
                    return XferType.REQ_FLASH_ERASE;
                case 0x61:
                    return XferType.REQ_FLASH_SEEK;
                case 0x62:
                    return XferType.REQ_FLASH_READ;
                case 0x63:
                    return XferType.REQ_FLASH_WRITE;
                case 0x64:
                    return XferType.REQ_FLASH_BANK;
                case 0x70:
                    return XferType.REQ_EEPROM512_SEEK;
                case 0x71:
                    return XferType.REQ_EEPROM512_READ;
                case 0x72:
                    return XferType.REQ_EEPROM512_WRITE;
                case 0x80:
                    return XferType.REQ_EEPROM8K_SEEK;
                case 0x81:
                    return XferType.REQ_EEPROM8K_READ;
                case 0x82:
                    return XferType.REQ_EEPROM8K_WRITE;
                case 0xA0:
                    return XferType.REQ_UART_SYNC;

                case 0x140:
                    return XferType.REPL_ROM_SEEK;
                case 0x141:
                    return XferType.REPL_ROM_READ;
                case 0x142:
                    return XferType.REPL_ROM_WRITE;
                case 0x150:
                    return XferType.REPL_SRAM_SEEK;
                case 0x151:
                    return XferType.REPL_SRAM_READ;
                case 0x152:
                    return XferType.REPL_SRAM_WRITE;
                case 0x160:
                    return XferType.REPL_FLASH_ERASE;
                case 0x161:
                    return XferType.REPL_FLASH_SEEK;
                case 0x162:
                    return XferType.REPL_FLASH_READ;
                case 0x163:
                    return XferType.REPL_FLASH_WRITE;
                case 0x164:
                    return XferType.REPL_FLASH_BANK;
                case 0x170:
                    return XferType.REPL_EEPROM512_SEEK;
                case 0x171:
                    return XferType.REPL_EEPROM512_READ;
                case 0x172:
                    return XferType.REPL_EEPROM512_WRITE;
                case 0x180:
                    return XferType.REPL_EEPROM8K_SEEK;
                case 0x181:
                    return XferType.REPL_EEPROM8K_READ;
                case 0x182:
                    return XferType.REPL_EEPROM8K_WRITE;
                case 0x1A0:
                    return XferType.REPL_UART_SYNC;
                default:
                    return XferType.UNKNOWN;
                }
        }

        static ushort xferTypeToUshort(XferType type)
        {
            switch (type)
            {
                case XferType.REQ_ROM_SEEK:
                    return 0x40;
                case XferType.REQ_ROM_READ:
                    return 0x41;
                case XferType.REQ_ROM_WRITE:
                    return 0x42;
                case XferType.REQ_SRAM_SEEK:
                    return 0x50;
                case XferType.REQ_SRAM_READ:
                    return 0x51;
                case XferType.REQ_SRAM_WRITE:
                    return 0x52;
                case XferType.REQ_FLASH_ERASE:
                    return 0x60;
                case XferType.REQ_FLASH_SEEK:
                    return 0x61;
                case XferType.REQ_FLASH_READ:
                    return 0x62;
                case XferType.REQ_FLASH_WRITE:
                    return 0x63;
                case XferType.REQ_FLASH_BANK:
                    return 0x64;
                case XferType.REQ_EEPROM512_SEEK:
                    return 0x70;
                case XferType.REQ_EEPROM512_READ:
                    return 0x71;
                case XferType.REQ_EEPROM512_WRITE:
                    return 0x72;
                case XferType.REQ_EEPROM8K_SEEK:
                    return 0x80;
                case XferType.REQ_EEPROM8K_READ:
                    return 0x81;
                case XferType.REQ_EEPROM8K_WRITE:
                    return 0x82;
                case XferType.REQ_UART_SYNC:
                    return 0xA0;

                case XferType.REPL_ROM_SEEK:
                    return 0x140;
                case XferType.REPL_ROM_READ:
                    return 0x141;
                case XferType.REPL_ROM_WRITE:
                    return 0x142;
                case XferType.REPL_SRAM_SEEK:
                    return 0x150;
                case XferType.REPL_SRAM_READ:
                    return 0x151;
                case XferType.REPL_SRAM_WRITE:
                    return 0x152;
                case XferType.REPL_FLASH_ERASE:
                    return 0x160;
                case XferType.REPL_FLASH_SEEK:
                    return 0x161;
                case XferType.REPL_FLASH_READ:
                    return 0x162;
                case XferType.REPL_FLASH_WRITE:
                    return 0x163;
                case XferType.REPL_FLASH_BANK:
                    return 0x164;
                case XferType.REPL_EEPROM512_SEEK:
                    return 0x170;
                case XferType.REPL_EEPROM512_READ:
                    return 0x171;
                case XferType.REPL_EEPROM512_WRITE:
                    return 0x172;
                case XferType.REPL_EEPROM8K_SEEK:
                    return 0x180;
                case XferType.REPL_EEPROM8K_READ:
                    return 0x181;
                case XferType.REPL_EEPROM8K_WRITE:
                    return 0x182;
                case XferType.REPL_UART_SYNC:
                    return 0x1A0;
                default:
                case XferType.UNKNOWN:
                    throw new ArgumentException("Internal Error. Invalid Xfer Type requested");
            }
        }

        private class Xfer
        {
            public Xfer(XferType type, ushort id)
            {
                this.type = type;
                data = new byte[0];
            }

            public Xfer(XferType type, ushort id, byte data)
            {
                this.type = type;
                this.data = new byte[1];
                this.data[0] = data;
            }

            public Xfer(XferType type, ushort id, ushort data)
            {
                this.type = type;
                this.data = new byte[2];
                this.data[0] = (byte)data;
                this.data[1] = (byte)(data >> 8);
            }

            public Xfer(XferType type, ushort id, uint data)
            {
                this.type = type;
                this.data = new byte[4];
                this.data[0] = (byte)data;
                this.data[1] = (byte)(data >> 8);
                this.data[2] = (byte)(data >> 16);
                this.data[3] = (byte)(data >> 24);
            }

            public Xfer(XferType type, ushort id, byte[] data)
            {

                this.type = type;
                this.data = new byte[data.Length];
                Array.Copy(data, this.data, data.Length);
            }

            /*
            private ushort calcCRC()
            {
                ushort crc = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    ushort x = (ushort)((crc >> 8) ^ data[i]);
                    x ^= (ushort)(x >> 4);
                    crc = (ushort)((crc << 8) ^ (x << 12) ^ (x << 5) ^ x);
                }
                return crc;
            }
            */

            public static Xfer readXfer(SerialPort sp)
            {
                byte[] header = new byte[8];
                int index = 0;
                int bytesRead = 0;
                int bytesToRead = header.Length;
                do
                {
                    bytesRead = sp.Read(header, index, bytesToRead);
                    bytesToRead -= bytesRead;
                    index += bytesRead;
                } while (bytesToRead > 0);

                ushort magic = (ushort)(header[0] | (header[1] << 8));
                ushort type = (ushort)(header[2] | (header[3] << 8));
                ushort id = (ushort)(header[4] | (header[5] << 8));
                ushort len = (ushort)(header[6] | (header[7] << 8));

                if (magic != 0xDE44)
                    throw new Exception("ERROR: Invalid Packet Magic");

                byte[] data = new byte[len];
                bytesToRead = len;
                index = 0;
                do
                {
                    bytesRead = sp.Read(data, index, bytesToRead);
                    bytesToRead -= bytesRead;
                    index += bytesRead;
                } while (bytesToRead > 0);

                XferType xferType = ushortToXferType(type);
                if (xferType == XferType.UNKNOWN)
                    throw new Exception("ERROR: Illegal message type: " + type.ToString("X2"));
                Xfer obj = new Xfer(xferType, id, data);
                return obj;
            }

            public void show()
            {
                string debug = "";
                for (int j = 0; j < data.Length; j++)
                    debug += data[j].ToString("X2") + " ";
                System.Windows.Forms.MessageBox.Show(type.ToString() + "\n" + debug + "\n" + id.ToString());
            }

            public string getDataAsString()
            {
                return Encoding.ASCII.GetString(data);
            }

            public void sendXfer(SerialPort sp)
            {
                byte[] packet = new byte[8 + data.Length];
                packet[0] = 0x57;
                packet[1] = 0x80;
                ushort t = xferTypeToUshort(type);
                packet[2] = (byte)t;
                packet[3] = (byte)(t >> 8);
                packet[4] = (byte)id;
                packet[5] = (byte)(id >> 8);
                packet[6] = (byte)(data.Length);
                packet[7] = (byte)(data.Length >> 8);

                for (int i = 0; i < data.Length; i++)
                    packet[8 + i] = data[i];

                sp.Write(packet, 0, packet.Length);
            }

            public readonly XferType type;
            public readonly byte[] data;
            public readonly ushort id;
        }

        private enum TransferDirection
        {
            HOST_TO_GBA, GBA_TO_HOST
        }
        public static void Action(BackgroundWorker bw, SerialPort sp, DoWorkEventArgs e)
        {
            int retryCount = 0;
            int i;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            sp.Open();
            long oldMillis = 0;
            uint oldPosition = 0;
            WorkerArgs wa = e.Argument as WorkerArgs;
            if (wa == null)
                throw new ArgumentException("Invalid transceiver args");
            FileStream file = null;
            TransferDirection dir = TransferDirection.GBA_TO_HOST;
            Xfer reply = null;
            switch (wa.type)
            {
                case WorkerArgs.OperationType.DOWNLOAD_4M_ROM:
                case WorkerArgs.OperationType.DOWNLOAD_8M_ROM:
                case WorkerArgs.OperationType.DOWNLOAD_16M_ROM:
                case WorkerArgs.OperationType.DOWNLOAD_32M_ROM:
                case WorkerArgs.OperationType.DOWNLOAD_512_EEPROM:
                case WorkerArgs.OperationType.DOWNLOAD_8K_EEPROM:
                case WorkerArgs.OperationType.DOWNLOAD_SRAM_32K:
                case WorkerArgs.OperationType.DOWNLOAD_SRAM_64K:
                case WorkerArgs.OperationType.DOWNLOAD_FLASH_64K:
                case WorkerArgs.OperationType.DOWNLOAD_FLASH_128K:
                    file = new FileStream(wa.file, FileMode.Create, FileAccess.Write, FileShare.Read);
                    dir = TransferDirection.GBA_TO_HOST;
                    break;
                case WorkerArgs.OperationType.UPLOAD_512_EEPROM:
                case WorkerArgs.OperationType.UPLOAD_8K_EEPROM:
                case WorkerArgs.OperationType.UPLOAD_SRAM_32K:
                case WorkerArgs.OperationType.UPLOAD_SRAM_64K:
                case WorkerArgs.OperationType.UPLOAD_FLASH_64K:
                case WorkerArgs.OperationType.UPLOAD_FLASH_128K:
                    file = new FileStream(wa.file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    dir = TransferDirection.HOST_TO_GBA;
                    break;
            }

            uint bytesToTransfer = 0;
            const int KiB = 1024;
            const int MiB = 1024 * KiB;
            switch (wa.type)
            {
                case WorkerArgs.OperationType.DOWNLOAD_4M_ROM:
                    bytesToTransfer = 2 * MiB;
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_8M_ROM:
                    bytesToTransfer = 8 * MiB;
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_16M_ROM:
                    bytesToTransfer = 16 * MiB;
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_32M_ROM:
                    bytesToTransfer = 32 * MiB;
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_512_EEPROM:
                case WorkerArgs.OperationType.UPLOAD_512_EEPROM:
                    bytesToTransfer = 512;
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_8K_EEPROM:
                case WorkerArgs.OperationType.UPLOAD_8K_EEPROM:
                    bytesToTransfer = 8 * KiB;
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_SRAM_32K:
                case WorkerArgs.OperationType.UPLOAD_SRAM_32K:
                    bytesToTransfer = 32 * KiB;
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_SRAM_64K:
                case WorkerArgs.OperationType.UPLOAD_SRAM_64K:
                case WorkerArgs.OperationType.DOWNLOAD_FLASH_64K:
                case WorkerArgs.OperationType.UPLOAD_FLASH_64K:
                    bytesToTransfer = 64 * KiB;
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_FLASH_128K:
                case WorkerArgs.OperationType.UPLOAD_FLASH_128K:
                    bytesToTransfer = 128 * KiB;
                    break;
            }

            uint totalBytes = bytesToTransfer;
            uint transferBlockSize = 0;
            switch (wa.type)
            {
                case WorkerArgs.OperationType.DOWNLOAD_4M_ROM:
                case WorkerArgs.OperationType.DOWNLOAD_8M_ROM:
                case WorkerArgs.OperationType.DOWNLOAD_16M_ROM:
                case WorkerArgs.OperationType.DOWNLOAD_32M_ROM:
                case WorkerArgs.OperationType.DOWNLOAD_SRAM_32K:
                case WorkerArgs.OperationType.DOWNLOAD_SRAM_64K:
                case WorkerArgs.OperationType.DOWNLOAD_FLASH_64K:
                case WorkerArgs.OperationType.DOWNLOAD_FLASH_128K:
                case WorkerArgs.OperationType.UPLOAD_SRAM_32K:
                case WorkerArgs.OperationType.UPLOAD_SRAM_64K:
                case WorkerArgs.OperationType.UPLOAD_FLASH_64K:
                case WorkerArgs.OperationType.UPLOAD_FLASH_128K:
                    transferBlockSize = maxPayloadSize;
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_512_EEPROM:
                case WorkerArgs.OperationType.DOWNLOAD_8K_EEPROM:
                case WorkerArgs.OperationType.UPLOAD_512_EEPROM:
                case WorkerArgs.OperationType.UPLOAD_8K_EEPROM:
                    transferBlockSize = 8;
                    break;
            }

            if (wa.type == WorkerArgs.OperationType.UPLOAD_FLASH_128K ||
                wa.type == WorkerArgs.OperationType.UPLOAD_FLASH_64K)
            {
                // erase flash
                Xfer eraseXfer = new Xfer(XferType.REQ_FLASH_ERASE, 0);
                
                for (i = 0; i < maxRetries; i++)
                {
                    eraseXfer.sendXfer(sp);
                    reply = Xfer.readXfer(sp);
                    if (reply.type == XferType.REPL_FLASH_ERASE)
                        break;
                    retryCount++;
                }
                if (i == maxRetries)
                    throw new IOException("Too many failed retries for erasing flash: " + reply.getDataAsString());
            }

            uint position = 0;
            bool setAddr = true;
            bool setBank = false;
            while (bytesToTransfer > 0)
            {
                uint transferSize = bytesToTransfer < transferBlockSize ? bytesToTransfer : transferBlockSize;
                if (wa.type == WorkerArgs.OperationType.UPLOAD_FLASH_128K ||
                    wa.type == WorkerArgs.OperationType.DOWNLOAD_FLASH_128K)
                {
                    if (position < 0x10000 && position + transferSize > 0x10000)
                        transferSize = 0x10000 - position;
                    if ((position & 0xFFFF) == 0)
                        setBank = true;
                    else
                        setBank = false;
                }


                // seek remote
                if (setAddr)
                {
                    Xfer xfer = null;
                    switch (wa.type)
                    {
                        case WorkerArgs.OperationType.DOWNLOAD_4M_ROM:
                        case WorkerArgs.OperationType.DOWNLOAD_8M_ROM:
                        case WorkerArgs.OperationType.DOWNLOAD_16M_ROM:
                        case WorkerArgs.OperationType.DOWNLOAD_32M_ROM:
                            xfer = new Xfer(XferType.REQ_ROM_SEEK, 0, (uint)position);
                            break;
                        case WorkerArgs.OperationType.DOWNLOAD_SRAM_32K:
                        case WorkerArgs.OperationType.DOWNLOAD_SRAM_64K:
                        case WorkerArgs.OperationType.UPLOAD_SRAM_32K:
                        case WorkerArgs.OperationType.UPLOAD_SRAM_64K:
                            xfer = new Xfer(XferType.REQ_SRAM_SEEK, 0, (ushort)position);
                            break;
                        case WorkerArgs.OperationType.DOWNLOAD_FLASH_64K:
                        case WorkerArgs.OperationType.DOWNLOAD_FLASH_128K:
                        case WorkerArgs.OperationType.UPLOAD_FLASH_64K:
                        case WorkerArgs.OperationType.UPLOAD_FLASH_128K:
                            xfer = new Xfer(XferType.REQ_FLASH_SEEK, 0, (ushort)position);
                            break;
                        case WorkerArgs.OperationType.DOWNLOAD_512_EEPROM:
                        case WorkerArgs.OperationType.UPLOAD_512_EEPROM:
                            xfer = new Xfer(XferType.REQ_EEPROM512_SEEK, 0, (ushort)position);
                            break;
                        case WorkerArgs.OperationType.DOWNLOAD_8K_EEPROM:
                        case WorkerArgs.OperationType.UPLOAD_8K_EEPROM:
                            xfer = new Xfer(XferType.REQ_EEPROM8K_SEEK, 0, (ushort)position);
                            break;
                    }

                    for (i = 0; i < maxRetries; i++)
                    {
                        xfer.sendXfer(sp);
                        reply = Xfer.readXfer(sp);
                        if (reply.type != XferType.REPL_ERR)
                            break;
                        retryCount++;
                    }
                    if (i == maxRetries)
                        throw new IOException("Too many failed retries for seeking ROM: " + reply.getDataAsString());
                }

                if (setBank)
                {
                    Xfer bankXfer = new Xfer(XferType.REQ_FLASH_BANK, 0, (byte)(position >> 16));
                    for (i = 0; i < maxRetries; i++)
                    {
                        bankXfer.sendXfer(sp);
                        reply = Xfer.readXfer(sp);
                        if (reply.type == XferType.REPL_FLASH_BANK)
                            break;
                        retryCount++;
                    }
                    if (i == maxRetries)
                        throw new IOException("Too many failed retries for bank switching Flash: " + reply.getDataAsString());
                }

                // transfer data
                Xfer dataXfer;
                byte[] dataBuf = new byte[transferSize];

                if (dir == TransferDirection.HOST_TO_GBA)
                    file.Read(dataBuf, 0, dataBuf.Length);

                switch (wa.type)
                {
                    case WorkerArgs.OperationType.DOWNLOAD_4M_ROM:
                    case WorkerArgs.OperationType.DOWNLOAD_8M_ROM:
                    case WorkerArgs.OperationType.DOWNLOAD_16M_ROM:
                    case WorkerArgs.OperationType.DOWNLOAD_32M_ROM:
                        dataXfer = new Xfer(XferType.REQ_ROM_READ, 0, (ushort)transferSize);
                        break;
                    case WorkerArgs.OperationType.DOWNLOAD_SRAM_32K:
                    case WorkerArgs.OperationType.DOWNLOAD_SRAM_64K:
                        dataXfer = new Xfer(XferType.REQ_SRAM_READ, 0, (ushort)transferSize);
                        break;
                    case WorkerArgs.OperationType.DOWNLOAD_FLASH_64K:
                    case WorkerArgs.OperationType.DOWNLOAD_FLASH_128K:
                        dataXfer = new Xfer(XferType.REQ_FLASH_READ, 0, (ushort)transferSize);
                        break;
                    case WorkerArgs.OperationType.DOWNLOAD_512_EEPROM:
                        dataXfer = new Xfer(XferType.REQ_EEPROM512_READ, 0, (ushort)transferSize);
                        break;
                    case WorkerArgs.OperationType.DOWNLOAD_8K_EEPROM:
                        dataXfer = new Xfer(XferType.REQ_EEPROM8K_READ, 0, (ushort)transferSize);
                        break;
                    case WorkerArgs.OperationType.UPLOAD_SRAM_32K:
                    case WorkerArgs.OperationType.UPLOAD_SRAM_64K:
                        dataXfer = new Xfer(XferType.REQ_SRAM_WRITE, 0, dataBuf);
                        break;
                    case WorkerArgs.OperationType.UPLOAD_FLASH_64K:
                    case WorkerArgs.OperationType.UPLOAD_FLASH_128K:
                        dataXfer = new Xfer(XferType.REQ_FLASH_WRITE, 0, dataBuf);
                        break;
                    case WorkerArgs.OperationType.UPLOAD_512_EEPROM:
                        dataXfer = new Xfer(XferType.REQ_EEPROM512_WRITE, 0, dataBuf);
                        break;
                    case WorkerArgs.OperationType.UPLOAD_8K_EEPROM:
                        dataXfer = new Xfer(XferType.REQ_EEPROM8K_WRITE, 0, dataBuf);
                        break;
                    default:
                        throw new Exception("internal error");
                }
                
                for (i = 0; i < maxRetries; i++)
                {
                    dataXfer.sendXfer(sp);
                    reply = Xfer.readXfer(sp);
                    if (reply.type != XferType.REPL_ERR)
                        break;
                    retryCount++;
                }
                if (i == maxRetries)
                    throw new IOException("Too many failed data requests: " + reply.getDataAsString());

                if (dir == TransferDirection.GBA_TO_HOST)
                    file.Write(reply.data, 0, reply.data.Length);

                position += transferSize;
                bytesToTransfer -= transferSize;

                if (sw.ElapsedMilliseconds > oldMillis + 250)
                {
                    double progress = position * 100.0 / totalBytes;
                    progress = Math.Round(progress, MidpointRounding.AwayFromZero);

                    bw.ReportProgress((int)progress,
                        new ProgressState(
                            retryCount,
                            (int)(position - oldPosition) * 4,
                            (int)position));
                    oldPosition = position;
                    oldMillis = sw.ElapsedMilliseconds;
                }

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            sp.Close();
            sw.Stop();
            file.Close();
        }
    }
}
