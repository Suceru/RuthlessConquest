﻿using System;

namespace Game
{
    public class ZipArchiveEntry
    {
        public override string ToString()
        {
            return this.FilenameInZip;
        }

        public ZipArchive.Compression Method;

        public string FilenameInZip;

        public uint FileSize;

        public uint CompressedSize;

        public uint HeaderOffset;

        public uint FileOffset;

        public uint HeaderSize;

        public uint Crc32;

        public DateTime ModifyTime;

        public string Comment;

        public bool EncodeUTF8;
    }
}
