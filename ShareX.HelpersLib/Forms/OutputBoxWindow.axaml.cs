using Avalonia.Controls;

namespace ShareX.HelpersLib
{
    public partial class OutputBoxWindow : Window
    {
        public OutputBoxWindow()
        {
            InitializeComponent();
            
            // Demo content
            rtbText.Text = "Sample output text...\n\nThis is a multiline output box.\nIt can contain logs, results, or other text content.";
        }

        public OutputBoxWindow(string text, string title) : this()
        {
            Title = title;
            rtbText.Text = text;
        }
    }
}
