using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Habilita o suporte a arquivos estáticos (HTML, JS, CSS)
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configura a API para buscar a interface dentro da pasta wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// ---------------------------------------------------------
// PONTE DE COMUNICAÇÃO: TELA -> SISTEMA OPERACIONAL
// ---------------------------------------------------------

// Rota que o seu JavaScript vai chamar para executar uma ação
app.MapPost("/api/executar", (RequisicaoHardware req) =>
{
    try
    {
        // Aqui o C# abre um terminal invisível no Linux e executa a ação
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{req.Comando}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        string resultado = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return Results.Ok(new { Sucesso = true, Retorno = resultado });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// O Back-end rodará localmente na porta 5000
app.Run("http://localhost:5000");

// Modelo para receber os dados do Javascript
class RequisicaoHardware
{
    public string Comando { get; set; } = string.Empty;
}