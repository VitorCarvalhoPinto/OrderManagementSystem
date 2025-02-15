# Order Management System

Este é o projeto Order Management System, uma aplicação .NET 8 que utiliza PostgreSQL como banco de dados. Este documento fornece instruções sobre como configurar e executar o projeto localmente usando Docker.

## Pré-requisitos

- [Docker](https://www.docker.com/get-started) instalado em sua máquina.
- [Docker Compose](https://docs.docker.com/compose/install/) instalado.

## Configuração

1. Clone o repositório para sua máquina local:

```bash
git clone https://github.com/seu-usuario/ordermanagementsystem.git
cd ordermanagementsystem
```

2. Certifique-se de que o arquivo `docker-compose.yml` está configurado corretamente. Ele deve conter os serviços `ordermanagementsystem` e `OrderManagementSystemDB`.

3. Será necessário configurar suas secrets no projeto OrderManagementSystem, seguindo este modelo:

```bash
{
    "Kestrel:Certificates:Development:Password": "b102d280-d020-4ad8-b586-d5934d80c2f2",
    "ConnectionStrings:DefaultConnection": "Host=OrderManagementSystemDB;Port=5432;Database=OrderManagementSystem;Username=postgres;Password=1234",
    "AzureServiceBus": {
        "QueueName": "nome-da-queue",
        "ConnectionString": "StringDeConexão"
    }
}
```

## Executando o Projeto

1. No diretório raiz do projeto, execute o seguinte comando para iniciar os contêineres Docker:
docker-compose up --build

2. O Docker Compose irá construir as imagens e iniciar os contêineres. O serviço da aplicação estará disponível nas portas `8080` e `8081`, e o banco de dados PostgreSQL estará disponível na porta `5432`.

## Configuração do Banco de Dados

O banco de dados PostgreSQL será configurado automaticamente com as seguintes credenciais:

- **Usuário:** postgres
- **Senha:** 1234
- **Banco de Dados:** OrderManagementSystem

## Acessando a Aplicação

- A aplicação estará disponível em `http://localhost:8080` e `http://localhost:8081`.

## Parando os Contêineres

Para parar e remover os contêineres, execute: docker-compose down


## Problemas Comuns

- **Erro de conexão com o banco de dados:** Verifique se o contêiner do banco de dados está em execução e se as credenciais estão corretas.
- **Portas em uso:** Certifique-se de que as portas `8080`, `8081` e `5432` não estão sendo usadas por outros serviços.

## Contribuição

Se você deseja contribuir com este projeto, por favor, faça um fork do repositório e envie um pull request com suas alterações.
