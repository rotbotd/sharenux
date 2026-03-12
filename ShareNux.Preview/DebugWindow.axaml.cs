using Avalonia.Controls;

namespace ShareNux.Preview
{
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
            
            // Demo content
            rtbDebug.Text = "[2026-03-11 19:55:00] ShareX started\n[2026-03-11 19:55:01] Loading settings...\n[2026-03-11 19:55:02] Settings loaded successfully\n[2026-03-11 19:55:03] Ready";
            llRunningFrom.Text = "/usr/share/sharex";
        }
    }
}
