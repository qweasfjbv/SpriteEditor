using UnityEngine;

namespace AutoAnimaker
{
    public static class StringUtils
    {
        public static string PreprocessPath(string absolutePath)
        {
            // start with "Assets/"
            string relativePath = "";
            string[] folders = absolutePath.Split('/');
            bool isInAssets = false;

            foreach (var folder in folders)
            {
                if (folder == "Assets" || folder == "assets")
                {
                    isInAssets = true;
                }

                if (isInAssets)
                {
                    relativePath += folder + "/";
                }
            }

            if (relativePath == "")
            {
                return PreprocessPath(Constants.PATH_BASIC);
            }

            return relativePath;
        }
        public static string GetConventionedName(string[] names, FileNameConventionEnum fileNameConvention)
        {
            if (names == null || names.Length == 0) return "";
            string ret = "";

            for (int i = 0; i < names.Length; i++)
            {
                switch (fileNameConvention) {
                    case FileNameConventionEnum.SnakeCase:
                        ret = ret + names[i].ToLower();
                        if (i != names.Length - 1) ret = ret + "_";
                        break;
                    case FileNameConventionEnum.CamelCase:
                        ret = ret + names[i].FirstCharToUpper();
                        Debug.Log(ret);
                        break;
                }
            }
            return ret;
        }
    }
}