using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Lopushok.Models;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lopushok;

public partial class AddMaterial : Window
{
    private readonly int _productId;
    public event Action MaterialAdded;


    public AddMaterial()
    {
        InitializeComponent();
    }

    public AddMaterial(int id)
    {
        InitializeComponent();
        _productId = id;
        LoadMaterialBoxAsync();
    }


    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!ValidateInputs(out int quantity))
        {
            return;
        }

        using var context = new LopushokContext();

        var selectedMaterialTitle = MaterialBox.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedMaterialTitle))
        {
            await ShowErrorMessage("Не выбран материал");
            return;
        }

        var material = await context.Materials.FirstOrDefaultAsync(material => material.Title == selectedMaterialTitle);

        if (material == null)
        {
            await ShowErrorMessage("Материал не найден");
            return;
        }

        if (await context.Productmaterials.AnyAsync(Productmaterials => Productmaterials.Productid == _productId && Productmaterials.Materialid == material.Id))
        {
            await ShowErrorMessage("Материал уже добавлен к продукту");
            return;
        }

        var productMaterial = new Productmaterial
        {
            Productid = _productId,
            Materialid = material.Id,
            Quantity = quantity
        };

        context.Productmaterials.Add(productMaterial);
        await context.SaveChangesAsync();

        MaterialAdded?.Invoke();

        await ShowSuccessMessage("Материал  добавлен");
        this.Close();
    }


    private async Task ShowErrorMessage(string message)
    {
        var errorMessage = MessageBoxManager.GetMessageBoxStandard("Ошибка", message, ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
        await errorMessage.ShowAsync();
    }

    private async Task ShowSuccessMessage(string message)
    {
        var successMessage = MessageBoxManager.GetMessageBoxStandard("Успешно", message, ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Success);
        await successMessage.ShowAsync();
    }

    private bool ValidateInputs(out int quantity)
    {
        quantity = 0;

        if (MaterialBox.SelectedItem == null)
        {
            ShowErrorMessage("Необходимо выбрать материал").Wait(); 
            return false;
        }

        if (!int.TryParse(QuantityBox.Text, out quantity) || quantity <= 0)
        {
            ShowErrorMessage("Количество должно быть положительным числом").Wait(); 
            return false;
        }

        return true;
    }

    private async void LoadMaterialBoxAsync()
    {
        using var context = new LopushokContext();

        var product = await context.Products
            .Include(p => p.Productmaterials)
                .ThenInclude(pm => pm.Material)
            .FirstOrDefaultAsync(p => p.Id == _productId);

        if (product != null)
        {
            if (product.Productmaterials.Any())
            {
                MaterialBox.ItemsSource = product.MaterialTitles.ToList();
            }
            else
            {
                MaterialBox.ItemsSource = new List<string>(); 
            }
        }
        else
        {
            MaterialBox.ItemsSource = new List<string>(); 
        }
    }

    private void BackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

}
