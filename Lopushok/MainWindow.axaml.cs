using Avalonia.Controls;
using Lopushok.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;


namespace Lopushok;

public partial class MainWindow : Window
{
    public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        LoadProducts();
        LoadComboBox();
    }

    private async void LoadProducts()
    {
        using var context = new LopushokContext();

        var allProducts = await context.Products
                                       .Include(product => product.Producttype)
                                       .Include(product => product.Productmaterials) 
                                       .ToListAsync(); 

        Products.Clear();

        foreach (var product in allProducts)
        {
            Products.Add(product);
        }



        // Сортировка 
        var sortedProducts = allProducts.AsQueryable();

        switch (SortBox.SelectedIndex)
        {
            case 1:
                sortedProducts = sortedProducts.OrderBy(product => product.Title); //по назв.
                break;
            case 2:
                sortedProducts = sortedProducts.OrderByDescending(product => product.Title);
                break;
            case 3:
                sortedProducts = sortedProducts.OrderBy(product => product.Productionworkshopnumber); //по цеху
                break;
            case 4:
                sortedProducts = sortedProducts.OrderByDescending(product => product.Productionworkshopnumber);
                break;
            case 5:
                sortedProducts = sortedProducts.OrderBy(product => product.Mincostforagent); // по мин.стоим.
                break;
            case 6:
                sortedProducts = sortedProducts.OrderByDescending(product => product.Mincostforagent);
                break;
            default:
                sortedProducts = sortedProducts.OrderBy(product => product.Id);
                break;
        }


        // Фильтрация по типу продукта
        if (FilterBox.SelectedItem is string productTypeTitle && productTypeTitle != "Все типы")
        {
            var productType = await context.Producttypes.FirstOrDefaultAsync(product => product.Title == productTypeTitle);

            if (productType != null)
            {
                sortedProducts = sortedProducts.Where(product => product.Producttypeid == productType.Id);
            }
        }

        ProductsBox.ItemsSource = sortedProducts.ToList();
    }

    private void SearchBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        LoadProducts();
    }

    private void SortBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        LoadProducts();
    }

    private void FilterBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        LoadProducts();
    }

    private async void LoadComboBox()
    {
        using var context = new LopushokContext();

        var productTypes = await context.Producttypes
                                         .Select(product => product.Title)
                                         .ToListAsync();

        productTypes.Add("Все типы");

        FilterBox.ItemsSource = productTypes.OrderBy(product => product != "Все типы"); 
    }
    private void AddProduct_Click(object? sender, RoutedEventArgs e)
    {
        var addProductWindow = new AddProduct();
        addProductWindow.Show();
    }


}