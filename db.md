## Table `usuarios`

### Columns

| Name | Type | Constraints |
|------|------|-------------|
| `cpf` | `text` |  Unique |
| `images` | `_text` |  Nullable |
| `embeddings` | `jsonb` |  Nullable |
| `rg` | `varchar` |  Nullable |
| `data_nascimento` | `date` |  Nullable |
| `genero` | `varchar` |  Nullable |
| `estado_civil` | `varchar` |  Nullable |
| `nacionalidade` | `varchar` |  Nullable |
| `naturalidade` | `varchar` |  Nullable |
| `telefone` | `varchar` |  Nullable |
| `telefone_secundario` | `varchar` |  Nullable |
| `cep` | `varchar` |  Nullable |
| `rua` | `varchar` |  Nullable |
| `numero` | `varchar` |  Nullable |
| `complemento` | `varchar` |  Nullable |
| `bairro` | `varchar` |  Nullable |
| `cidade` | `varchar` |  Nullable |
| `estado` | `varchar` |  Nullable |
| `hospital_vinculado` | `varchar` |  Nullable |
| `medico_responsavel` | `varchar` |  Nullable |
| `tipo_sanguineo` | `varchar` |  Nullable |
| `peso` | `numeric` |  Nullable |
| `altura` | `numeric` |  Nullable |
| `imc` | `numeric` |  Nullable |
| `pressao_arterial` | `varchar` |  Nullable |
| `frequencia_cardiaca` | `varchar` |  Nullable |
| `alergias` | `text` |  Nullable |
| `condicoes_cronicas` | `text` |  Nullable |
| `cirurgias_anteriores` | `text` |  Nullable |
| `medicamentos_em_uso` | `text` |  Nullable |
| `historico_familiar` | `text` |  Nullable |
| `template_biometrico` | `bytea` |  Nullable |
| `observacoes_medicas` | `text` |  Nullable |
| `nome_plano` | `varchar` |  Nullable |
| `numero_carteirinha` | `varchar` |  Nullable |
| `validade_carteirinha` | `date` |  Nullable |
| `nome_responsavel` | `varchar` |  Nullable |
| `parentesco` | `varchar` |  Nullable |
| `telefone_responsavel` | `varchar` |  Nullable |
| `cartao_sus` | `varchar` |  Nullable |
| `cnh` | `varchar` |  Nullable |
| `id` | `uuid` | Primary |
| `criado_em` | `timestamp` |  Nullable |
| `possui_plano_saude` | `bool` |  Nullable |
| `senha` | `text` |  Nullable |
| `device_token` | `text` |  Nullable |
| `nome` | `text` |  Nullable |
| `email` | `text` |  Nullable |
| `embedding_path` | `text` |  Nullable |
| `embedding` | `_float8` |  Nullable |

## Table `profissionais`

### Columns

| Name | Type | Constraints |
|------|------|-------------|
| `id` | `uuid` | Primary |
| `nome` | `text` |  |
| `tipo_profissional` | `varchar` |  |
| `crm_coren` | `text` |  Nullable |
| `especialidade` | `text` |  Nullable |
| `telefone` | `varchar` |  |
| `email` | `text` |  |

## Table `hospitais`

### Columns

| Name | Type | Constraints |
|------|------|-------------|
| `nome_hospital` | `text` |  |
| `cep` | `varchar` |  Nullable |
| `rua` | `varchar` |  Nullable |
| `numero` | `varchar` |  Nullable |
| `complemento` | `varchar` |  Nullable |
| `bairro` | `varchar` |  Nullable |
| `cidade` | `varchar` |  Nullable |
| `estado` | `varchar` |  Nullable |
| `id` | `uuid` | Primary |

## Table `exames`

### Columns

| Name | Type | Constraints |
|------|------|-------------|
| `id_usuario` | `uuid` |  |
| `tipo_exame` | `text` |  |
| `data_realizacao` | `date` |  |
| `resultado_link` | `text` |  Nullable |
| `nome_laboratorio` | `text` |  Nullable |
| `id` | `uuid` | Primary |

## Table `vacinas`

### Columns

| Name | Type | Constraints |
|------|------|-------------|
| `id` | `uuid` | Primary |
| `id_usuario` | `uuid` |  |
| `nome_vacina` | `text` |  |
| `data_aplicacao` | `date` |  |
| `dose` | `text` |  |
| `lote` | `text` |  |
| `id_hospital` | `uuid` |  |
| `id_profissional` | `uuid` |  |

## Table `consultas`

### Columns

| Name | Type | Constraints |
|------|------|-------------|
| `id_usuario` | `uuid` |  |
| `id_profissional` | `uuid` |  |
| `id_hospital` | `uuid` |  |
| `data_consulta` | `timestamp` |  |
| `motivo_consulta` | `text` |  |
| `diagnostico` | `text` |  |
| `prescricao` | `text` |  Nullable |
| `anotacoes_medicas` | `text` |  Nullable |
| `id` | `uuid` | Primary |

## Table `senhas`

### Columns

| Name | Type | Constraints |
|------|------|-------------|
| `id` | `int8` | Primary Identity |
| `cpf` | `text` |  |
| `numero_senha` | `text` |  |
| `especialidade` | `text` |  |
| `local_chamada` | `text` |  Nullable |
| `created_at` | `timestamptz` |  |
| `status` | `text` |  Nullable |

## Table `triagens`

### Columns

| Name | Type | Constraints |
|------|------|-------------|
| `pressao_arterial` | `text` |  Nullable |
| `temperatura` | `text` |  Nullable |
| `frequencia_cardiaca` | `int4` |  Nullable |
| `peso` | `numeric` |  Nullable |
| `id_usuario` | `uuid` |  |
| `altura` | `numeric` |  Nullable |
| `queixa_principal` | `text` |  |
| `id` | `uuid` | Primary |
| `data_hora_triagem` | `timestamp` |  Nullable |

