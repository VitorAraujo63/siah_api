using SiahApi.Application.DTOs.Perfil;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;
using Microsoft.AspNetCore.Http;

namespace SiahApi.Application.UseCases;

public class PerfilUseCase : IPerfilUseCase
{
    private readonly IAuthRepository _authRepository;

    public PerfilUseCase(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<PerfilResponse> ObterAsync(Guid userId)
    {
        var paciente = await _authRepository.ObterPorIdAsync(userId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        return MapearParaResponse(paciente);
    }

    public async Task<PerfilResponse> AtualizarAsync(Guid userId, AtualizarPerfilRequest request)
    {
        var paciente = await _authRepository.ObterPorIdAsync(userId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        if (request.Nome is not null) paciente.Nome = request.Nome;
        if (request.Telefone is not null) paciente.Telefone = request.Telefone;
        if (request.Email is not null) paciente.Email = request.Email;
        if (request.DataNascimento is not null) paciente.DataNascimento = request.DataNascimento;
        if (request.Genero is not null) paciente.Genero = request.Genero;
        if (request.Cep is not null) paciente.Cep = request.Cep;
        if (request.Rua is not null) paciente.Rua = request.Rua;
        if (request.Numero is not null) paciente.Numero = request.Numero;
        if (request.Bairro is not null) paciente.Bairro = request.Bairro;
        if (request.Cidade is not null) paciente.Cidade = request.Cidade;
        if (request.Estado is not null) paciente.Estado = request.Estado;

        var atualizado = await _authRepository.AtualizarAsync(paciente);
        return MapearParaResponse(atualizado);
    }

    public async Task<PerfilResponse> AtualizarDadosBasicosAsync(Guid userId, AtualizarDadosBasicosRequest request)
    {
        var paciente = await _authRepository.ObterPorIdAsync(userId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        if (request.Nome is not null) paciente.Nome = request.Nome;
        if (request.Telefone is not null) paciente.Telefone = request.Telefone;
        if (request.Cep is not null) paciente.Cep = request.Cep;
        if (request.Rua is not null) paciente.Rua = request.Rua;
        if (request.Numero is not null) paciente.Numero = request.Numero;
        if (request.Bairro is not null) paciente.Bairro = request.Bairro;
        if (request.Cidade is not null) paciente.Cidade = request.Cidade;
        if (request.Estado is not null) paciente.Estado = request.Estado;

        var atualizado = await _authRepository.AtualizarAsync(paciente);
        return MapearParaResponse(atualizado);
    }

    public Task<AtualizarFotoResponse> AtualizarFotoAsync(Guid userId, IFormFile photo)
    {
        return Task.FromResult(new AtualizarFotoResponse { FotoUrl = string.Empty });
    }

    public Task RemoverFotoAsync(Guid userId) => Task.CompletedTask;

    public Task SolicitarExclusaoAsync(Guid userId) => Task.CompletedTask;

    private static PerfilResponse MapearParaResponse(Paciente paciente) => new()
    {
        Id = paciente.Id,
        Nome = paciente.Nome,
        Cpf = paciente.Cpf,
        Email = paciente.Email,
        Telefone = paciente.Telefone,
        DataNascimento = paciente.DataNascimento,
        Genero = paciente.Genero,
        TipoSanguineo = paciente.TipoSanguineo,
        HospitalVinculado = paciente.HospitalVinculado,
        Endereco = new EnderecoDto
        {
            Cep = paciente.Cep,
            Rua = paciente.Rua,
            Numero = paciente.Numero,
            Bairro = paciente.Bairro,
            Cidade = paciente.Cidade,
            Estado = paciente.Estado
        },
        PlanoSaude = new PlanoSaudeDto
        {
            Possui = paciente.PossuiPlanoSaude,
            Nome = paciente.NomePlano,
            NumeroCarteirinha = paciente.NumeroCarteirinha,
            Validade = paciente.ValidadeCarteirinha
        },
        Responsavel = new ResponsavelDto
        {
            Nome = paciente.NomeResponsavel,
            Parentesco = paciente.Parentesco,
            Telefone = paciente.TelefoneResponsavel
        }
    };
}
