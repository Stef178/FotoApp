﻿using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.ViewModel
{
    public class HomePageViewModel
    {
        public ICommand NavigateCommand { get; }

        public HomePageViewModel()
        {
            NavigateCommand = new Command<string>(Navigate);
        }

        private async void Navigate(string pageName)
        {
            
            Page page = pageName switch
            {
				"HomePage" => new FotoApp.MVVM.View.HomePage(),
                "Assignments" => new FotoApp.MVVM.View.Assignments(),
                "UserPhotos" => new FotoApp.MVVM.View.UserPhotos(),
                "User" => new FotoApp.MVVM.View.UserManagement(),
                _ => null
			};

            if (page != null)
            {
                
                await Application.Current.MainPage.Navigation.PushAsync(page);
            }
        }
    }
}
