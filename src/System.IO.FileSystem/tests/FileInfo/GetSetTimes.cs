// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace System.IO.Tests
{
    public class FileInfo_GetSetTimes : InfoGetSetTimes<FileInfo>
    {
        public override FileInfo GetExistingItem()
        {
            string path = GetTestFilePath();
            File.Create(path).Dispose();
            return new FileInfo(path);
        }

        public override FileInfo GetMissingItem() => new FileInfo(GetTestFilePath());

        public override void InvokeCreate(FileInfo item) => item.Create();

        public override IEnumerable<TimeFunction> TimeFunctions(bool requiresRoundtripping = false)
        {
            if (IOInputs.SupportsGettingCreationTime && (!requiresRoundtripping || IOInputs.SupportsSettingCreationTime))
            {
                yield return TimeFunction.Create(
                    ((testFile, time) => { testFile.CreationTime = time; }),
                    ((testFile) => testFile.CreationTime),
                    DateTimeKind.Local);
                yield return TimeFunction.Create(
                    ((testFile, time) => { testFile.CreationTimeUtc = time; }),
                    ((testFile) => testFile.CreationTimeUtc),
                    DateTimeKind.Unspecified);
                yield return TimeFunction.Create(
                    ((testFile, time) => { testFile.CreationTimeUtc = time; }),
                    ((testFile) => testFile.CreationTimeUtc),
                    DateTimeKind.Utc);
            }
            yield return TimeFunction.Create(
                ((testFile, time) => { testFile.LastAccessTime = time; }),
                ((testFile) => testFile.LastAccessTime),
                DateTimeKind.Local);
            yield return TimeFunction.Create(
                ((testFile, time) => { testFile.LastAccessTimeUtc = time; }),
                ((testFile) => testFile.LastAccessTimeUtc),
                DateTimeKind.Unspecified);
            yield return TimeFunction.Create(
                ((testFile, time) => { testFile.LastAccessTimeUtc = time; }),
                ((testFile) => testFile.LastAccessTimeUtc),
                DateTimeKind.Utc);
            yield return TimeFunction.Create(
                ((testFile, time) => { testFile.LastWriteTime = time; }),
                ((testFile) => testFile.LastWriteTime),
                DateTimeKind.Local);
            yield return TimeFunction.Create(
                ((testFile, time) => { testFile.LastWriteTimeUtc = time; }),
                ((testFile) => testFile.LastWriteTimeUtc),
                DateTimeKind.Unspecified);
            yield return TimeFunction.Create(
                ((testFile, time) => { testFile.LastWriteTimeUtc = time; }),
                ((testFile) => testFile.LastWriteTimeUtc),
                DateTimeKind.Utc);
        }

        public void DeleteAfterEnumerate_TimesStillSet()
        {
            // When enumerating we populate the state as we already have it.
            DateTime beforeTime = DateTime.UtcNow.AddSeconds(-1);
            string filePath = GetTestFilePath();
            File.Create(filePath).Dispose();
            FileInfo info = new DirectoryInfo(TestDirectory).EnumerateFiles().First();

            DateTime afterTime = DateTime.UtcNow.AddSeconds(1);

            // Deleting doesn't change any info state
            info.Delete();
            ValidateSetTimes(info, beforeTime, afterTime);
        }
    }
}
