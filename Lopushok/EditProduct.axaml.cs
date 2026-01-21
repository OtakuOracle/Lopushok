using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Lopushok.Models;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Linq;
using Avalonia.Platform.Storage;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Avalonia.Controls.Chrome;

namespace Lopushok;

public partial class EditProduct : Window
{
    private readonly Product _product;
    private string _imageName;
    private readonly ObservableCollection<Productmaterial> _materials = new ObservableCollection<Productmaterial>();

    public EditProduct()
    {
        InitializeComponent();
    }

    public EditProduct(Models.Product product) : this()
    {
        _product = product;
        _imageName = _product.Image ?? Guid.NewGuid().ToString("N");
        LoadTypeBox();
        LoadMaterials();
    }

    private void LoadMaterials()
    {
        using var context = new LopushokContext();
        var materials = context.Productmaterials
            .Include(Productmaterials => Productmaterials.Material)
            .Where(Productmaterials => Productmaterials.Productid == _product.Id)
            .ToList();

        _materials.Clear();
        foreach (var material in materials)
        {
            _materials.Add(material);
        }

        MaterialsListBox.ItemsSource = _materials;
    }

    private void AddMaterial_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var addMaterialWindow = new AddMaterial(_product.Id);
        addMaterialWindow.MaterialAdded += OnMaterialAdded;
        addMaterialWindow.ShowDialog(this);
    }

    private void OnMaterialAdded()
    {
        LoadMaterials();
    }

    private async void AddProduct_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!Validation())
        {
            return;
        }

        using var context = new LopushokContext();
        var productToUpdate = await context.Products.FindAsync(_product.Id);

        if (productToUpdate != null)
        {
            UpdateProductDetails(productToUpdate, context);
            await context.SaveChangesAsync();

            await ShowSuccessMessage();
            OpenMainWindow();
            this.Close();
        }
    }

    private void UpdateProductDetails(Product productToUpdate, LopushokContext context)
    {
        productToUpdate.Title = TitleBox.Text;
        productToUpdate.Articlenumber = ArticleBox.Text;
        productToUpdate.Image = _imageName;

        if (int.TryParse(PersonCountBox.Text, out int person))
            productToUpdate.Productionpersoncount = person;

        if (int.TryParse(WorkShopNumberBox.Text, out int workshopnumber))
            productToUpdate.Productionworkshopnumber = workshopnumber;

        if (decimal.TryParse(MinCostBox.Text, out decimal mincost))
            productToUpdate.Mincostforagent = mincost;

        if (TypeBox.SelectedItem is string productTypeName)
        {
            var typeId = context.Producttypes
                .Where(Producttypes => Producttypes.Title == productTypeName)
                .Select(Producttypes => Producttypes.Id)
                .FirstOrDefault();

            productToUpdate.Producttypeid = typeId;
        }
    }


    private bool Validation()
    {
        if (IsAnyFieldEmpty())
        {
            ShowErrorMessage("Ошибка", "Необходимо заполнить все обязательные поля.");
            return false;
        }

        if (!ValidateNumericField(PersonCountBox.Text, "Количество человек") ||
            !ValidateNumericField(WorkShopNumberBox.Text, "Номер цеха") ||
            !ValidateDecimalField(MinCostBox.Text, "Минимальная стоимость"))
        {
            return false;
        }

        return true;
    }

    private bool IsAnyFieldEmpty()
    {
        return string.IsNullOrEmpty(TitleBox.Text) ||
               string.IsNullOrEmpty(ArticleBox.Text) ||
               string.IsNullOrEmpty(TypeBox.SelectedItem?.ToString()) ||
               string.IsNullOrEmpty(PersonCountBox.Text) ||
               string.IsNullOrEmpty(WorkShopNumberBox.Text) ||
               string.IsNullOrEmpty(MinCostBox.Text);
    }

    private async Task ShowSuccessMessage()
    {
        var successMessage = MessageBoxManager.GetMessageBoxStandard("Успешно", "Продукт обновлен", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Success);
        await successMessage.ShowAsync();
    }

    private void OpenMainWindow()
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    private async void AddImage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
        {
            Title = "Выберите изображение",
            FileTypeChoices = new[]
            {
            new FilePickerFileType("Изображения")
            {
                Patterns = new[] { "*.jpg", "*.jpeg", "*.png" }
            }
        }
        });

        if (file != null)
        {
            ImageBox.Source = new Bitmap(file.Path.LocalPath);
            var targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _imageName + Path.GetExtension(file.Name));
            File.Copy(file.Path.LocalPath, targetPath, overwrite: true);
            _imageName = targetPath;
        }
    }

    private void GetInfo()
    {
        using (var context = new LopushokContext())
        {
            TitleBox.Text = _product.Title;
            ArticleBox.Text = _product.Articlenumber;
            PersonCountBox.Text = _product.Productionpersoncount?.ToString();
            WorkShopNumberBox.Text = _product.Productionworkshopnumber?.ToString();
            MinCostBox.Text = _product.Mincostforagent.ToString("F2");

            if (!string.IsNullOrEmpty(_product.Image) && File.Exists(_product.Image))
            {
                ImageBox.Source = new Bitmap(_product.Image);
            }
            else
            {
                ImageBox.Source = new Bitmap("picture.png");
            }

            var productType = context.Producttypes.FirstOrDefault(Producttypes => Producttypes.Id == _product.Producttypeid);

            if (productType != null)
            {
                TypeBox.SelectedItem = productType.Title;
            }
        }
    }



    private void LoadTypeBox()
    {

        using var context = new LopushokContext();
        var typeProducts = context.Producttypes.Select(Producttypes => Producttypes.Title).ToList();
        TypeBox.ItemsSource = typeProducts;

    }


    private async void ShowErrorMessage(string title, string message)
    {
        var errorMessage = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
        await errorMessage.ShowAsync();
    }

    private bool ValidateNumericField(string input, string fieldName)
    {
        if (!int.TryParse(input, out int value))
        {
            ShowErrorMessage("Ошибка", $"Некорректное значение для поля '{fieldName}'.");
            return false;
        }

        if (value <= 0)
        {
            ShowErrorMessage("Ошибка", $"Поле '{fieldName}' должно быть больше нуля.");
            return false;
        }

        return true;
    }

    private bool ValidateDecimalField(string input, string fieldName)
    {
        if (!decimal.TryParse(input, out decimal value))
        {
            ShowErrorMessage("Ошибка", $"Некорректное значение для поля '{fieldName}'.");
            return false;
        }

        if (value <= 0)
        {
            ShowErrorMessage("Ошибка", $"Поле '{fieldName}' должно быть больше нуля.");
            return false;
        }

        return true;
    }

    private async void DeleteProduct_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var context = new LopushokContext();

        if (_product != null)
        {
            var question = MessageBoxManager.GetMessageBoxStandard("Подтверждение", "Вы действительно хотите удалить продукт?", ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Info);
            var answer = await question.ShowAsync();

            if (answer == ButtonResult.Yes)
            {
                context.Products.Remove(_product);
                await context.SaveChangesAsync();

                var successMessage = MessageBoxManager.GetMessageBoxStandard("Успешно", "Продукт удален.", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Success);
                await successMessage.ShowAsync();

                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }
    }

    private void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }

}