using RequestTester.Entities;
using System.Diagnostics;
using System.IO;

namespace RequestTester.Managers
{
    public static class DiffManager
    {
        
        internal static void ShowDiff(RequestCase requestCase)
        {
            string filePath = Path.GetTempPath();

            string winMergeString = "";
            int i = 0;
            foreach (var result in requestCase.Responses.Values)
            {
                File.WriteAllText($"{filePath}{i}.txt", result.body);
                winMergeString += $"{filePath}{i++}.txt ";
            }

            Process.Start("WinMergeU.exe", winMergeString);

        }
    }
}
