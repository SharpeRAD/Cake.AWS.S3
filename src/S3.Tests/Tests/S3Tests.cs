#region Using Statements
    using System;
    using System.IO;
    using System.Collections.Generic;

    using Xunit;

    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Core.Diagnostics;

    using Cake.AWS.S3;
#endregion



namespace Cake.AWS.S3.Tests
{
    public class S3Tests
    {
        [Fact]
        public void File_List_Dates()
        {
            ICakeEnvironment environment = CakeHelper.CreateEnvironment();
            ICakeLog log = new DebugLog();
            IFileSystem system = new FileSystem();

            IDirectory dir = system.GetDirectory(new DirectoryPath("../../").MakeAbsolute(environment));
            IEnumerable<IFile> files = dir.GetFiles("*", SearchScope.Recursive);



            log.Debug("================= File Modified Dates =================");

            foreach (IFile file in files)
            {
                DateTimeOffset date = (DateTimeOffset)new FileInfo(file.Path.FullPath).LastWriteTime;

                log.Debug(date.ToString() + "   ===   " + file.Path.FullPath);
            }

            log.Debug("================= File Modified Dates =================");
        }
    }
}
