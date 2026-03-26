using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new();
    private CollectionView? lst_produtos;

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos?.ItemsSource = lista;
    }

    private void InitializeComponent()
    {
        // Tenta resolver o elemento nomeado "lst_produtos" caso o registro via XAML não tenha ocorrido
        try
        {
            lst_produtos = (CollectionView?)FindByName("lst_produtos");
        }
        catch
        {
            lst_produtos = null;
        }
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await CarregarLista();
    }

    private async Task CarregarLista()
    {
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            foreach (var item in tmp)
                lista.Add(item);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Clicked(object? sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new NovoProduto());
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue ?? string.Empty;

            SetRefreshing(true);

            lista.Clear();

            List<Produto> tmp = await App.Db.Search(q);

            foreach (var item in tmp)
                lista.Add(item);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        }
        finally
        {
            SetRefreshing(false);
        }
    }

    private async void ToolbarItem_Clicked_1(object? sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"O total é {soma:C}";

        await DisplayAlertAsync("Total dos Produtos", msg, "OK");
    }

    private async void MenuItem_Clicked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is not MenuItem selecionado)
                return;

            if (selecionado.BindingContext is not Produto p)
                return;

            bool confirm = await DisplayAlertAsync(
                "Tem certeza?",
                $"Remover {p.Descricao}?",
                "Sim",
                "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
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

            await Navigation.PushAsync(new EditarProduto
            {
                BindingContext = p,
            });

            if (sender is CollectionView cv)
                cv.SelectedItem = null;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        if (sender is not RefreshView rv)
            return;

        try
        {
            await CarregarLista();
        }
        finally
        {
            rv.IsRefreshing = false;
        }
    }

    private void SetRefreshing(bool value)
    {
        Element? element = lst_produtos;

        while (element != null)
        {
            if (element is RefreshView rv)
            {
                rv.IsRefreshing = value;
                return;
            }

            element = element.Parent;
        }
    }
}