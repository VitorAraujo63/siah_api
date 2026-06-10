using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SiahApi.Infrastructure.Swagger;

public class AtualizarPerfilExampleFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionName = context.MethodInfo.Name;
        var controllerName = context.MethodInfo.DeclaringType?.Name;

        if (controllerName != "PerfilController" || actionName != "AtualizarDadosBasicos")
            return;

        var example = new OpenApiObject
        {
            ["nome"] = new OpenApiString("João da Silva"),
            ["telefone"] = new OpenApiString("(11) 99999-9999"),
            ["cep"] = new OpenApiString("01310-100"),
            ["rua"] = new OpenApiString("Av. Paulista"),
            ["numero"] = new OpenApiString("1000"),
            ["bairro"] = new OpenApiString("Bela Vista"),
            ["cidade"] = new OpenApiString("São Paulo"),
            ["estado"] = new OpenApiString("SP")
        };

        if (operation.RequestBody?.Content.TryGetValue("application/json", out var mediaType) == true)
        {
            mediaType.Example = example;
        }
    }
}

public class AtualizarPerfilCompletoExampleFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionName = context.MethodInfo.Name;
        var controllerName = context.MethodInfo.DeclaringType?.Name;

        if (controllerName != "PerfilController" || actionName != "Atualizar")
            return;

        var example = new OpenApiObject
        {
            ["nome"] = new OpenApiString("João da Silva"),
            ["email"] = new OpenApiString("joao@email.com"),
            ["telefone"] = new OpenApiString("(11) 99999-9999"),
            ["dataNascimento"] = new OpenApiString("1990-01-15"),
            ["genero"] = new OpenApiString("Masculino"),
            ["tipoSanguineo"] = new OpenApiString("O+"),
            ["rg"] = new OpenApiString("12.345.678-9"),
            ["cartaoSus"] = new OpenApiString("123456789012345"),
            ["cnh"] = new OpenApiString("12345678901"),
            ["cep"] = new OpenApiString("01310-100"),
            ["rua"] = new OpenApiString("Av. Paulista"),
            ["numero"] = new OpenApiString("1000"),
            ["bairro"] = new OpenApiString("Bela Vista"),
            ["cidade"] = new OpenApiString("São Paulo"),
            ["estado"] = new OpenApiString("SP"),
            ["possuiPlanoSaude"] = new OpenApiBoolean(true),
            ["nomePlano"] = new OpenApiString("Unimed"),
            ["numeroCarteirinha"] = new OpenApiString("9876543210"),
            ["validadeCarteirinha"] = new OpenApiString("2026-12-31"),
            ["nomeResponsavel"] = new OpenApiString("Maria da Silva"),
            ["parentesco"] = new OpenApiString("Mãe"),
            ["telefoneResponsavel"] = new OpenApiString("(11) 98888-7777")
        };

        if (operation.RequestBody?.Content.TryGetValue("application/json", out var mediaType) == true)
        {
            mediaType.Example = example;
        }
    }
}
