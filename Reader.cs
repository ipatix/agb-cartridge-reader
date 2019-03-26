using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.ComponentModel;
using System.Diagnostics;

namespace AGB_Cartridge_Reader
{
    class Reader
    {
        private const int maxRetries = 3;
        private const int maxPayloadSize = 4096;



        public static void Action(BackgroundWorker bw, SerialPort sp, DoWorkEventArgs e)
        {
            int i;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sp.Open();
            Transfer t = new Transfer(bw, sw, sp, e);

            // check if device is there
            for (i = 0; i < maxRetries; i++)
            {
                try
                {
                    new Xfer(XferType.REQ_UART_SYNC, 0).sendXfer(sp);
                    Xfer r = Xfer.readXfer(sp);
                    if (r.type == XferType.REPL_UART_SYNC)
                        break;
                }
                catch (TimeoutException) { }
            }
            if (i == maxRetries)
                throw new IOException("Cart Reader didn't respond");

            WorkerArgs wa = e.Argument as WorkerArgs;
            if (wa == null)
                throw new ArgumentException("Invalid transceiver args");

            Xfer rx;
            uint size;
            switch (wa.type)
            {
                case WorkerArgs.OperationType.DOWNLOAD_AUTO_ROM:
                    new Xfer(XferType.REQ_ROM_SIZE, 0).sendXfer(sp);
                    rx = Xfer.readXfer(sp);
                    if (rx.type != XferType.REPL_ROM_SIZE)
                        throw new IOException("Failed to get ROM size: " + rx.getDataAsString());
                    size = (uint)(rx.data[0] | (rx.data[1] << 8) | (rx.data[2] << 16) | (rx.data[3] << 24));
                    t.DownloadROM(wa.file, size);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_4M_ROM:
                    t.DownloadROM(wa.file, 0x400000);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_8M_ROM:
                    t.DownloadROM(wa.file, 0x800000);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_16M_ROM:
                    t.DownloadROM(wa.file, 0x1000000);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_32M_ROM:
                    t.DownloadROM(wa.file, 0x2000000);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_AUTO_SAVE:
                    new Xfer(XferType.REQ_BKP_TYPE, 0).sendXfer(sp);
                    rx = Xfer.readXfer(sp);
                    if (rx.type != XferType.REPL_BKP_TYPE)
                        throw new IOException("Failed to get save type: " + rx.getDataAsString());
                    size = (uint)(rx.data[0] | (rx.data[1] << 8) | (rx.data[2] << 16) | (rx.data[3] << 24));
                    if (size == 0x200)
                        t.DownloadEEPROM(wa.file, 0x200);
                    else if (size == 0x2000)
                        t.DownloadEEPROM(wa.file, 0x2000);
                    else if (size == 0x8000)
                        t.DownloadSRAM(wa.file, 0x8000);
                    else if (size == 0x10000)
                        t.DownloadFlash(wa.file, 0x10000);
                    else if (size == 0x20000)
                        t.DownloadFlash(wa.file, 0x20000);
                    else
                        throw new IOException("Invalid Save Type response: " + size);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_512_EEPROM:
                    t.DownloadEEPROM(wa.file, 0x200);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_8K_EEPROM:
                    t.DownloadEEPROM(wa.file, 0x2000);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_SRAM_32K:
                    t.DownloadSRAM(wa.file, 0x8000);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_FLASH_64K:
                    t.DownloadFlash(wa.file, 0x10000);
                    break;
                case WorkerArgs.OperationType.DOWNLOAD_FLASH_128K:
                    t.DownloadFlash(wa.file, 0x20000);
                    break;
                case WorkerArgs.OperationType.UPLOAD_AUTO_SAVE:
                    new Xfer(XferType.REQ_BKP_TYPE, 0).sendXfer(sp);
                    rx = Xfer.readXfer(sp);
                    if (rx.type != XferType.REPL_BKP_TYPE)
                        throw new IOException("Failed to get save type: " + rx.getDataAsString());
                    size = (uint)(rx.data[0] | (rx.data[1] << 8) | (rx.data[2] << 16) | (rx.data[3] << 24));
                    if (size == 0x200)
                        t.UploadEEPROM(wa.file, 0x200);
                    else if (size == 0x2000)
                        t.UploadEEPROM(wa.file, 0x2000);
                    else if (size == 0x8000)
                        t.UploadSRAM(wa.file, 0x8000);
                    else if (size == 0x10000)
                        t.UploadFlash(wa.file, 0x10000);
                    else if (size == 0x20000)
                        t.UploadFlash(wa.file, 0x20000);
                    else
                        throw new IOException("Invalid Save Type response: " + size);
                    break;
                case WorkerArgs.OperationType.UPLOAD_512_EEPROM:
                    t.UploadEEPROM(wa.file, 0x200);
                    break;
                case WorkerArgs.OperationType.UPLOAD_8K_EEPROM:
                    t.UploadEEPROM(wa.file, 0x2000);
                    break;
                case WorkerArgs.OperationType.UPLOAD_SRAM_32K:
                    t.UploadSRAM(wa.file, 0x8000);
                    break;
                case WorkerArgs.OperationType.UPLOAD_FLASH_64K:
                    t.UploadFlash(wa.file, 0x10000);
                    break;
                case WorkerArgs.OperationType.UPLOAD_FLASH_128K:
                    t.UploadFlash(wa.file, 0x20000);
                    break;
            }

            sp.Close();
            sw.Stop();
        }
    }

