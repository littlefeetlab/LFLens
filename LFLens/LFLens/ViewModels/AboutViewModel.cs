using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LFLens.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://github.com/littlefeetlab"));
            logolinkcommand = new Command(async () => await Browser.OpenAsync("https://littlefeetservices.com/"));
        }

        public ICommand OpenWebCommand { get; }
        public ICommand logolinkcommand { get; }
    }
}