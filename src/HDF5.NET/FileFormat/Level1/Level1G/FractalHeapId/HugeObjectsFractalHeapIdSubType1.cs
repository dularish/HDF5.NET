﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace HDF5.NET
{
    public class HugeObjectsFractalHeapIdSubType1 : FractalHeapId
    {
        #region Fields

        private H5BinaryReader _reader;
        private Superblock _superblock;
        private FractalHeapHeader _heapHeader;

        #endregion

        #region Constructors

        internal HugeObjectsFractalHeapIdSubType1(H5BinaryReader reader, Superblock superblock, H5BinaryReader localReader, FractalHeapHeader header)
        {
            _reader = reader;
            _superblock = superblock;
            _heapHeader = header;

            // BTree2 key
            this.BTree2Key = H5Utils.ReadUlong(localReader, header.HugeIdsSize);
        }

        #endregion

        #region Properties

        public ulong BTree2Key { get; set; }

        #endregion

        #region Methods

        public override T Read<T>(Func<H5BinaryReader, T> func, [AllowNull]ref IEnumerable<BTree2Record01> record01Cache)
        {
            // huge objects b-tree v2
            if (record01Cache == null)
            {
                _reader.Seek((long)_heapHeader.HugeObjectsBTree2Address, SeekOrigin.Begin);
                var hugeBtree2 = new BTree2Header<BTree2Record01>(_reader, _superblock);
                record01Cache = hugeBtree2.EnumerateRecords();
            }

            var hugeRecord = record01Cache.FirstOrDefault(record => record.HugeObjectId == this.BTree2Key);
            _reader.Seek((long)hugeRecord.HugeObjectAddress, SeekOrigin.Begin);
            
            return func(_reader);
        }

        #endregion
    }
}
