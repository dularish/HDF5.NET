﻿using System.Diagnostics;

namespace HDF5.NET
{
    [DebuggerDisplay("{Name}: Class = '{Datatype.Class}'")]
    public partial class H5CommitedDatatype : H5Object
    {
        #region Constructors

        internal H5CommitedDatatype(H5Context context, ObjectHeader header, H5NamedReference reference) 
            : base(context, reference, header)
        {
            this.Datatype = header.GetMessage<DatatypeMessage>();
        }

        #endregion
    }
}