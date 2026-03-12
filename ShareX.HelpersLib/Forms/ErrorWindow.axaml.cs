using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace ShareX.HelpersLib
{
    public partial class ErrorWindow : Window
    {
        public ErrorWindow()
        {
            InitializeComponent();
            
            // Demo content
            lblErrorMessage.Text = "An error has occurred:";
            txtException.Text = "System.NullReferenceException: Object reference not set to an instance of an object.\n   at ShareX.Program.Main(String[] args)\n   at System.AppDomain._nExecuteAssembly(RuntimeAssembly assembly, String[] args)";
            
            btnSendBugReport.Click += BtnSendBugReport_Click;
            btnOpenLogFile.Click += BtnOpenLogFile_Click;
            btnContinue.Click += BtnContinue_Click;
            btnClose.Click += BtnClose_Click;
            btnOK.Click += BtnOK_Click;
        }

        private void BtnSendBugReport_Click(object? sender, RoutedEventArgs e)
        {
            // Would open browser to github issues
        }

        private void BtnOpenLogFile_Click(object? sender, RoutedEventArgs e)
        {
            // Would open log file
        }

        private void BtnContinue_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnClose_Click(object? sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void BtnOK_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
