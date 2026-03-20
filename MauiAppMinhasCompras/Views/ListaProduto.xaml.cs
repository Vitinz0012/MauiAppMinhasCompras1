using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new();

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            foreach (var item in tmp)
                lista.Add(item);
        }
        catch (Exception ex)
        {
            await this.DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Clicked(object? sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            await this.DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue ?? string.Empty;

            lista.Clear();

            List<Produto> tmp = await App.Db.Search(q);

            foreach (var item in tmp)
                lista.Add(item);
        }
        catch (Exception ex)
        {
            await this.DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Clicked_1(object? sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"O total é {soma:C}";

        await this.DisplayAlertAsync("Total dos Produtos", msg, "OK");
    }

    private async void MenuItem_Clicked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is not MenuItem selecionado)
                return;

            if (selecionado.BindingContext is not Produto p)
                return;

            bool confirm = await this.DisplayAlertAsync(
                "Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await this.DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            if (e.SelectedItem is not Produto p)
                return;

            await Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });

            lst_produtos.SelectedItem = null; // limpa seleção
        }
        catch (Exception ex)
        {
            await this.DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            if (e.CurrentSelection[0] is not Produto p)
                return;

            await Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });

            if (sender is CollectionView cv)
                cv.SelectedItem = null; // limpa seleção
        }
        catch (Exception ex)
        {
            await this.DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }
}