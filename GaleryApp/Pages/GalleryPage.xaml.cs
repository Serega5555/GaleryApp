using GaleryApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GaleryApp.Pages
{
    public partial class GalleryPage : ContentPage
    {
        private ObservableCollection<ImageModel> images;
        public GalleryPage()
        {
            InitializeComponent();
            images = new ObservableCollection<ImageModel>();
            ImagesCollection.ItemsSource = images;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var readStatus = await Xamarin.Essentials.Permissions.CheckStatusAsync<Xamarin.Essentials.Permissions.StorageRead>();
            if (readStatus != Xamarin.Essentials.PermissionStatus.Granted)
            {
                readStatus = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageRead>();
            }

            if (readStatus != Xamarin.Essentials.PermissionStatus.Granted)
            {
                await DisplayAlert("Ошибка", "Нет доступа к памяти устройства.", "OK");
                return;
            }

            // Можно добавить проверку WRITE если хотите удалять файлы
            var writeStatus = await Xamarin.Essentials.Permissions.CheckStatusAsync<Xamarin.Essentials.Permissions.StorageWrite>();
            if (writeStatus != Xamarin.Essentials.PermissionStatus.Granted)
            {
                writeStatus = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageWrite>();
            }

            if (writeStatus != Xamarin.Essentials.PermissionStatus.Granted)
            {
                await DisplayAlert("Ошибка", "Нет доступа на запись файлов.", "OK");
                return;
            }

            LoadImages();
        }

        private void LoadImages()
        {
            try
            {
                images.Clear();

                // Путь к папке камеры
                var cameraPath = "/storage/emulated/0/DCIM/Camera";

                // Проверим, существует ли папка
                if (Directory.Exists(cameraPath))
                {
                    var files = Directory.GetFiles(cameraPath).Where(f => f.EndsWith(".jpg") || f.EndsWith(".png")).OrderByDescending(f => File.GetCreationTime(f)).ToList();

                    foreach (var file in files)
                    {
                        images.Add(new ImageModel
                        {
                            FilePath = file,
                            FileName = Path.GetFileName(file),
                            DateTaken = File.GetCreationTime(file).ToString("dd.MM.yyyy HH:mm:ss")
                        });
                    }
                }
                else
                {
                    DisplayAlert("Ошибка", "Папка камеры не найдена.", "ОК");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Ошибка", $"Не удалось загрузить изображения: {ex.Message}", "ОК");
            }
        }

        private async void OpenButton_Clicked(object sender, EventArgs e)
        {
            var selected = (ImageModel)ImagesCollection.SelectedItem;
            if (selected == null)
            {
                await DisplayAlert("Внимание", "Выберите изображение", "ОК");
                return;
            }

            await Navigation.PushAsync(new ImageViewPage(selected));
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var selected = (ImageModel)ImagesCollection.SelectedItem;
            if (selected == null)
            {
                await DisplayAlert("Внимание", "Выберите изображение для удаления", "ОК");
                return;
            }
            var confirm = await DisplayAlert("Подтверждение", $"Вы уверены, что хотите удалить {selected.FileName}?", "Да", "Нет");
            if (confirm)
            {
                try
                {
                    File.Delete(selected.FilePath);
                    images.Remove(selected);
                    await DisplayAlert("Успех", "Изображение удалено.", "ОК");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", $"Не удалось удалить изображение: {ex.Message}", "ОК");
                }
            }
        }
    }
}
