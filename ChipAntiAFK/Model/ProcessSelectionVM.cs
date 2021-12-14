using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace ChipAntiAFK
{
    public class ProcessSelectionVM : NotifyPropertyChangedBase
    {
        public ProcessSelectionVM()
        {
            AllProcesses = Process.GetProcesses().Where(p => p.MainWindowTitle.Length != 0 && p.ProcessName.Contains("ffxiv")).ToList();
        }

        public IList<Process> AllProcesses { get; set; }

        public Process SelectedProcess { get; set; }

        public ICommand OpenProcessButtonCommand
        {
            get
            {
                return new RelayCommand(
                    (obj) =>
                    {
                        return SelectedProcess != null;
                    },
                    (obj) =>
                    {
                        Program.Instance.ActiveProcess = SelectedProcess;
                        (obj as ICloseable).Close();
                    }
                );
            }
        }
    }
}
