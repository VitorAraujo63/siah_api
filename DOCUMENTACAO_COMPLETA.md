# Documentação Completa — Ecossistema SIAH + RAG API

> **Data de geração:** 03/06/2026  
> **Destinatário:** Equipe de Desenvolvimento de Rotas (Backend)  
> **Objetivo:** Fornecer todo o contexto técnico necessário para a criação das rotas que a RagAPI precisa consumir da SIAH API para buscar dados de consultas por CPF.

---

## Índice

1. [Visão Geral do Ecossistema](#1-visão-geral-do-ecossistema)
2. [Projeto 1 — SIAH API (siah_api)](#2-projeto-1--siah-api)
   - 2.1 [Arquitetura](#21-arquitetura)
   - 2.2 [Stack e Dependências](#22-stack-e-dependências)
   - 2.3 [Configuração e Banco de Dados](#23-configuração-e-banco-de-dados)
   - 2.4 [Estrutura de Pastas](#24-estrutura-de-pastas)
   - 2.5 [Entidades do Domínio](#25-entidades-do-domínio)
   - 2.6 [Controllers Existentes (Rotas)](#26-controllers-existentes-rotas)
   - 2.7 [Use Cases](#27-use-cases)
   - 2.8 [Repositórios (Output Adapters)](#28-repositórios-output-adapters)
   - 2.9 [Schema do Banco de Dados](#29-schema-do-banco-de-dados)
3. [Projeto 2 — RAG API (RagAPI)](#3-projeto-2--rag-api)
   - 3.1 [Arquitetura](#31-arquitetura)
   - 3.2 [Stack e Dependências](#32-stack-e-dependências)
   - 3.3 [Estrutura de Pastas](#33-estrutura-de-pastas)
   - 3.4 [Fluxo de Funcionamento (RAG Pipeline)](#34-fluxo-de-funcionamento-rag-pipeline)
   - 3.5 [DTOs Existentes](#35-dtos-existentes)
   - 3.6 [Services Existentes](#36-services-existentes)
   - 3.7 [Controller Existente](#37-controller-existente)
4. [Integração entre os Projetos](#4-integração-entre-os-projetos)
   - 4.1 [O Problema Atual](#41-o-problema-atual)
   - 4.2 [O que precisa ser criado na SIAH API](#42-o-que-precisa-ser-criado-na-siah-api)
5. [Especificação das Rotas que Precisam ser Criadas](#5-especificação-das-rotas-que-precisam-ser-criadas)
   - 5.1 [Rota: Buscar Paciente por CPF](#51-rota-buscar-paciente-por-cpf)
   - 5.2 [Rota: Buscar Consultas por CPF](#52-rota-buscar-consultas-por-cpf)
6. [Contratos de Dados (DTOs Esperados pela RagAPI)](#6-contratos-de-dados-dtos-esperados-pela-ragapi)
7. [Ambiente e URLs](#7-ambiente-e-urls)
8. [Regras de Negócio Importantes](#8-regras-de-negócio-importantes)
9. [Checklist para o Time de Rotas](#9-checklist-para-o-time-de-rotas)

---

## 1. Visão Geral do Ecossistema

O sistema é composto por **dois projetos .NET 8** independentes que se comunicam via HTTP:

```
┌─────────────────────────────────────────────┐
│                  RAG API                    │
│         (RagAPI — porta 5000 aprox.)        │
│                                             │
│  Input: CPF do paciente via GET             │
│  Processo: Busca dados na SIAH API          │
│            → Monta prompt                  │
│            → Envia ao LLM (Ollama/phi3)     │
│            → Retorna resumo estruturado     │
└──────────────┬──────────────────────────────┘
               │ HTTP GET (consultas + dados do paciente)
               ▼
┌─────────────────────────────────────────────┐
│                 SIAH API                    │
│         (siah_api — porta 7154 / 5110)      │
│                                             │
│  Arquitetura: Hexagonal                     │
│  Banco: PostgreSQL (Supabase)               │
│  Exposta via: ngrok (mulberry-carload-...)  │
└──────────────┬──────────────────────────────┘
               │ NpgsqlDataSource (direct SQL)
               ▼
┌─────────────────────────────────────────────┐
│           Supabase / PostgreSQL             │
│                                             │
│  Host: aws-1-sa-east-1.pooler.supabase.com  │
│  DB: postgres                               │
│  SSL: Require                               │
└─────────────────────────────────────────────┘
```

**Fluxo resumido:**
1. O front-end ou totem envia o **CPF** para a **RagAPI**.
2. A RagAPI chama duas rotas da **SIAH API**: uma para buscar os dados do paciente e outra para buscar as consultas por CPF.
3. A RagAPI monta um prompt com todo o histórico e envia ao **LLM local (Ollama + modelo phi3)**.
4. O LLM responde com um JSON estruturado (diagnósticos, sintomas, tratamentos, resumo clínico).
5. A RagAPI retorna ao solicitante um objeto contendo **dados do paciente + resumo gerado pela IA**.

---

## 2. Projeto 1 — SIAH API

**Caminho local:** `/Users/vitorh/RiderProjects/ApiFilmes/siah_api/`  
**Namespace raiz:** `SiahApi`  
**Descrição:** API do Sistema Integrado de Atendimento Hospitalar (SIAH). É o backend principal que gerencia pacientes, médicos, agendamentos, triagem, documentos e fila de atendimento.

### 2.1 Arquitetura

O projeto segue **Arquitetura Hexagonal (Ports & Adapters)**:

```
┌──────────────────────────────────────────────────┐
│                   Domain Layer                   │
│  Entities/  ← Modelos de domínio puro            │
│  Ports/Input/  ← Interfaces dos Use Cases        │
│  Ports/Output/ ← Interfaces dos Repositórios     │
└────────────────────────┬─────────────────────────┘
                         │
┌────────────────────────▼─────────────────────────┐
│                Application Layer                 │
│  UseCases/  ← Implementação dos casos de uso     │
│  DTOs/      ← Data Transfer Objects              │
│  Services/  ← Interfaces de serviços auxiliares  │
└────────────────────────┬─────────────────────────┘
                         │
┌────────────────────────▼─────────────────────────┐
│               Infrastructure Layer               │
│  Adapters/Input/  ← Controllers (ASP.NET)        │
│  Adapters/Output/ ← Repositórios (Npgsql/Supa.)  │
│  Services/        ← Impl. de serviços externos   │
│  Configuration/   ← DI e configurações           │
└──────────────────────────────────────────────────┘
```

### 2.2 Stack e Dependências

| Componente | Versão/Detalhe |
|---|---|
| Runtime | .NET 8.0 |
| Framework | ASP.NET Core Web API |
| ORM/DB Driver | Npgsql 8.0.3 (SQL direto, sem ORM) |
| Banco de Dados | PostgreSQL via Supabase |
| Documentação | Swashbuckle (Swagger) |
| Autenticação | JWT Bearer Token |
| Exposição Pública | ngrok (`mulberry-carload-example.ngrok-free.dev`) |

**Arquivo de projeto:** `siah_api/siah_api.csproj`
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.24" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.1" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
<PackageReference Include="Npgsql" Version="8.0.3" />
```

### 2.3 Configuração e Banco de Dados

**Arquivo:** `appsettings.json`

```json
{
  "Supabase": {
    "ConnectionStrings": {
      "DefaultConnection": "Host=aws-1-sa-east-1.pooler.supabase.com;Database=postgres;Username=postgres.bngwnknyxmhkeesoeizb;Password=postgressiah1;SSL Mode=Require;Trust Server Certificate=true"
    }
  },
  "Jwt": {
    "SecretKey": "F6a9Maj8mtTVRg13XEHfnA9wyEpVZ3ZmfKru+4BBzg3D8XaS2CQA+r4p/k9FIlinFBEJGNPZl/VbK9PHNaiFfQ==",
    "Issuer": "siah-api",
    "Audience": "siah-app",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 30
  },
  "FaceEmbedding": {
    "ServiceUrl": "http://localhost:5001/generate-embedding"
  }
}
```

**Como a conexão é registrada** (`DependencyInjection.cs`):
```csharp
var connectionString = configuration["Supabase:ConnectionStrings:DefaultConnection"];
services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
```

### 2.4 Estrutura de Pastas

```
siah_api/
├── Program.cs                        ← Entry point
├── appsettings.json                  ← Configurações (Supabase, JWT, CORS)
├── appsettings.Development.json      ← Configurações de desenvolvimento
├── siah_api.csproj                   ← Dependências do projeto
├── context/
│   └── context.md                    ← Especificação completa da API (SIAH_Especificacao_API_v1)
├── Domain/
│   ├── Entities/                     ← Entidades de domínio
│   │   ├── Agendamento.cs
│   │   ├── Documento.cs
│   │   ├── Especialidade.cs
│   │   ├── Medico.cs
│   │   ├── Paciente.cs
│   │   ├── SenhaAtendimento.cs
│   │   └── Triagem.cs
│   └── Ports/
│       ├── Input/                    ← Interfaces dos Use Cases
│       │   ├── IAgendamentoUseCase.cs
│       │   ├── IAuthUseCase.cs
│       │   ├── IBiometriaUseCase.cs
│       │   ├── IDocumentoUseCase.cs
│       │   ├── IEspecialidadeUseCase.cs
│       │   ├── IFilaUseCase.cs
│       │   ├── IHistoricoUseCase.cs
│       │   ├── IMedicoUseCase.cs
│       │   ├── IPacienteUseCase.cs
│       │   ├── IPerfilUseCase.cs
│       │   └── ITriagemUseCase.cs
│       └── Output/                   ← Interfaces dos Repositórios
│           ├── IAgendamentoRepository.cs
│           ├── IAuthRepository.cs
│           ├── IBiometriaRepository.cs
│           ├── IDocumentoRepository.cs
│           ├── IEspecialidadeRepository.cs
│           ├── IFilaRepository.cs
│           ├── IHistoricoRepository.cs
│           ├── IMedicoRepository.cs
│           ├── IPacienteRepository.cs
│           └── ITriagemRepository.cs
├── Application/
│   ├── DTOs/                         ← DTOs por módulo
│   │   ├── Agendamento/
│   │   ├── Auth/
│   │   ├── Biometria/
│   │   ├── Documento/
│   │   ├── Especialidade/
│   │   ├── Fila/
│   │   ├── Historico/
│   │   ├── Medico/
│   │   ├── Paciente/
│   │   ├── Perfil/
│   │   └── Triagem/
│   ├── Services/
│   │   └── IFaceEmbeddingService.cs
│   └── UseCases/                     ← Lógica de negócio
│       ├── AgendamentoUseCase.cs
│       ├── AuthUseCase.cs
│       ├── BiometriaUseCase.cs
│       ├── DocumentoUseCase.cs
│       ├── EspecialidadeUseCase.cs
│       ├── FilaUseCase.cs
│       ├── HistoricoUseCase.cs
│       ├── MedicoUseCase.cs
│       ├── PacienteUseCase.cs
│       ├── PerfilUseCase.cs
│       └── TriagemUseCase.cs
└── Infrastructure/
    ├── Adapters/
    │   ├── Input/                    ← Controllers HTTP
    │   │   ├── AgendamentosController.cs
    │   │   ├── AuthController.cs
    │   │   ├── BiometriaController.cs
    │   │   ├── BuscaController.cs
    │   │   ├── DocumentosController.cs
    │   │   ├── EspecialidadesController.cs
    │   │   ├── FilaController.cs
    │   │   ├── HistoricoController.cs
    │   │   ├── MedicosController.cs
    │   │   ├── PacientesController.cs
    │   │   ├── PerfilController.cs
    │   │   ├── TriagemController.cs
    │   │   └── UsuariosController.cs
    │   └── Output/                   ← Repositórios Supabase
    │       ├── SupabaseAgendamentoRepository.cs
    │       ├── SupabaseAuthRepository.cs
    │       ├── SupabaseBiometriaRepository.cs
    │       ├── SupabaseDocumentoRepository.cs
    │       ├── SupabaseEspecialidadeRepository.cs
    │       ├── SupabaseFilaRepository.cs
    │       ├── SupabaseHistoricoRepository.cs
    │       ├── SupabaseMedicoRepository.cs
    │       ├── SupabasePacienteRepository.cs
    │       └── SupabaseTriagemRepository.cs
    ├── Services/
    │   └── FaceEmbeddingServiceStub.cs
    └── Configuration/
        ├── DependencyInjection.cs
        └── SupabaseConfiguration.cs
```

### 2.5 Entidades do Domínio

#### Paciente (`Domain/Entities/Paciente.cs`)
Mapeado para a tabela `usuarios` no banco. Campos principais:

| Campo | Tipo C# | Coluna DB |
|---|---|---|
| Id | Guid | id (uuid, PK) |
| Nome | string | nome |
| Cpf | string | cpf (Unique) |
| Email | string? | email |
| Telefone | string? | telefone |
| DataNascimento | DateOnly? | data_nascimento |
| Genero | string? | genero |
| TipoSanguineo | string? | tipo_sanguineo |
| HospitalVinculado | string? | hospital_vinculado |
| Rg | string? | rg |
| CartaoSus | string? | cartao_sus |
| PossuiPlanoSaude | bool | possui_plano_saude |
| NomePlano | string? | nome_plano |
| Images | List\<string\> | images (_text) |
| Embedding | float[]? | embedding (_float8) |
| EmbeddingPath | string? | embedding_path |

#### Agendamento (`Domain/Entities/Agendamento.cs`)
Mapeado para a tabela `consultas` no banco:

| Campo | Tipo C# | Coluna DB |
|---|---|---|
| Id | Guid | id (uuid, PK) |
| IdUsuario | Guid | id_usuario |
| MedicoId | Guid | id_profissional |
| EspecialidadeId | Guid | — (mock) |
| Data | string | data_consulta (data) |
| Horario | string | data_consulta (hora) |
| Observacoes | string? | motivo_consulta |
| Status | string | calculado (scheduled/completed) |
| NumeroDaSenha | string? | — |
| MotivoCancelamento | string? | — |
| TipoCancelamento | string? | — |
| ReembolsoElegivel | bool | — |
| CanceladoEm | DateTime? | — |

### 2.6 Controllers Existentes (Rotas)

#### `AuthController` — Rota: `auth`
| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/auth/register` | Cadastro de novo paciente |
| POST | `/auth/login` | Login com CPF e senha |

#### `PacientesController` — Rota: `api/pacientes`
| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/pacientes/cadastrar` | Cadastrar paciente completo com biometria |
| POST | `/api/pacientes/reconhecer` | Reconhecimento facial |

> **⚠️ IMPORTANTE:** Não existe ainda uma rota `GET /Patients/cpf/{cpf}` nem `GET /api/Consultations/patient/{cpf}`. Essas são exatamente as rotas que precisam ser criadas para a integração com a RagAPI.

#### `AgendamentosController` — Rota: `appointments`
| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/appointments?userId={guid}` | Listar consultas (paginadas) |
| GET | `/appointments/upcoming?userId={guid}` | Próximas consultas |
| GET | `/appointments/{id}?userId={guid}` | Detalhe de uma consulta |
| POST | `/appointments?userId={guid}` | Agendar nova consulta |
| PATCH | `/appointments/{id}/reschedule?userId={guid}` | Reagendar consulta |
| DELETE | `/appointments/{id}/cancel?userId={guid}` | Cancelar consulta |

#### `HistoricoController` — Rota: `history`
| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/history/appointments?userId={guid}` | Histórico completo |
| GET | `/history/appointments/{id}?userId={guid}` | Detalhe do atendimento |
| GET | `/history/recent?userId={guid}` | Últimos 10 atendimentos |

#### `MedicosController` — Rota: `doctors`
| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/doctors` | Listar médicos |
| GET | `/doctors/{id}` | Perfil do médico |
| GET | `/doctors/{id}/availability` | Disponibilidade |
| POST | `/doctors/{id}/favorite` | Favoritar médico |
| DELETE | `/doctors/{id}/favorite` | Desfavoritar |
| GET | `/doctors/favorites?userId={guid}` | Médicos favoritos |

#### `EspecialidadesController` — Rota: `specialties`
| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/specialties` | Listar especialidades |
| GET | `/specialties/{id}/doctors` | Médicos por especialidade |

#### `FilaController` — Rota: `queue`
| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/queue/validate-totem` | Totem emite senha digital |
| GET | `/queue/my-ticket?userId={guid}` | Minha senha ativa |
| GET | `/queue/status/{ticketId}` | Status da senha |
| POST | `/queue/confirm-arrival?userId={guid}` | Confirmar presença |

#### `HistoricoController` — Rota: `history` (já listado acima)

#### `DocumentosController` — Rota: `documents`
| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/documents/certificates` | Listar atestados |
| GET | `/documents/certificates/{id}` | Detalhe do atestado |
| GET | `/documents/certificates/{id}/pdf` | Download em PDF |
| GET | `/documents/exams` | Listar exames |
| GET | `/documents/exams/{id}` | Detalhe do exame |
| GET | `/documents/exams/{id}/result` | Resultado do exame |
| GET | `/documents/prescriptions` | Listar receitas |
| GET | `/documents/prescriptions/active` | Receitas ativas |
| GET | `/documents/search` | Busca unificada |

#### `PerfilController` — Rota: `profile`
| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/profile?userId={guid}` | Dados completos do perfil |
| PUT | `/profile?userId={guid}` | Atualizar perfil completo |
| PATCH | `/profile/basic-info?userId={guid}` | Atualizar dados básicos |
| PATCH | `/profile/photo?userId={guid}` | Trocar foto |
| DELETE | `/profile/photo?userId={guid}` | Remover foto |

#### `BuscaController` — Rota: `search`
| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/search/doctors?q={termo}` | Busca full-text de médicos |

#### `TriagemController` — Rota: `triagem`
| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/triagem` | Registrar nova triagem |

#### `BiometriaController` — Rota: `biometria`
| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/biometria/register` | Registrar biometria facial |
| POST | `/biometria/verify` | Verificar biometria |

### 2.7 Use Cases

Cada Use Case é registrado como `Scoped` no DI e implementa uma interface de input port:

| Use Case | Interface | Responsabilidade |
|---|---|---|
| `AgendamentoUseCase` | `IAgendamentoUseCase` | CRUD de consultas/agendamentos |
| `AuthUseCase` | `IAuthUseCase` | Login, registro, JWT |
| `BiometriaUseCase` | `IBiometriaUseCase` | Reconhecimento facial |
| `DocumentoUseCase` | `IDocumentoUseCase` | Atestados, exames, receitas |
| `EspecialidadeUseCase` | `IEspecialidadeUseCase` | Listagem de especialidades |
| `FilaUseCase` | `IFilaUseCase` | Senhas de atendimento (totem) |
| `HistoricoUseCase` | `IHistoricoUseCase` | Histórico de atendimentos |
| `MedicoUseCase` | `IMedicoUseCase` | Dados e disponibilidade de médicos |
| `PacienteUseCase` | `IPacienteUseCase` | Cadastro e reconhecimento de pacientes |
| `PerfilUseCase` | `IPerfilUseCase` | Gerenciamento de perfil do usuário |
| `TriagemUseCase` | `ITriagemUseCase` | Registro de triagem clínica |

### 2.8 Repositórios (Output Adapters)

Todos usam `NpgsqlDataSource` com SQL direto (sem ORM):

| Repositório | Interface | Tabela Principal |
|---|---|---|
| `SupabasePacienteRepository` | `IPacienteRepository` | `usuarios` |
| `SupabaseAgendamentoRepository` | `IAgendamentoRepository` | `consultas` |
| `SupabaseHistoricoRepository` | `IHistoricoRepository` | `consultas` |
| `SupabaseAuthRepository` | `IAuthRepository` | `usuarios` |
| `SupabaseMedicoRepository` | `IMedicoRepository` | `profissionais` |
| `SupabaseEspecialidadeRepository` | `IEspecialidadeRepository` | — |
| `SupabaseFilaRepository` | `IFilaRepository` | `senhas` |
| `SupabaseDocumentoRepository` | `IDocumentoRepository` | `exames`, `vacinas` |
| `SupabaseTriagemRepository` | `ITriagemRepository` | `triagens` |
| `SupabaseBiometriaRepository` | `IBiometriaRepository` | `usuarios` (embedding) |

**Exemplo de query existente (SupabaseAgendamentoRepository):**
```sql
SELECT id, id_usuario, id_profissional, data_consulta, motivo_consulta
FROM consultas
WHERE id_usuario = @id_usuario
ORDER BY data_consulta DESC
LIMIT @per_page OFFSET @offset
```

**Exemplo: verificar paciente por CPF (SupabasePacienteRepository):**
```sql
SELECT EXISTS(SELECT 1 FROM usuarios WHERE cpf = @cpf)
```

### 2.9 Schema do Banco de Dados

#### Tabela: `usuarios`
| Coluna | Tipo | Constraints |
|---|---|---|
| id | uuid | PRIMARY KEY |
| cpf | text | UNIQUE |
| nome | text | Nullable |
| email | text | Nullable |
| senha | text | Nullable |
| telefone | varchar | Nullable |
| data_nascimento | date | Nullable |
| genero | varchar | Nullable |
| rg | varchar | Nullable |
| tipo_sanguineo | varchar | Nullable |
| hospital_vinculado | varchar | Nullable |
| medico_responsavel | varchar | Nullable |
| peso | numeric | Nullable |
| altura | numeric | Nullable |
| imc | numeric | Nullable |
| pressao_arterial | varchar | Nullable |
| frequencia_cardiaca | varchar | Nullable |
| alergias | text | Nullable |
| condicoes_cronicas | text | Nullable |
| cirurgias_anteriores | text | Nullable |
| medicamentos_em_uso | text | Nullable |
| historico_familiar | text | Nullable |
| possui_plano_saude | bool | Nullable |
| nome_plano | varchar | Nullable |
| numero_carteirinha | varchar | Nullable |
| validade_carteirinha | date | Nullable |
| nome_responsavel | varchar | Nullable |
| parentesco | varchar | Nullable |
| telefone_responsavel | varchar | Nullable |
| cartao_sus | varchar | Nullable |
| cnh | varchar | Nullable |
| cep | varchar | Nullable |
| rua | varchar | Nullable |
| numero | varchar | Nullable |
| complemento | varchar | Nullable |
| bairro | varchar | Nullable |
| cidade | varchar | Nullable |
| estado | varchar | Nullable |
| images | _text | Nullable (array de strings) |
| embeddings | jsonb | Nullable |
| embedding | _float8 | Nullable (array de floats) |
| embedding_path | text | Nullable |
| template_biometrico | bytea | Nullable |
| device_token | text | Nullable |
| criado_em | timestamp | Nullable |

#### Tabela: `consultas`
| Coluna | Tipo | Constraints |
|---|---|---|
| id | uuid | PRIMARY KEY |
| id_usuario | uuid | FK → usuarios.id |
| id_profissional | uuid | FK → profissionais.id |
| id_hospital | uuid | FK → hospitais.id |
| data_consulta | timestamp | NOT NULL |
| motivo_consulta | text | NOT NULL |
| diagnostico | text | NOT NULL |
| prescricao | text | Nullable |
| anotacoes_medicas | text | Nullable |

#### Tabela: `profissionais`
| Coluna | Tipo | Constraints |
|---|---|---|
| id | uuid | PRIMARY KEY |
| nome | text | NOT NULL |
| tipo_profissional | varchar | NOT NULL |
| crm_coren | text | Nullable |
| especialidade | text | Nullable |
| telefone | varchar | NOT NULL |
| email | text | NOT NULL |

#### Tabela: `hospitais`
| Coluna | Tipo | Constraints |
|---|---|---|
| id | uuid | PRIMARY KEY |
| nome_hospital | text | NOT NULL |
| cep | varchar | Nullable |
| rua | varchar | Nullable |
| numero | varchar | Nullable |
| complemento | varchar | Nullable |
| bairro | varchar | Nullable |
| cidade | varchar | Nullable |
| estado | varchar | Nullable |

#### Tabela: `exames`
| Coluna | Tipo | Constraints |
|---|---|---|
| id | uuid | PRIMARY KEY |
| id_usuario | uuid | NOT NULL |
| tipo_exame | text | NOT NULL |
| data_realizacao | date | NOT NULL |
| resultado_link | text | Nullable |
| nome_laboratorio | text | Nullable |

#### Tabela: `vacinas`
| Coluna | Tipo | Constraints |
|---|---|---|
| id | uuid | PRIMARY KEY |
| id_usuario | uuid | NOT NULL |
| nome_vacina | text | NOT NULL |
| data_aplicacao | date | NOT NULL |
| dose | text | NOT NULL |
| lote | text | NOT NULL |
| id_hospital | uuid | NOT NULL |
| id_profissional | uuid | NOT NULL |

#### Tabela: `senhas`
| Coluna | Tipo | Constraints |
|---|---|---|
| id | int8 | PRIMARY KEY (Identity) |
| cpf | text | NOT NULL |
| numero_senha | text | NOT NULL |
| especialidade | text | NOT NULL |
| local_chamada | text | Nullable |
| created_at | timestamptz | NOT NULL |
| status | text | Nullable |

#### Tabela: `triagens`
| Coluna | Tipo | Constraints |
|---|---|---|
| id | uuid | PRIMARY KEY |
| id_usuario | uuid | NOT NULL |
| queixa_principal | text | NOT NULL |
| pressao_arterial | text | Nullable |
| temperatura | text | Nullable |
| frequencia_cardiaca | int4 | Nullable |
| peso | numeric | Nullable |
| altura | numeric | Nullable |
| data_hora_triagem | timestamp | Nullable |

---

## 3. Projeto 2 — RAG API

**Caminho local:** `/Users/vitorh/Developer/rag_siah/RagAPI/`  
**Namespace raiz:** `RagAPI`  
**Repositório:** `JeannAlves12/RagAPI`  
**Descrição:** API de Recuperação Aumentada por Geração (RAG). Recebe um CPF, busca o histórico médico na SIAH API, e usa um LLM local para gerar um resumo clínico estruturado.

### 3.1 Arquitetura

Arquitetura simples de serviços (sem hexagonal):

```
RagController
     │
     ▼
SummaryService
     ├── HospitalService → SIAH API (HTTP)
     └── AiService → Ollama/LLM (HTTP)
```

### 3.2 Stack e Dependências

| Componente | Versão/Detalhe |
|---|---|
| Runtime | .NET 8.0 |
| Framework | ASP.NET Core Web API |
| LLM | Ollama (local, porta 11434) |
| Modelo LLM | phi3 |
| Documentação | Swashbuckle (Swagger) |
| CORS | AllowAll (configurado) |

**Arquivo de projeto:** `RagAPI/RagAPI.csproj`
```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
```

### 3.3 Estrutura de Pastas

```
RagAPI/
├── Program.cs                    ← Entry point
├── appsettings.json              ← Configurações básicas
├── RagAPI.csproj                 ← Dependências
├── Controllers/
│   └── RagController.cs          ← Endpoint GET /api/Rag/summary/{cpf}
├── Services/
│   ├── AiService.cs              ← Comunicação com Ollama (LLM)
│   ├── HospitalService.cs        ← Comunicação com SIAH API
│   └── SummaryService.cs         ← Orquestração: busca dados + gera resumo
└── Dtos/
    ├── PatientDto.cs             ← Dados básicos do paciente
    ├── ConsultationDto.cs        ← Dados de uma consulta individual
    ├── MedicalSummaryDto.cs      ← Output da IA (JSON estruturado)
    ├── PatientSummaryResponseDto.cs ← Resposta final (paciente + resumo)
    └── QuestionDto.cs            ← DTO auxiliar (não usado atualmente)
```

### 3.4 Fluxo de Funcionamento (RAG Pipeline)

```
1. GET /api/Rag/summary/{cpf}
         │
         ▼
2. SummaryService.GeneratePatientSummaryAsync(cpf)
         │
         ├── HospitalService.GetPatientAsync(cpf)
         │   └── GET https://localhost:7154/Patients/cpf/{cpf}
         │       └── Retorna: PatientDto (id, nome, cpf, data_nascimento)
         │
         ├── HospitalService.GetPatientConsultations(cpf)
         │   └── GET https://localhost:7154/api/Consultations/patient/{cpf}
         │       └── Retorna: List<ConsultationDto>
         │           (id, reason, finalDiagnosis, date, observations, medications)
         │
         ├── Monta StringBuilder com histórico de consultas
         │
         ├── AiService.AskModel(prompt)
         │   └── POST http://localhost:11434/api/generate
         │       ├── model: "phi3"
         │       ├── stream: false
         │       ├── format: "json"
         │       └── temperature: 0.2
         │       └── Retorna: MedicalSummaryDto (JSON)
         │           {
         │             "main_diagnoses": [],
         │             "recent_symptoms": [],
         │             "treatments": [],
         │             "clinical_summary": ""
         │           }
         │
         └── Retorna PatientSummaryResponseDto
             {
               "patient": { id, name, cpf, birthDate },
               "summary": { mainDiagnoses, recentSymptoms, treatments, clinicalSummary }
             }
```

### 3.5 DTOs Existentes

#### `PatientDto` — Dados do paciente esperados da SIAH API
```csharp
public class PatientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
}
```

> **Observação:** O campo `Id` é `int` na RagAPI, mas na SIAH API o id é `uuid`. A rota a ser criada deve retornar um objeto compatível. Ver [Seção 6](#6-contratos-de-dados-dtos-esperados-pela-ragapi).

#### `ConsultationDto` — Dados de cada consulta
```csharp
public class ConsultationDto
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;         // motivo_consulta
    public string FinalDiagnosis { get; set; } = string.Empty; // diagnostico
    public DateTime Date { get; set; }                          // data_consulta
    public string Observations { get; set; } = string.Empty;   // anotacoes_medicas
    public List<MedicationDto>? Medications { get; set; }      // medicamentos prescritos
}

public class MedicationDto
{
    public string Name { get; set; } = string.Empty;           // nome do medicamento
}
```

#### `MedicalSummaryDto` — Output estruturado da IA
```csharp
public class MedicalSummaryDto
{
    [JsonPropertyName("main_diagnoses")]
    public List<string> MainDiagnoses { get; set; } = new();   // diagnósticos principais

    [JsonPropertyName("recent_symptoms")]
    public List<string> RecentSymptoms { get; set; } = new();  // sintomas recentes

    [JsonPropertyName("treatments")]
    public List<string> Treatments { get; set; } = new();      // tratamentos/medicamentos

    [JsonPropertyName("clinical_summary")]
    public string ClinicalSummary { get; set; } = string.Empty; // resumo em até 3 frases
}
```

#### `PatientSummaryResponseDto` — Resposta final da RagAPI
```csharp
public class PatientSummaryResponseDto
{
    public PatientDto Patient { get; set; } = null!;
    public MedicalSummaryDto Summary { get; set; } = null!;
}
```

### 3.6 Services Existentes

#### `HospitalService` — Comunicação com a SIAH API
```csharp
// Busca dados do paciente por CPF
GET https://localhost:7154/Patients/cpf/{cpf}
→ Retorna: PatientDto? (null se não encontrado)

// Busca consultas do paciente por CPF
GET https://localhost:7154/api/Consultations/patient/{cpf}
→ Retorna: List<ConsultationDto> (lista vazia se nenhuma)
```

#### `AiService` — Comunicação com Ollama (LLM local)
```csharp
// Endpoint do Ollama
POST http://localhost:11434/api/generate
Body: { model: "phi3", prompt: "...", stream: false, format: "json", options: { temperature: 0.2 } }
→ Retorna: { response: "{ JSON estruturado }" }
// Timeout configurado: 5 minutos
```

#### `SummaryService` — Orquestrador principal
- Busca paciente → Se null, retorna null.
- Busca consultas → Se vazia, retorna paciente + mensagem padrão.
- Monta prompt com histórico completo de consultas.
- Envia ao LLM e parseia a resposta JSON.
- Retorna `PatientSummaryResponseDto`.

**Prompt enviado ao LLM:**
```
Você é um assistente médico especializado em análise de histórico clínico.

Sua tarefa é analisar o histórico de consultas de um paciente e extrair informações relevantes de forma estruturada.

Responda APENAS em JSON válido, sem explicações adicionais.

Formato obrigatório:
{
    "main_diagnoses": [],
    "recent_symptoms": [],
    "treatments": [],
    "clinical_summary": ""
}

Regras:
- "main_diagnoses": usar OBRIGATORIAMENTE o campo "Diagnóstico" do sistema.
- Extrair sintomas a partir dos motivos das consultas.
- Extrair tratamentos (medicamentos, repouso ou condutas).
- REGRA DE CONFLITO: "Medicamentos no Sistema" tem prioridade máxima.
- NÃO inventar informações.
- "clinical_summary": máximo 3 frases.
- Arrays devem ser planos (não aninhados).
```

### 3.7 Controller Existente

#### `RagController` — Rota: `api/Rag`
```
GET /api/Rag/summary/{cpf}
```

**Responses:**
| Status | Descrição |
|---|---|
| 200 OK | `PatientSummaryResponseDto` completo |
| 404 Not Found | `{ message: "Paciente não possui consultas." }` |
| 500 Internal | `{ error: "...", details: "..." }` |

**Exemplo de request:**
```
GET https://ragapi.local/api/Rag/summary/12345678901
```

**Exemplo de response (200):**
```json
{
  "patient": {
    "id": 1,
    "name": "João da Silva",
    "cpf": "12345678901",
    "birthDate": "1990-05-15T00:00:00"
  },
  "summary": {
    "main_diagnoses": ["Hipertensão Arterial Sistêmica", "Diabetes Mellitus Tipo 2"],
    "recent_symptoms": ["Cefaleia", "Tontura", "Polidipsia"],
    "treatments": ["Losartana 50mg", "Metformina 850mg", "Dieta hipossódica"],
    "clinical_summary": "Paciente com histórico de hipertensão e diabetes em acompanhamento regular. Última consulta evidenciou controle glicêmico inadequado. Ajuste de medicação recomendado."
  }
}
```

---

## 4. Integração entre os Projetos

### 4.1 O Problema Atual

A `HospitalService` da RagAPI está chamando duas URLs que **ainda não existem** na SIAH API:

```csharp
// Rota 1 — Buscar paciente por CPF (NÃO EXISTE NA SIAH API)
GET https://localhost:7154/Patients/cpf/{cpf}

// Rota 2 — Buscar consultas por CPF (NÃO EXISTE NA SIAH API)
GET https://localhost:7154/api/Consultations/patient/{cpf}
```

As rotas existentes na SIAH API buscam por **userId (Guid)**, mas a RagAPI só tem o **CPF** como identificador de entrada.

### 4.2 O que precisa ser criado na SIAH API

1. **Um novo Controller** (ex: `ConsultationsController`) ou endpoints adicionados em controllers existentes, expostos sem autenticação JWT (ou com API Key interna), que aceitem CPF como parâmetro.

2. **Dois novos endpoints:**
   - `GET /Patients/cpf/{cpf}` → retorna dados básicos do paciente
   - `GET /api/Consultations/patient/{cpf}` → retorna lista de consultas com medicamentos

3. **Opcionalmente:** Um novo Use Case ou extensão dos Use Cases existentes para consulta por CPF.

---

## 5. Especificação das Rotas que Precisam ser Criadas

### 5.1 Rota: Buscar Paciente por CPF

```
GET /Patients/cpf/{cpf}
```

**Parâmetros:**
| Parâmetro | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| cpf | string (route) | Sim | CPF do paciente (11 dígitos, somente números) |

**Query SQL a executar:**
```sql
SELECT id, nome, cpf, data_nascimento
FROM usuarios
WHERE cpf = @cpf
LIMIT 1
```

**Response 200 (PatientDto esperado pela RagAPI):**
```json
{
  "id": 1,
  "name": "João da Silva",
  "cpf": "12345678901",
  "birthDate": "1990-05-15T00:00:00"
}
```
> **Nota sobre o campo `id`:** A RagAPI usa `int` mas o banco usa `uuid`. Recomenda-se retornar o uuid como string ou adaptar o DTO.

**Response 404:**
```json
null
```
ou body vazio com status 404.

**Segurança:** Esta rota é consumida internamente pela RagAPI. Pode ser protegida com API Key no header ou liberada apenas para loopback/rede interna. **Não usar JWT de usuário.**

---

### 5.2 Rota: Buscar Consultas por CPF

```
GET /api/Consultations/patient/{cpf}
```

**Parâmetros:**
| Parâmetro | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| cpf | string (route) | Sim | CPF do paciente (11 dígitos, somente números) |

**Query SQL a executar:**
```sql
SELECT 
  c.id,
  c.motivo_consulta,
  c.diagnostico,
  c.data_consulta,
  c.anotacoes_medicas,
  c.prescricao
FROM consultas c
INNER JOIN usuarios u ON c.id_usuario = u.id
WHERE u.cpf = @cpf
ORDER BY c.data_consulta DESC
```

> **Sobre Medicamentos:** A `ConsultationDto` da RagAPI espera um campo `Medications` com lista de `{ Name }`. Por ora, o campo `prescricao` da tabela `consultas` é um texto livre. Recomenda-se retornar `prescricao` como um único item na lista ou parsear por vírgula/quebra de linha.

**Response 200 (List\<ConsultationDto\> esperado pela RagAPI):**
```json
[
  {
    "id": 1,
    "reason": "Dor de cabeça frequente e tontura",
    "finalDiagnosis": "Hipertensão Arterial Sistêmica",
    "date": "2026-05-20T10:30:00",
    "observations": "Paciente relata não ter medido pressão há 3 meses",
    "medications": [
      { "name": "Losartana 50mg" },
      { "name": "Hidroclorotiazida 25mg" }
    ]
  },
  {
    "id": 2,
    "reason": "Retorno — controle de pressão",
    "finalDiagnosis": "Hipertensão Arterial Sistêmica controlada",
    "date": "2026-06-01T09:00:00",
    "observations": "Pressão estável com medicação",
    "medications": [
      { "name": "Losartana 50mg" }
    ]
  }
]
```

**Response 404 (sem consultas):**
```json
[]
```
ou status 404 — a RagAPI trata ambos.

**Segurança:** Mesmas considerações da rota anterior (API Key interna ou loopback).

---

## 6. Contratos de Dados (DTOs Esperados pela RagAPI)

A `HospitalService` desserializa as respostas com `PropertyNameCaseInsensitive = true`, ou seja, **os nomes dos campos não precisam ter casing exato**, mas os nomes devem ser os mesmos:

### PatientDto esperado:
```json
{
  "id": <int ou uuid como string>,
  "name": "string",
  "cpf": "string (11 dígitos)",
  "birthDate": "datetime ISO 8601"
}
```

### ConsultationDto esperado:
```json
{
  "id": <int>,
  "reason": "string",           ← motivo_consulta
  "finalDiagnosis": "string",   ← diagnostico
  "date": "datetime ISO 8601",  ← data_consulta
  "observations": "string",     ← anotacoes_medicas
  "medications": [              ← parsear prescricao
    { "name": "string" }
  ]
}
```

---

## 7. Ambiente e URLs

| Ambiente | URL Base SIAH API | Observação |
|---|---|---|
| Local (https) | `https://localhost:7154` | URL hardcoded na HospitalService atual |
| Local (http) | `http://localhost:5110` | Porta alternativa |
| ngrok (público) | `https://mulberry-carload-example.ngrok-free.dev` | Tunnel ativo no momento |
| Produção (previsto) | `https://api.siah.com.br/v1` | Conforme especificação |

**URL da RagAPI:**
- Local: porta padrão do .NET (5000/7000 aprox.)
- Swagger da RagAPI: `/swagger/index.html`

**LLM (Ollama):**
- URL: `http://localhost:11434/api/generate`
- Modelo: `phi3`
- Timeout: 5 minutos

---

## 8. Regras de Negócio Importantes

1. **CPF é o identificador primário** usado pela RagAPI. A SIAH API usa UUIDs internamente, mas deve expor endpoints públicos com CPF.

2. **Nenhum dado médico é inventado pela IA.** O LLM recebe apenas o que está no banco e deve extrair sem inventar.

3. **Medicamentos do campo `prescricao`** têm prioridade sobre o texto de anamnese do médico no prompt da IA.

4. **Se o paciente não tiver consultas**, a RagAPI retorna o objeto do paciente com `clinical_summary: "Paciente sem histórico de consultas registradas."` (não retorna 404).

5. **Se o paciente não existir** (CPF não encontrado), a RagAPI retorna 404.

6. **As novas rotas de CPF não devem exigir JWT de usuário**, pois a RagAPI não tem como obter um token JWT de usuário — ela é uma API interna. Usar API Key no header ou confiar no IP/rede interna.

7. **Padrão de nomes de campos:** A RagAPI usa `PropertyNameCaseInsensitive = true`, então tanto `camelCase` quanto `PascalCase` funcionam.

---

## 9. Checklist para o Time de Rotas

Estas são as tarefas que precisam ser implementadas na **SIAH API** para que a **RagAPI** funcione corretamente:

### Controller/Endpoint 1 — Paciente por CPF
- [ ] Criar endpoint `GET /Patients/cpf/{cpf}` na SIAH API
- [ ] Implementar query SQL: `SELECT id, nome, cpf, data_nascimento FROM usuarios WHERE cpf = @cpf`
- [ ] Retornar DTO com campos: `id`, `name`, `cpf`, `birthDate`
- [ ] Retornar 404 se CPF não encontrado
- [ ] Não exigir JWT (ou usar API Key interna)
- [ ] Registrar no DI (se novo Use Case criado)

### Controller/Endpoint 2 — Consultas por CPF
- [ ] Criar endpoint `GET /api/Consultations/patient/{cpf}` na SIAH API
- [ ] Implementar query SQL com JOIN entre `consultas` e `usuarios` filtrando por CPF
- [ ] Retornar lista de DTOs com campos: `id`, `reason`, `finalDiagnosis`, `date`, `observations`, `medications[]`
- [ ] Parsear campo `prescricao` em lista de `{ name }` (separar por vírgula ou `\n`)
- [ ] Retornar lista vazia `[]` (ou 404) se não houver consultas
- [ ] Não exigir JWT (ou usar API Key interna)

### Opcional — Melhorias
- [ ] Adicionar tabela de medicamentos separada para evitar parsear texto livre
- [ ] Atualizar `HospitalService` na RagAPI para usar a URL ngrok ou variável de ambiente
- [ ] Configurar `appsettings.json` da RagAPI com a URL base da SIAH API
- [ ] Adicionar tratamento de erro para quando o Ollama estiver offline
- [ ] Adicionar logs estruturados para rastrear chamadas entre as APIs

---

*Documentação gerada automaticamente a partir da leitura completa do código-fonte dos dois projetos.*
