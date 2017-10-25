#region Using Statements
using System.IO;

using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
#endregion



namespace Cake.AWS.S3.Tests
{
    internal static class CakeHelper
    {
        #region Methods
        public static ICakeEnvironment CreateEnvironment()
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();

            environment.WorkingDirectory = Directory.GetCurrentDirectory();
            environment.WorkingDirectory = environment.WorkingDirectory.Combine("../../../");

            return environment;
        }



        public static IS3Manager CreateS3Manager()
        {
            var environment = CakeHelper.CreateEnvironment();

            return new S3Manager(new FileSystem(), environment, new DebugLog());
        }
        #endregion
    }
}
