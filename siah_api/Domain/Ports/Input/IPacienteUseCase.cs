using SiahApi.Application.DTOs.Paciente;

namespace SiahApi.Domain.Ports.Input;

public interface IPacienteUseCase
{
    Task<CadastrarPacienteResponse> CadastrarAsync(CadastrarPacienteRequest request);
    Task<ReconhecerPacienteResponse> ReconhecerAsync(ReconhecerPacienteRequest request);
}
