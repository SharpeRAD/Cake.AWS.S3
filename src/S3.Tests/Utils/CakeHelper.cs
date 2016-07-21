#region Using Statements
    using System.IO;

    using Cake.Core;
    using Cake.Core.IO;

    using NSubstitute;
#endregion



namespace Cake.AWS.S3.Tests
{
    internal static class CakeHelper
    {
        #region Functions (2)
            public static ICakeEnvironment CreateEnvironment()
            {
                var environment = Substitute.For<ICakeEnvironment>();
                environment.WorkingDirectory = Directory.GetCurrentDirectory();

                return environment;
            }



            public static IS3Manager CreateS3Manager()
            {
                return new S3Manager(new FileSystem(), CakeHelper.CreateEnvironment(), new DebugLog());
            }
        #endregion
    }
}
