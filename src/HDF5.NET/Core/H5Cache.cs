﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace HDF5.NET
{
    internal static class H5Cache
    {
        #region Constructors

        static H5Cache()
        {
            _globalHeapMap = new ConcurrentDictionary<Superblock, Dictionary<ulong, GlobalHeapCollection>>();
            _fileMap = new ConcurrentDictionary<Superblock, Dictionary<string, H5File>>();
        }

        #endregion

        #region Shared

        public static void Clear(Superblock superblock)
        {
            // global heap
            if (_globalHeapMap.ContainsKey(superblock))
                _globalHeapMap.TryRemove(superblock, out var _);


            // file map
            if (_fileMap.ContainsKey(superblock))
            {
                var pathToH5FileMap = _fileMap[superblock];

                foreach (var h5File in pathToH5FileMap.Values)
                {
                    h5File.Dispose();
                }

                _fileMap.TryRemove(superblock, out var _);
            }
        }

        #endregion

        #region Global Heap

        private static ConcurrentDictionary<Superblock, Dictionary<ulong, GlobalHeapCollection>> _globalHeapMap;

        public static GlobalHeapCollection GetGlobalHeapObject(H5BinaryReader reader, Superblock superblock, ulong address)
        {
            if (!_globalHeapMap.TryGetValue(superblock, out var addressToCollectionMap))
            {
                addressToCollectionMap = new Dictionary<ulong, GlobalHeapCollection>();
                _globalHeapMap.AddOrUpdate(superblock, addressToCollectionMap, (_, oldAddressToCollectionMap) => addressToCollectionMap);
            }

            if (!addressToCollectionMap.TryGetValue(address, out var collection))
            {
                collection = H5Cache.ReadGlobalHeapCollection(reader, superblock, address);
                addressToCollectionMap[address] = collection;
            }

            return collection;
        }

        private static GlobalHeapCollection ReadGlobalHeapCollection(H5BinaryReader reader, Superblock superblock, ulong address)
        {
            reader.Seek((long)address, SeekOrigin.Begin);
            return new GlobalHeapCollection(reader, superblock);
        }

        #endregion

        #region File Handles

        private static ConcurrentDictionary<Superblock, Dictionary<string, H5File>> _fileMap;

        public static H5File GetH5File(Superblock superblock, string absoluteFilePath)
        {
            if (!Uri.TryCreate(absoluteFilePath, UriKind.Absolute, out var uri))
                throw new Exception("The provided path is not absolute.");

            if (!uri.IsFile && !uri.IsUnc)
                throw new Exception("The provided path is not a file path or aN UNC path.");

            if (!_fileMap.TryGetValue(superblock, out var pathToH5FileMap))
            {
                pathToH5FileMap = new Dictionary<string, H5File>();
                _fileMap.AddOrUpdate(superblock, pathToH5FileMap, (_, oldPathToH5FileMap) => pathToH5FileMap);
            }

            if (!pathToH5FileMap.TryGetValue(uri.AbsoluteUri, out var h5File))
            {
#warning This does not correspond to https://support.hdfgroup.org/HDF5/doc/RM/H5L/H5Lcreate_external.htm
                h5File = H5File.Open(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                pathToH5FileMap[uri.AbsoluteUri] = h5File;
            }

            return h5File;
        }

        #endregion
    }
}
