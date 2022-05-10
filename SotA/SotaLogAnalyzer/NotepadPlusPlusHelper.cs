using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace SotaLogAnalyzer
{
    public class NotepadPlusPlusHelper
    {
        public static void OpenEditor(string path, int lineNumber)
        {
            if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Notepad++", null, null) is string notepadPath)
            {
                System.Diagnostics.Process.Start(notepadPath + @"\notepad++.exe", $"-n{lineNumber} \"{path}\"");
            }

            else
            {
                MessageBox.Show("Notepad++ not found", null, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
