﻿namespace HDF5.NET
{
    internal class HyperslabSelectionInfo1 : HyperslabSelectionInfo
    {
        #region Constructors

        public HyperslabSelectionInfo1(H5BinaryReader reader) : base(reader)
        {
            // reserved
            reader.ReadBytes(4);

            // length
            var length = reader.ReadUInt32();

            // rank
            this.Rank = reader.ReadUInt32();

            // block count
            this.BlockCount = reader.ReadUInt32();

            // block offsets
            var totalOffsets = this.BlockCount * 2 * this.Rank;
            this.BlockOffsets = new uint[totalOffsets];

            for (uint i = 0; i < totalOffsets; i++)
            {
                this.BlockOffsets[i] = reader.ReadUInt32();
            }
        }

        #endregion

        #region Properties

        public uint Rank { get; set; }
        public uint BlockCount { get; set; }
        public uint[] BlockOffsets { get; set; }

        #endregion
    }
}
