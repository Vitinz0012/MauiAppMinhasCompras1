using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
    public EditarProduto()
    {
        InitializeComponent();
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (BindingContext is not Produto produtoAnexado)
            {
                throw new Exception("Erro ao carregar o produto.");
            }

            if (string.IsNullOrWhiteSpace(txt_descricao.Text))
            {
                throw new Exception("Preencha a descrição.");
            }

            if (!double.TryParse(txt_quantidade.Text, out double quantidade))
            {
                throw new Exception("Quantidade inválida.");
            }

            if (!double.TryParse(txt_preco.Text, out double preco))
            {
                throw new Exception("Preço inválido.");
            }

            Produto p = new Produto
            {
                Id = produtoAnexado.Id,
                Descricao = txt_descricao.Text,
                Quantidade = quantidade,
                Preco = preco
            };

            await App.Db.Update(p);
            await DisplayAlertAsync("Sucesso!", "Registro Atualizado", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }
}