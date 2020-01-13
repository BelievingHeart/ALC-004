﻿using System.IO;

namespace WPFCommon.Helpers
{
    public static class DirectoryHelper
    {
        public static string SolutionDirectory => Directory.GetParent(Directory.GetCurrentDirectory()).Parent?.Parent?.FullName;
    }
}