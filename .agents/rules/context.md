---
trigger: always_on
---

# Faça sempre a aplicação aplicando os padrões de Arquitetura Hexagonal, para que fique uma API bem construida.

# Evite deixar comentarios espalhados pelo código.

# Evite a utilização de emojis.

# Faça uma API bem construida nos padroes de projetos com arquitetura hexagonal.


## "Documentação de Integração_SIAH API C#"
###Documentação de Integração: SIAH API
C#
O frontend SIAH (React) se comunica com a API C# exclusivamente via HTTP POST, enviando
o payload em JSON.
Configuração: O baseURL da API C# deve ser configurado na variável de
ambiente VITE_API_CSHARP_URL no frontend.
1. Cadastro de Paciente
Rota: POST /api/pacientes/cadastrar
Objetivo: Inserir um novo paciente no banco de dados com todos os dados cadastrais,
embedding facial e caminhos das imagens no Storage.
O que enviamos (Payload)
JSON
{
"nome": "João da Silva"
,
"cpf": "123.456.789-00"
,
"email": "joao@email.com"
,
"telefone": "(11) 99999-9999"
,
"data
nascimento": "1990-01-15"
,
_
"genero": "Masculino"
,
"tipo
_
sanguineo": "O+"
,
"hospital
_
vinculado": "Hospital Central"
,
"rg": "12.345.678-9"
,
"cartao
sus": "123456789012345"
,
_
"cnh": "12345678901"
,
"cep": "01310-100"
,
"rua": "Av. Paulista"
,
"numero": "1000"
,
"bairro": "Bela Vista"
,
"cidade": "São Paulo"
,
"estado": "SP"
,
"possui
_plano
saude": false,
_
"nome
_plano": ""
,
"numero
carteirinha": ""
,_
"validade
carteirinha": ""
,
_
"nome
_
responsavel": ""
,
"parentesco": ""
,
"telefone
_
responsavel": ""
,
"images": ["12345678900/frente.jpg"
,
"12345678900/esquerda.jpg"
,
"12345678900/cima.jpg"],
"embedding": [0.1234,
-0.5678, 0.9012,
"
...
"],
"embedding_path": "12345678900/"
,
"temp_
file": "data:image/jpeg;base64,/9j/4AAQSkZJRgAB...
"
"12345678900/direita.jpg"
,
}
Observações sobre os dados:
cpf: Chega formatado com máscara (123.456.789-00).
images: São os caminhos das fotos já salvas no Supabase Storage (bucket faces).
embedding: Array de 512 floats gerado pelo modelo VGG-Face via DeepFace (deve
ser salvo na coluna embedding).
temp_file: Foto frontal em Base64 usada para gerar o embedding (pode ser
descartada após processamento).
Opcionais: Podem chegar como "" ou null.
O que você precisa fazer
1. Verificar se o cpf já existe na tabela usuarios.
2. Fazer um INSERT na tabela usuarios com todos os campos recebidos.
3. Salvar o embedding (array de floats) na coluna correspondente.
4. Descartar o campo temp_file.
O que você nos retorna
Status Descrição do Retorno
200 /
201
400 500 Sucesso. Retornar o objeto do paciente com pelo menos id, nome, cpf,
embedding_path e images.
Erro. CPF já cadastrado na base.
Erro. Falha ao inserir no banco.
Exemplo de Resposta (Sucesso):
JSON
{
"id": "uuid-gerado"
,
"nome": "João da Silva"
,
"cpf": "123.456.789-00"
,
"embedding_path": "12345678900/"
,
"images": ["12345678900/frente.jpg"
,
"
...
"]
}
2. Reconhecimento Facial (Match 1:N)
Rota: POST /api/pacientes/reconhecer
Objetivo: Identificar o paciente comparando fotos capturadas pela webcam com os
embeddings armazenados no banco.
O que enviamos (Payload)
JSON
{
"images": [
"data:image/jpeg;base64,/9j/4AAQ...
"
,
"data:image/jpeg;base64,/9j/4BBQ...
"
,
"data:image/jpeg;base64,/9j/4CCQ...
"
,
"data:image/jpeg;base64,/9j/4DDQ...
"
]
}
Observações sobre os dados:
Sempre enviamos 4 fotos em sequência: frente, esquerda, direita e cima.
O reconhecimento pode ser feito usando apenas a primeira imagem (images[0]) ou
usando todas as 4 para maior precisão (decisão do backend).
Threshold de distância cosseno recomendado: 0.60 (quanto menor, mais parecido).
O que você precisa fazer
1. Gerar o embedding facial da imagem recebida (via DeepFace VGG-Face ou
equivalente).
2. Fazer SELECT id, nome, cpf, embedding FROM usuarios WHERE
embedding IS NOT NULL.
3. Calcular a distância cosseno entre o embedding recebido (A) e cada embedding do
banco (B):
dista
ˆ
ncia=1−∥A∥∥B∥A⋅B
4. Retornar o paciente correspondente se a menor distância encontrada for < 0.60.
O que você nos retorna
Statu
s
Descrição do Retorno
200 401 Sucesso. Match encontrado (ver exemplo
abaixo).
Falha. Rosto não reconhecido na base de dados.
Exemplo de Resposta (Sucesso):
JSON
{
"sucesso": true,
"paciente": {
"id": "uuid-do-paciente"
,
"nome": "João da Silva"
,
"cpf": "123.456.789-00"
,
"distancia": 0.32
}
}
3. Triagem
Rota: POST /api/triagem/registrar
Objetivo: Registrar os sinais vitais e a queixa principal de um paciente identificado (busca
baseada no CPF).
O que enviamos (Payload)
JSON
{
"cpf
_paciente": "123.456.789-00"
,
"queixa
_principal": "Dor de cabeça intensa há 2 dias"
,
"peso": "75.5"
,
"altura": "1.80"
,
"temperatura": "36.5"
,
"pressao
arterial": "120/80"
,
_
"frequencia
cardiaca": "80"
_
}
Observações sobre os dados:

cpf_paciente: Chega formatado com máscara (123.456.789-00).
Valores numéricos: Chegam como string já com ponto decimal (ex: "75.5").
Formatos específicos: pressao_arterial vem no formato "120/80"
.
Vazios: Sinais vitais não preenchidos podem chegar vazios ou null (salvar como null
no banco).
queixa_principal: Campo obrigatório.
O que você precisa fazer
1. Buscar o id do paciente na tabela usuarios onde cpf = cpf_paciente.
2. Se não for encontrado, retornar erro 404.
3. Inserir na tabela triagens com o id_usuario encontrado e os dados vitais.
O que você nos retorna
Status Descrição do Retorno
200 /
201
404 500 Sucesso. Triagem registrada. Retornar os dados criados (ver
exemplo).
Erro. Paciente não encontrado pelo CPF.
Erro. Falha ao inserir no banco.
Exemplo de Resposta (Sucesso):
JSON
{
"status": "sucesso"
,
"data": {
"id": "uuid-da-triagem"
,
"id
_
usuario": "uuid-do-paciente"
,
"queixa
_principal": "Dor de cabeça intensa há 2 dias"
,
"peso": "75.5"
,
"altura": "1.80"
,
"temperatura": "36.5"
,
"pressao
arterial": "120/80"
,
_
"frequencia
cardiaca": "80"
_
}
}
Resumo Técnico
Tabela de Rotas
/api/pacientes/cadas
trar
/api/pacientes/recon
hecer
/api/triagem/registr
ar
Rota Método Finalidade
POST Cadastrar novo paciente com embedding facial
POST Identificar paciente via match facial 1:N
POST Salvar sinais vitais e queixa principal vinculados ao
paciente
Tecnologias no Frontend
Core: React 19 + Vite
Validação Facial: face-api.js (client-side, antes de enviar)
Captura: react-webcam (imagens em image/jpeg Base64)
IA: Embeddings via DeepFace (VGG-Face, 512 dimensões). Nota: Atualmente no
backend Python, precisará ser reimplementado no C#.
Estrutura do Supabase
Tabela usuarios: Dados do paciente, embedding (float[]), images (text[]),
embedding_path (text).
Tabela triagens: Sinais vitais vinculados via id_usuario.
Bucket Storage (faces): Fotos organizadas no formato
{cpf_sem_mascara}/{posicao}.jpg.

## "documentacao"
###Aqui está o texto completo contido no documento fornecido:

Documentação de Integração: API de Biometria

Fala, dev! Para o microsserviço do Totem rodar liso integrado com o banco de dados, vou precisar que você construa 3 endpoints no seu backend C#.

O meu Agente Local (WinForms em background) vai se comunicar com você exclusivamente via HTTP POST, enviando o payload em JSON.

Abaixo estão os contratos detalhados do que eu envio e do que eu preciso que você retorne, além da regra de negócio que a sua API precisa processar.

1. Endpoint: Cadastro de Biometria
Rota: POST /api/biometria/cadastrar

Objetivo: Vincular o byte[] de uma digital recém-capturada a um CPF que já existe no banco.

O que eu envio (Payload)

JSON
{
  "cpf": "12345678900",
  "template_biometrico": "TUIHZE1BMEDU3FHUOLIMORR..." // String em Base64
}
O que você precisa fazer

Receber o template_biometrico (Base64) e converter para um array de bytes usando Convert.FromBase64String.

Fazer um UPDATE na tabela usuarios do Supabase, salvando esse byte[] na coluna template_biometrico onde o cpf for igual ao enviado.

O que você me retorna

Status 200 OK: Se o UPDATE der certo.

Status 404 Not Found: Se o CPF não existir na base.

2. Endpoint: Identificação Biométrica (Match 1:N)
Rota: POST /api/biometria/identificar

Objetivo: Receber uma digital avulsa e varrer o banco de dados para descobrir de quem é.

O que eu envio (Payload)

JSON
{
  "digital_capturada": "TU1HZE1BME DU3FHU011MORR..." // String em Base64
}
O que você precisa fazer

Instalar a biblioteca DigitalPersona (DPFP) no seu backend C#.

Converter a digital_capturada de Base64 para byte[].

Fazer um SELECT cpf, nome, template_biometrico FROM usuarios WHERE template_biometrico IS NOT NULL no Supabase.

Usar o DPFP.Verification.Verification para iterar (foreach) sobre os resultados do banco, comparando o byte[] que eu mandei com os byte[] do banco.

O que você me retorna

Sucesso - match encontrado:

Status 200 OK: Quando achar o match, retornando os dados do paciente.

JSON
{
  "cpf": "12345678900",
  "nome": "João da Silva"
}
* **Falha - nenhum match:**
  * **Status 401 Unauthorized ou 404 Not Found:** Se terminar o loop e a digital não bater com ninguém.

---

### 3. Endpoint: Cadastro de Novos Usuários (Pré-Biometria)
* **Rota:** POST /api/usuarios
* **Objetivo:** Inserir os dados cadastrais básicos de um paciente que ainda não tem registro (vem da tela do Totem).

**O que eu envio (Payload)**
```json
{
  "nome": "João da Silva",
  "cpf": "12345678900",
  "email": "joao@email.com" // Pode ser nulo/vazio
}
O que você precisa fazer

Fazer um INSERT na tabela usuarios.

Tratar com ON CONFLICT (cpf) DO UPDATE SET nome = EXCLUDED.nome caso o paciente já exista mas esteja atualizando o cadastro. O campo template_biometrico ficará nulo por enquanto.

O que você me retorna

Status 200 OK ou 201 Created: Operação bem-sucedida.

Status 500: Caso dê erro no banco.