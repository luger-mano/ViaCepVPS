# 🔎 Consulta de CEP com ViaCEP — API .NET

---

## ✨ Visão Geral

Este projeto expõe uma **API REST** para consulta de endereços a partir de um **CEP**, consumindo os dados do [ViaCEP](https://viacep.com.br).  
Ele valida o CEP, chama o serviço externo e retorna uma resposta **padronizada**, com documentação automática via **Swagger**.

---

## 🚀 Funcionalidades

- ✔️ Consulta de endereço por UF | Cidade | Logradouro (`GET /api/endereco/{uf}/{cidade}/{logradouro}`)
- ✔️ Documentação interativa com **Swagger**
- ✔️ Tratamento de erros e respostas consistentes
- ✔️ Estrutura pensada para extensões (cache, retry)
- ✔️ Histórico de consultas

---

## 🧱 Arquitetura (resumo)

```
Cliente → Controller → Service → HttpClient → ViaCEP
                     ↘︎ Validação ↙︎
```

- **Controller**: expõe endpoints e retorna respostas HTTP
- **Service**: regra de negócio (validação)
- **HttpClient**: chamada ao ViaCEP (`https://viacep.com.br/ws/{uf}/{cidade}/{logradouro}/json/`)

---

## 🧰 Requisitos

- [.NET SDK 7.0+](https://dotnet.microsoft.com/download) (recomendado 8.0)
- Acesso à internet para consultar o ViaCEP

---

## ⚙️ Configuração

### appsettings.json (exemplo)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Program.cs (registro do HttpClient, exemplo)
```csharp
builder.Services.AddHttpClient<IViaCepService, ViaCepService>(client =>
{
    client.BaseAddress = new Uri("https://viacep.com.br");
}).AddPolicyHandler(GetRetryPolicy());
```
---

## ▶️ Como rodar localmente

### 1) Clonar o repositório
```bash
git clone https://github.com/luger-mano/ViaCepVps.git
cd ViaCepVps
```

### 2) Restaurar e compilar
```bash
dotnet restore
dotnet build
```

### 3) Executar a API
```bash
dotnet run
```

> Por padrão, a aplicação sobe em `http://localhost:5000` (HTTP) e/ou `https://localhost:5001` (HTTPS), dependendo do seu `launchSettings.json`.

---

## 🧪 Como testar (rápido e direto)

1. **Clone o projeto**  
2. **Execute** `dotnet run`  
3. **Acesse** `http://localhost:5000/swagger`  
4. **Teste os endpoints** pelo Swagger UI

### Teste com cURL
```bash
# Uf, Cidade, Logradouro válido (ex: SP/São Paulo/Paulista)
curl http://localhost:5000/api/endereco/SP/São Paulo/Paulista

# Uf, Cidade, Logradouro inválido (deve retornar 400)
curl http://localhost:5000/api/endereco/SPA/S/Pa
```

### Exemplo de resposta (sucesso – 200)
```json
[{
    "cep": "08190-461",
    "logradouro": "Viela Paulista",
    "complemento": "",
    "bairro": "Vila Itaim",
    "localidade": "São Paulo",
    "uf": "SP",
    "ibge": "3550308",
    "gia": "1004",
    "ddd": "11",
    "siafi": "7107"
  } + 49 resultados]
```

### Exemplo de resposta (erro – 204)
```json
{
  "code": "204",
  "message": "Cep não encontrado"
}
```

---

## 🌐 Endpoints

### `GET /api/endereco/{uf}/{cidade}/{logradouro}`
- **Descrição:** Consulta um endereço através de uf, cidade e logradouro.
- **Parâmetros de rota:**
  - `uf` (string): aceitar `SP` ou `sp`.
  - `cidade` (string): aceitar `São Paulo` ou `são paulo`.
  - `logradouro` (string): aceitar `Paulista` ou `paulista`.  
- **Códigos de resposta:**
  - `200 OK` — endereço encontrado
  - `204 Not Content` — requisição com sucesso retorno sem conteúdo
  - `400 Bad Request` — parametros inválidos

---

## ✅ Boas Práticas Implementáveis (opcional)

- **Retry** com [Polly](https://github.com/App-vNext/Polly)
- **Cache** de respostas (IMemoryCache) para reduzir chamadas ao ViaCEP
- **Log** (ILogger)

> Inclua conforme a necessidade do seu projeto.

---

## 🧷 Dicas de Validação de CEP

- Remover caracteres não numéricos: `Regex.Replace(cep, "[^0-9]", "")`
- Verificar tamanho: exatamente **8 dígitos**
- Tratar CEP inexistente: ViaCEP retorna `{ "erro": true }` para CEPs não encontrados — converta isso em `404 Not Found`.

---

## 🛡️ Tratamento de Erros (sugestão de middleware)

```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        // mapear exceções conhecidas para 400/404/503, etc.
        // retornar ProblemDetails ou payload padrão
    });
});
```

---

## 📄 Swagger / OpenAPI

- **URL**: `http://localhost:5000/swagger`
- Interaja com os endpoints, envie requisições e veja exemplos de payloads.

---

## 🧩 FAQ

**1) Posso enviar UF outros dados em Minúsculo?**  
Sim. é **normalizado** para caixa alta automaticamente.

---


Obrigado por ver até aqui. ;)
