using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GaleryApp.Pages
{
    public partial class PinPage : ContentPage
    {
        private bool isFirstLaunch = false;
        public PinPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Проверим, есть ли сохраненный PIN
            var storedPin = await SecureStorage.GetAsync("user_pin");

            if (string.IsNullOrEmpty(storedPin))
            {
                // Первый запуск - установка PIN
                isFirstLaunch = true;
                label_message.Text = "Создайте PIN-код из 4 цифр для доступа к галерее";
                label_message.IsVisible = true;
            }
            else
            {
                // Очищаем поле PIN
                entry_pin.Text = string.Empty;
                //PIN- код уже существует
                isFirstLaunch = false;
                label_message.Text = "Введите ваш PIN-код";
                label_message.IsVisible = true;
            }
        }

        private async void SavePinButton_Clicked (object sender, EventArgs e)
        {
            var enteredPin = entry_pin.Text;

            if (string.IsNullOrWhiteSpace(enteredPin) || enteredPin.Length != 4)
            {
                label_message.Text = "PIN-код должен состоять из 4 цифр";
                label_message.IsVisible = true;
                return;
            }

            if (isFirstLaunch)
            {
                // Сохраним PIN-код
                await SecureStorage.SetAsync("user_pin", enteredPin);
                await DisplayAlert("Успех", "PIN-код успешно установлен", "ОК");

                // Очищаем поле PIN
                entry_pin.Text = string.Empty;

                // Переходим в галерею
                await Navigation.PushAsync(new GalleryPage());
            }
            else
            {
                var storedPin = await SecureStorage.GetAsync("user_pin");
                if (enteredPin == storedPin)
                {
                    // Успешный вход
                    await Navigation.PushAsync(new GalleryPage());
                }
                else
                {
                    label_message.Text = "Неверный PIN-код. Попробуйте снова";
                    label_message.IsVisible = true;
                }
            }
        }
    }
}
