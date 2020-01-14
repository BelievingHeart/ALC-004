using System.IO;
using WPFCommon.Helpers;

namespace ALC.Core.Constants
{
    public static class DirectoryConstants
    {
        public static string ConfigBaseDir = "D:\\ALC-Configs";

        public static string DocumentaryDir => Path.Combine(DirectoryHelper.SolutionDirectory, "Docs");
        public static string ErrorLogDir => "D:\\ALC-Logs";
    }
}