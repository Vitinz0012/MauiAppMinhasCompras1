using System.Collections.ObjectModel;
using System.Linq;
using MauiAppMinhasCompras.Models;

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
        await CarregarLista();
    }

    private async Task CarregarLista()
    {
        try
        {
            lista.Clear();
            var tmp = await App.Db.GetAll();

            foreach (var item in tmp)
                lista.Add(item);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NovoProduto());
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        string q = e.NewTextValue ?? "";

        lista.Clear();

        var tmp = await App.Db.Search(q);

        foreach (var item in tmp)
            lista.Add(item);
    }

    private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);
        await DisplayAlertAsync("Total", $"Total: {soma:C}", "OK");
    }

    private async void picker_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        string categoria = picker_categoria.SelectedItem?.ToString();

        lista.Clear();

        var dados = await App.Db.GetAll();

        if (categoria != "Todos")
            dados = dados.Where(p => p.Categoria == categoria).ToList();

        foreach (var item in dados)
            lista.Add(item);
    }

    private async void Relatorio_Clicked(object sender, EventArgs e)
    {
        var relatorio = lista
            .GroupBy(p => p.Categoria)
            .Select(g => $"{g.Key}: {g.Sum(x => x.Total):C}");

        string msg = string.Join("\n", relatorio);

        await DisplayAlertAsync("Relatório", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        if (sender is MenuItem m && m.BindingContext is Produto p)
        {
            bool confirm = await DisplayAlertAsync("Confirmar", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
    }

    private async void lst_produtos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;

        var p = e.CurrentSelection[0] as Produto;

        await Navigation.PushAsync(new EditarProduto { BindingContext = p });

        ((CollectionView)sender).SelectedItem = null;
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        if (sender is RefreshView rv)
        {
            await CarregarLista();
            rv.IsRefreshing = false;
        }
    }
}