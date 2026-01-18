using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Lopushok.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lopushok;

public partial class AddProduct : Window
{
    private string? _imageName = null;

    public AddProduct()
    {
        InitializeComponent();
        LoadTypeBox();
    }

    private void LoadTypeBox()
    {
        using var context = new LopushokContext();
        TypeBox.ItemsSource = context.Producttypes.Select(Producttype => Producttype.Title).ToList();
    }


    private async void AddProduct_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!await Validation())
        {
            return;
        }

        using var context = new LopushokContext();
        var p = int.TryParse(PersonCountBox.Text, out int person);
        var w = int.TryParse(WorkShopNumberBox.Text, out int workshopnumber);
        var min = int.TryParse(MinCostBox.Text, out int mincost);

        var newProduct = new Product
        {
            Title = TitleBox.Text,
            Articlenumber = ArticleBox.Text,
            Image = _imageName,
            Productionpersoncount = person,
            Productionworkshopnumber = workshopnumber,
            Mincostforagent = mincost

        };

        if (TypeBox.SelectedItem != null)
        {
            var productType = context.Producttypes.FirstOrDefault(Producttype => Producttype.Title == TypeBox.SelectedItem.ToString());
            newProduct.Producttypeid = productType.Id;
        }

        context.Products.Add(newProduct);
        await context.SaveChangesAsync();

        var successMessage = MessageBoxManager.GetMessageBoxStandard("Успешно", "Продукт добавлен", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Success);
        await successMessage.ShowAsync();

        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }

    private async Task<bool> Validation()
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text) ||
            string.IsNullOrWhiteSpace(ArticleBox.Text) ||
            string.IsNullOrWhiteSpace(PersonCountBox.Text) ||
            string.IsNullOrWhiteSpace(WorkShopNumberBox.Text) ||
            string.IsNullOrWhiteSpace(MinCostBox.Text))
        {
            var errorMessage = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Пустые поля", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await errorMessage.ShowAsync();
            return false;
        }
        if (!int.TryParse(PersonCountBox.Text, out int person) || person <= 0)
        {
            var errorMessage = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Количество человек должно быть положительным", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await errorMessage.ShowAsync();
            return false;
        }

        if (!int.TryParse(WorkShopNumberBox.Text, out int workshopnumber) || workshopnumber <= 0)
        {
            var errorMessage = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Номер цеха должен быть положительным", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await errorMessage.ShowAsync();
            return false;
        }

        if (!decimal.TryParse(MinCostBox.Text, out decimal mincost) || mincost <= 0)
        {
            var errorMessage = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Минимальная стоимость должна быть положительной", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await errorMessage.ShowAsync();
            return false;
        }

        return true;
    }

    private async void AddImage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        try
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                Title = "Выбор изображения",
                FileTypeFilter = new[] { Avalonia.Platform.Storage.FilePickerFileTypes.ImageAll }
            });

            if (files.Count > 0)
            {
                _imageName = files[0].Name;
                using var stream = await files[0].OpenReadAsync();
                ImageBox.Source = new Avalonia.Media.Imaging.Bitmap(stream);
            }
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard(
                "Ошибка",
                $"Не удалось загрузить изображение: {ex.Message}",
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error
            ).ShowAsync();
        }
    }

    private void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}
