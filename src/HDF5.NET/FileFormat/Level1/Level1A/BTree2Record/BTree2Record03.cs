﻿namespace HDF5.NET
{
    internal struct BTree2Record03 : IBTree2Record
    {
        #region Constructors

        public BTree2Record03(H5BinaryReader reader, Superblock superblock)
        {
            this.HugeObjectAddress = superblock.ReadOffset(reader);
            this.HugeObjectLength = superblock.ReadLength(reader);
        }

        #endregion

        #region Properties

        public ulong HugeObjectAddress { get; set; }
        public ulong HugeObjectLength { get; set; }

        #endregion
    }
}
