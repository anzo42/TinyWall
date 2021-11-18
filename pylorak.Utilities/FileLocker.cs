﻿using System.Collections.Generic;
using System.IO;

namespace PKSoft
{
    internal static class FileLocker
    {
        private static readonly Dictionary<string, FileStream> LockedFiles = new Dictionary<string, FileStream>();

        internal static bool LockFile(string filePath, FileAccess localAccess, FileShare shareMode)
        {
            if (IsLocked(filePath))
                return false;

            try
            {
                LockedFiles.Add(filePath, new FileStream(filePath, FileMode.OpenOrCreate, localAccess, shareMode));
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static FileStream GetStream(string filePath)
        {
            return LockedFiles[filePath];
        }

        internal static bool IsLocked(string filePath)
        {
            return LockedFiles.ContainsKey(filePath);
        }

        internal static bool UnlockFile(string filePath)
        {
            if (!IsLocked(filePath))
                return false;

            try
            {
                LockedFiles[filePath].Close();
                LockedFiles.Remove(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static void UnlockAll()
        {
            // We cannot remove items from a dictionary while we are iterating over it.
            // So we iterate over a copy, and modify the original.

            Dictionary<string, FileStream> listCopy = new Dictionary<string, FileStream>();
            foreach (var elem in LockedFiles)
                listCopy.Add(elem.Key, elem.Value);

            foreach (string filePath in listCopy.Keys)
                UnlockFile(filePath);
        }
    }
}