    class Transfer
    {
        BackgroundWorker bw;
        Stopwatch sw;
        SerialPort sp;
        DoWorkEventArgs e;
        long oldMillis;
        uint oldPos;

        public Transfer(BackgroundWorker bw, Stopwatch sw, SerialPort sp, DoWorkEventArgs e)
        {
            this.bw = bw;
            this.sw = sw;
            this.sp = sp;
            this.e = e;
            this.oldMillis = 0;
            this.oldPos = 0;
        }

        private void UpdateProgress(uint pos, uint total)
        {
            if (sw.ElapsedMilliseconds > oldMillis + 250)
            {
                double progress = pos * 100.0 / total;
                progress = Math.Round(progress, MidpointRounding.AwayFromZero);

                bw.ReportProgress((int)progress,
                    new ProgressState(
                        (int)(pos - oldPos) * 4,
                        (int)pos));
                oldPos = pos;
                oldMillis = sw.ElapsedMilliseconds;
            }
        }

        public void DownloadROM(string file, uint size)
        {
            FileStream f = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            size &= 0xFFFFF000;

            // seek to address 0
            new Xfer(XferType.REQ_ROM_SEEK, 0, (uint)0).sendXfer(sp);
            Xfer rx = Xfer.readXfer(sp);
            if (rx.type != XferType.REPL_ROM_SEEK)
                throw new IOException("ROM SEEK failed: " + rx.getDataAsString());

            // read data
            for (uint i = 0; i < size; i += 0x1000)
            {
                new Xfer(XferType.REQ_ROM_READ, 0, (ushort)0x1000).sendXfer(sp);
                rx = Xfer.readXfer(sp);
                if (rx.type != XferType.REPL_ROM_READ)
                    throw new IOException("ROM READ failed: " + rx.getDataAsString());
                f.Write(rx.data, 0, rx.data.Length);

                UpdateProgress(i, size);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            f.Close();
        }

        public void DownloadEEPROM(string file, uint size)
        {
            FileStream f = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            size &= 0xFFFFFFF8;

            // seek to address 0
            if (size == 0x200)
                new Xfer(XferType.REQ_EEPROM512_SEEK, 0, (ushort)0).sendXfer(sp);
            else if (size == 0x2000)
                new Xfer(XferType.REQ_EEPROM8K_SEEK, 0, (ushort)0).sendXfer(sp);
            else
                throw new Exception("INTERNAL ERROR: bad eeprom size");
            Xfer rx = Xfer.readXfer(sp);
            if (rx.type == XferType.REPL_ERR)
                throw new IOException("EEPROM SEEK failed: " + rx.getDataAsString());

            // read data
            for (uint i = 0; i < size; i += 8)
            {
                if (size == 0x200)
                    new Xfer(XferType.REQ_EEPROM512_READ, 0, (ushort)8).sendXfer(sp);
                else
                    new Xfer(XferType.REQ_EEPROM8K_READ, 0, (ushort)8).sendXfer(sp);
                rx = Xfer.readXfer(sp);
                if (rx.type == XferType.REPL_ERR)
                    throw new IOException("EEPROM READ failed: " + rx.getDataAsString());
                f.Write(rx.data, 0, rx.data.Length);

                UpdateProgress(i, size);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            f.Close();
        }

        public void DownloadSRAM(string file, uint size)
        {
            FileStream f = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            size &= 0xFFFFF000;

            // seek to address 0
            new Xfer(XferType.REQ_SRAM_SEEK, 0, (ushort)0).sendXfer(sp);
            Xfer rx = Xfer.readXfer(sp);
            if (rx.type != XferType.REPL_SRAM_SEEK)
                throw new IOException("SRAM SEEK failed: " + rx.getDataAsString());

            // read data
            for (uint i = 0; i < size; i += 0x1000)
            {
                new Xfer(XferType.REQ_SRAM_READ, 0, (ushort)0x1000).sendXfer(sp);
                rx = Xfer.readXfer(sp);
                if (rx.type != XferType.REPL_SRAM_READ)
                    throw new IOException("SRAM READ failed: " + rx.getDataAsString());
                f.Write(rx.data, 0, rx.data.Length);

                UpdateProgress(i, size);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            f.Close();
        }

        public void DownloadFlash(string file, uint size)
        {
            FileStream f = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            size &= 0xFFFFF000;

            // seek to address 0
            new Xfer(XferType.REQ_FLASH_SEEK, 0, (ushort)0).sendXfer(sp);
            Xfer rx = Xfer.readXfer(sp);
            if (rx.type != XferType.REPL_FLASH_SEEK)
                throw new IOException("FLASH SEEK failed: " + rx.getDataAsString());

            // optionally switch bank to banks
            bool useBanks = size > 0x10000;

            // read data
            for (uint i = 0; i < size; i += 0x1000)
            {
                if (useBanks && (size & 0xFFFF) == 0)
                {
                    byte bank = (byte)(size >> 16);
                    new Xfer(XferType.REQ_FLASH_BANK, 0, bank).sendXfer(sp);
                    rx = Xfer.readXfer(sp);
                    if (rx.type != XferType.REPL_FLASH_BANK)
                        throw new IOException("FLASH BANK failed: " + rx.getDataAsString());
                }

                new Xfer(XferType.REQ_FLASH_READ, 0, (ushort)0x1000).sendXfer(sp);
                rx = Xfer.readXfer(sp);
                if (rx.type != XferType.REPL_FLASH_READ)
                    throw new IOException("FLASH READ failed: " + rx.getDataAsString());
                f.Write(rx.data, 0, rx.data.Length);

                UpdateProgress(i, size);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            f.Close();
        }

        public void UploadEEPROM(string file, uint size)
        {
            FileStream f = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            size &= 0xFFFFFFF8;

            // seek to address 0
            if (size == 0x200)
                new Xfer(XferType.REQ_EEPROM512_SEEK, 0, (ushort)0).sendXfer(sp);
            else if (size == 0x2000)
                new Xfer(XferType.REQ_EEPROM8K_SEEK, 0, (ushort)0).sendXfer(sp);
            else
                throw new Exception("INTERNAL ERROR: bad eeprom size");
            Xfer rx = Xfer.readXfer(sp);
            if (rx.type == XferType.REPL_ERR)
                throw new IOException("EEPROM SEEK failed: " + rx.getDataAsString());

            // write data
            for (uint i = 0; i < size; i += 8)
            {
                byte[] d = new byte[8];
                f.Read(d, 0, d.Length);
                if (size == 0x200)
                    new Xfer(XferType.REQ_EEPROM512_WRITE, 0, d).sendXfer(sp);
                else
                    new Xfer(XferType.REQ_EEPROM8K_WRITE, 0, d).sendXfer(sp);
                rx = Xfer.readXfer(sp);
                if (rx.type == XferType.REPL_ERR)
                    throw new IOException("EEPROM WRITE failed: " + rx.getDataAsString());

                UpdateProgress(i, size);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            f.Close();
        }

        public void UploadSRAM(string file, uint size)
        {
            FileStream f = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            size &= 0xFFFFF000;

            // seek to address 0
            new Xfer(XferType.REQ_SRAM_SEEK, 0, (ushort)0).sendXfer(sp);
            Xfer rx = Xfer.readXfer(sp);
            if (rx.type != XferType.REPL_SRAM_SEEK)
                throw new IOException("SRAM SEEK failed: " + rx.getDataAsString());

            // write data
            for (uint i = 0; i < size; i += 0x1000)
            {
                byte[] d = new byte[0x1000];
                f.Read(d, 0, d.Length);
                new Xfer(XferType.REQ_SRAM_WRITE, 0, d).sendXfer(sp);
                rx = Xfer.readXfer(sp);
                if (rx.type != XferType.REPL_SRAM_WRITE)
                    throw new IOException("SRAM WRITE failed: " + rx.getDataAsString());

                UpdateProgress(i, size);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            f.Close();
        }

        public void UploadFlash(string file, uint size)
        {
            FileStream f = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            size &= 0xFFFFF000;

            // seek to address 0
            new Xfer(XferType.REQ_FLASH_SEEK, 0, (ushort)0).sendXfer(sp);
            Xfer rx = Xfer.readXfer(sp);
            if (rx.type != XferType.REPL_FLASH_SEEK)
                throw new IOException("FLASH SEEK failed: " + rx.getDataAsString());

            // erase flash
            new Xfer(XferType.REQ_FLASH_ERASE, 0).sendXfer(sp);
            rx = Xfer.readXfer(sp);
            if (rx.type != XferType.REPL_FLASH_ERASE)
                throw new IOException("FLASH BANK failed: " + rx.getDataAsString());

            // optionally switch bank to banks
            bool useBanks = size > 0x10000;

            // write data
            for (uint i = 0; i < size; i += 0x1000)
            {
                if (useBanks && (size & 0xFFFF) == 0)
                {
                    byte bank = (byte)(size >> 16);
                    new Xfer(XferType.REQ_FLASH_BANK, 0, bank).sendXfer(sp);
                    rx = Xfer.readXfer(sp);
                    if (rx.type != XferType.REPL_FLASH_BANK)
                        throw new IOException("FLASH BANK failed: " + rx.getDataAsString());
                }

                byte[] d = new byte[0x1000];
                f.Read(d, 0, d.Length);
                new Xfer(XferType.REQ_FLASH_WRITE, 0, d).sendXfer(sp);
                rx = Xfer.readXfer(sp);
                if (rx.type != XferType.REPL_FLASH_WRITE)
                    throw new IOException("FLASH WRITE failed: " + rx.getDataAsString());

                UpdateProgress(i, size);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            f.Close();
        }
    }

    class Xfer
    {
        public Xfer(XferType type, ushort id)
        {
            this.type = type;
            data = new byte[0];
            this.id = id;
        }

        public Xfer(XferType type, ushort id, byte data)
        {
            this.type = type;
            this.data = new byte[1];
            this.data[0] = data;
            this.id = id;
        }

        public Xfer(XferType type, ushort id, ushort data)
        {
            this.type = type;
            this.data = new byte[2];
            this.data[0] = (byte)data;
            this.data[1] = (byte)(data >> 8);
            this.id = id;
        }

        public Xfer(XferType type, ushort id, uint data)
        {
            this.type = type;
            this.data = new byte[4];
            this.data[0] = (byte)data;
            this.data[1] = (byte)(data >> 8);
            this.data[2] = (byte)(data >> 16);
            this.data[3] = (byte)(data >> 24);
            this.id = id;
        }

        public Xfer(XferType type, ushort id, byte[] data)
        {
            this.type = type;
            this.data = new byte[data.Length];
            Array.Copy(data, this.data, data.Length);
            this.id = id;
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

            XferType xferType = type.ToXferType();
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
            ushort t = type.ToUshort();
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

    enum XferType
    {
        REQ_ROM_SEEK,
        REQ_ROM_READ,
        REQ_ROM_WRITE,
        REQ_ROM_SIZE,
        REQ_BKP_TYPE,
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
        REPL_ROM_SIZE,
        REPL_BKP_TYPE,
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

    static class XferTypeExt
    {
        public static XferType ToXferType(this ushort type)
        {
            switch (type)
            {
                case 0x40:
                    return XferType.REQ_ROM_SEEK;
                case 0x41:
                    return XferType.REQ_ROM_READ;
                case 0x42:
                    return XferType.REQ_ROM_WRITE;
                case 0x48:
                    return XferType.REQ_ROM_SIZE;
                case 0x49:
                    return XferType.REQ_BKP_TYPE;
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
                case 0x148:
                    return XferType.REPL_ROM_SIZE;
                case 0x149:
                    return XferType.REPL_BKP_TYPE;
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
                case 0x1F0:
                    return XferType.REPL_ERR;
                default:
                    return XferType.UNKNOWN;
            }
        }

        public static ushort ToUshort(this XferType type)
        {
            switch (type)
            {
                case XferType.REQ_ROM_SEEK:
                    return 0x40;
                case XferType.REQ_ROM_READ:
                    return 0x41;
                case XferType.REQ_ROM_WRITE:
                    return 0x42;
                case XferType.REQ_ROM_SIZE:
                    return 0x48;
                case XferType.REQ_BKP_TYPE:
                    return 0x49;
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
                case XferType.REPL_ROM_SIZE:
                    return 0x148;
                case XferType.REPL_BKP_TYPE:
                    return 0x149;
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
                case XferType.REPL_ERR:
                    return 0x1F0;
                default:
                case XferType.UNKNOWN:
                    throw new ArgumentException("Internal Error. Invalid Xfer Type requested");
            }
        }
    }
}
