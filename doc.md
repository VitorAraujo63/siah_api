# Documentação dos Endpoints Criados/Atualizados

Esta documentação consolida as modificações e novas rotas implementadas para atender às necessidades do sistema (conforme os arquivos em `@siah_api/context`). A arquitetura utilizada segue os padrões da **Arquitetura Hexagonal** (Clean Architecture), com divisões claras entre _Controllers_ (Input Adapters), _Use Cases_ (Input Ports) e _Repositories_ (Output Ports).

## 1. Consultas (`/Consultations`)

**Controlador:** `ConsultationsController`

- **`GET /Consultations/?cpf={cpf}&pesquisa={pesquisa}`**
  - **Objetivo:** Listar as consultas de um paciente pelo CPF, com filtro opcional de pesquisa pelo `motivo_consulta` (usando `ILIKE "%pesquisa%"` no banco de dados).
  - **Retorno:** Lista de consultas, incluindo as anotações, prescrições (formatadas como array de medicamentos), e dados do Médico e Hospital vinculados.

- **`POST /Consultations`**
  - **Objetivo:** Cadastrar uma nova consulta.
  - **Payload:**
    ```json
    {
      "idPacient": "uuid",
      "idDoctor": "uuid",
      "idHospital": "uuid",
      "reason": "string",
      "finalDiagnosis": "string",
      "date": "2026-06-02T14:47:00",
      "observations": "string",
      "medications": "string"
    }
    ```

## 2. Exames (`/Exams`)

**Controlador:** `ExamsController`

- **`GET /Exams/?cpf={cpf}&pesquisa={pesquisa}`**
  - **Objetivo:** Listar exames de um paciente pelo CPF, com filtro opcional de pesquisa pelo `tipo_exame` (usando `ILIKE "%pesquisa%"` no banco).
  - **Retorno:** Lista de exames contendo `id`, `exam` (tipo do exame), `dateExam`, `pdfUrl` e `nameLaboratory`.

- **`POST /Exams`**
  - **Objetivo:** Registrar um novo exame para o paciente.
  - **Payload:**
    ```json
    {
      "idPacient": "uuid",
      "exam": "string",
      "dateExam": "2026-04-29",
      "pdfUrl": "string",
      "nameLaboratory": "string"
    }
    ```

## 3. Hospitais (`/Hospitals`)

**Controlador:** `HospitalsController`

- **`GET /Hospitals`**
  - **Objetivo:** Retornar a lista de todos os hospitais disponíveis no banco de dados.
  - **Retorno:** Lista de hospitais com `id`, `nomeHospital`, `cep`, `rua`, `numero`, `bairro`, `cidade`, `estado`, etc.

## 4. Detalhes do Paciente (`/Pacient`)

**Controlador:** `PacientController`

- **`GET /Pacient/{cpf}`**
  - **Objetivo:** Buscar todos os dados completos de um paciente a partir do CPF informado.
  - **Retorno:** Retorna um objeto detalhado com informações como nome, email, CPF, caminhos de imagens (frente, direita, esquerda, cima), dados médicos (peso, altura, alergias) e dados do responsável.

## 5. Triagens (`/Screenings`)

**Controlador:** `ScreeningsController`

- **`GET /Screenings/?cpf={cpf}&pesquisa={pesquisa}`**
  - **Objetivo:** Listar o histórico de triagens do paciente. Possui filtro opcional de pesquisa pela `queixa_principal` (usando `ILIKE "%pesquisa%"` no banco de dados).
  - **Retorno:** Lista de triagens (Pressão, Temperatura, Peso, Altura, Batimentos, Queixa e Data).

- **`POST /Screenings`**
  - **Objetivo:** Cadastrar uma nova triagem a partir do CPF do paciente (o UUID é resolvido internamente pelo UseCase/Repository).
  - **Payload:**
    ```json
    {
      "cpf": "string",
      "bloodPressure": "string",
      "temperature": "string",
      "heartRate": 80,
      "weight": 75.5,
      "height": 1.80,
      "complaint": "string",
      "dateScreening": "2026-06-04T20:02:00"
    }
    ```

## 6. Vacinas (`/api/vacinas`)

**Controlador:** `VacinaController` (Já existente, foi modificado)

- **`GET /api/vacinas/?cpf={cpf}&pesquisa={pesquisa}`**
  - **Objetivo:** Listar vacinas pelo CPF.
  - **Modificação:** Foi adicionado o parâmetro opcional de pesquisa/search na Query para filtrar pelo `nome_vacina` (usando `ILIKE "%pesquisa%"`). Além disso, a rota foi aprimorada com `LEFT JOIN` para agora retornar também os blocos aninhados contendo as informações completas do **Doutor** e do **Hospital** em cada vacina. A modificação foi aplicada verticalmente (Portas Input/Output, UseCase e Repository).

## 7. Pacientes (`/api/pacientes`)

**Controlador:** `PacientesController` (Já existente, foi modificado)

- **`POST /api/pacientes/cadastrar`**
  - **Objetivo:** Inserir um novo paciente no banco de dados.
  - **Modificação:** O C# assumiu a responsabilidade que antes era do Python. Agora ele recebe o `tempFile` (a foto frontal do paciente em Base64 enviada pelo frontend), utiliza o modelo VGG-Face embarcado e gera automaticamente o array de 512 floats (`embedding`) para salvar na coluna do banco, permitindo que o `/reconhecer` consiga efetuar a checagem correta.

---

> Todas as modificações respeitam rigorosamente a **Arquitetura Hexagonal**. Foram implementadas as portas correspondentes (`IConsultaUseCase`, `IConsultaRepository`, etc.), os `UseCases` para orquestração da regra de negócios e os adaptadores de banco de dados específicos usando `Supabase` + `NpgsqlDataSource` no Output. A injeção de dependências foi toda devidamente mapeada em `DependencyInjection.cs`.
