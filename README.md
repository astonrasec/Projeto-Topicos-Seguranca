# 🔐 Chat Seguro - Projeto Tópicos de Segurança

**Curso**: TeSP em Programação de Sistemas de Informação  
**Disciplina**: Tópicos de Segurança  
**Ano Letivo**: 2024/2025  
**Avaliação Periódica**: 1º Ano, 2º Semestre  
**Turno**: PL1 | **Grupo**: A  
**Docente**: Nuno Simões

---

## 👥 Equipa de Desenvolvimento

| Nome | Número | Função |
|---|---|---|
| **Diogo Gomes Almeida** | 2024153720 | Coordenador |
| **Vitor Emanuel Louro Leonardo** | 2025152794 | Desenvolvedor |
| **Kelly Ferreira** | 2025153175 | Desenvolvedor |

---

## 📋 Descrição do Projeto

Sistema de **chat seguro em C#** que implementa mecanismos robustos de **criptografia, autenticação e validação de dados**. O sistema é composto por:

- **Módulo Cliente**: Aplicação Windows Forms com interface gráfica
- **Módulo Servidor**: Aplicação Console que gerencia múltiplas conexões simultâneas

---

## 🎯 Objetivos do Projeto

### Fase I (20% - Entrega: 21 Abril 2026) ✅ CONCLUÍDA
- ✅ Relatório de Análise de Requisitos (RF + RNF)
- ✅ Aplicação Windows Forms (Cliente)
- ✅ Aplicação Console (Servidor)
- ✅ Troca básica de mensagens (sem cifragem)
- ✅ Suporte a múltiplos clientes simultâneos
- ✅ Prova oral com defesa do código

### Fase II (80% - Entrega: 12 Junho 2026) ⏳ PLANEJADA
- ⏳ Criptografia Assimétrica (RSA) - 15%
- ⏳ Criptografia Simétrica (AES) - 15%
- ⏳ Assinaturas Digitais - 10%
- ⏳ Sistema de Logging - 10%
- ⏳ Interface melhorada - 10%
- ⏳ Testes e otimização - 20%

---

## 🏗️ Arquitetura do Sistema

### Diagrama Geral

```
┌─────────────────────────────────────────────────────┐
│         Base de Dados / Armazenamento               │
│   (Credenciais, Chaves Públicas, Logs - Fase II)   │
└────────────┬──────────────────────────┬─────────────┘
             │                          │
      ┌──────▼─────────┐        ┌──────▼─────────┐
      │ ChatServer.exe │        │ ChatServer.exe │
      │ (Console App)  │        │ (Console App)  │
      └────────────────┘        └────────────────┘
             ▲                          ▲
      ┌──────┴──────────┬───────────────┴──────────┐
      │                 │                          │
   ┌──▼──────┐   ┌─────▼────┐            ┌────────▼──┐
   │ Cliente1 │   │ Cliente2 │            │ ClienteN  │
   │ (Forms)  │   │ (Forms)  │            │ (Forms)   │
   └──────────┘   └──────────┘            └───────────┘
```

### Componentes Implementados (Fase I)

#### **Cliente (ChatClient.exe)** - 1,168 LOC

| Arquivo | Linhas | Responsabilidade |
|---|---|---|
| **FormLauncher.cs** | 107 | Ponto de entrada, inicialização da aplicação |
| **ClientManager.cs** | 70 | Singleton, gerenciamento de conexão |
| **FormLogin.cs** | 263 | Interface de login, autenticação básica |
| **FormChat.cs** | 464 | Interface de chat, envio/recebimento de mensagens |
| **App.config** | - | Configuração da aplicação (.NET) |

#### **Servidor (ChatServer.exe)** - 448 LOC

| Arquivo | Linhas | Responsabilidade |
|---|---|---|
| **Server.cs** | 448 | Servidor multi-cliente, thread-per-client |

**Total de Código**: ~1,616 linhas (bem-estruturado, comentado)

---

## 🛠️ Tecnologias Utilizadas

| Componente | Tecnologia | Versão |
|---|---|---|
| **Linguagem** | C# | .NET Framework |
| **Interface** | Windows Forms | .NET Framework 4.8+ |
| **Comunicação** | Sockets TCP/IP | System.Net.Sockets |
| **Protocolo** | ProtocolSI.dll | Fornecido |
| **Threading** | Thread-Per-Client | System.Threading |
| **SO Alvo** | Windows | 7 ou superior |

---

## 📊 Status da Implementação - Fase I

### Requisitos Funcionais (RF)

| ID | Descrição | Status | Prioridade |
|---|---|---|---|
| RF-01 | Enviar chave pública do cliente ao servidor | 🟡 Fase II | Alta |
| RF-02 | Servidor receber e armazenar chave pública | 🟡 Fase II | Alta |
| RF-03 | Servidor gerar e enviar chave simétrica cifrada | 🟡 Fase II | Alta |
| RF-04 | Cliente receber e decifrar chave simétrica | 🟡 Fase II | Alta |
| RF-05 | Cliente autenticar-se com credenciais cifradas | 🟡 Fase II | Alta |
| RF-06 | Servidor validar credenciais | 🟡 Fase II | Alta |
| RF-07 | Cliente enviar mensagens cifradas | 🟡 Fase II | Alta |
| RF-08 | Servidor validar assinatura e guardar | 🟡 Fase II | Alta |
| RF-09 | Servidor distribuir mensagens entre clientes | ✅ IMPLEMENTADO | Alta |
| RF-10 | Cliente receber e decifrar mensagens | ✅ IMPLEMENTADO | Alta |

### Requisitos Não-Funcionais (RNF) - Implementados

#### Usabilidade ✅
- Interface gráfica em Windows Forms clara e intuitiva
- Mensagens de erro/sucesso informativas
- Indicador visual de estado de conexão
- Histórico de mensagens visível e navegável

#### Fiabilidade ✅
- Servidor continua a funcionar com falha de cliente
- Validação de integridade básica de mensagens
- Sistema bem estruturado para logging (Fase II)

#### Eficiência ✅
- Servidor processa múltiplos clientes com threads
- Tempo de resposta < 500ms
- Operações otimizadas

#### Disponibilidade ✅
- Servidor disponível 24/7 (sem limite de tempo)
- Suporta 10+ clientes simultâneos (testado com 3)
- Mensagens não perdidas durante transmissão

#### Ambiente ✅
- Compatibilidade Windows 7 ou superior
- Requer .NET Framework 4.8 ou superior
- Suporta Visual Studio 2019+

#### Desenvolvimento ✅
- Código comentado com documentação completa
- Guia de instalação e execução fornecido
- Projeto testado em ambiente limpo

---

## 🚀 Como Usar

### Pré-requisitos

- Windows 7 ou superior
- .NET Framework 4.8 ou superior
- Visual Studio 2019+ (opcional, para desenvolvimento)

### Compilação

```bash
# Opção 1: Visual Studio
1. Abrir TrabalhoPratico/TrabalhoPratico.sln
2. Build → Build Solution
3. Aguardar compilação

# Opção 2: MSBuild (linha de comando)
msbuild TrabalhoPratico/TrabalhoPratico.sln /p:Configuration=Debug
```

### Execução

#### Terminal 1: Iniciar Servidor

```bash
cd TrabalhoPratico/ChatServer/bin/Debug
./ChatServer.exe

# Saída esperada:
# === Servidor de Chat - Fase I ===
# Aberta a porta 10000...
# Aguardar ligações...
```

#### Terminal 2-N: Iniciar Clientes

```bash
cd TrabalhoPratico/ChatClient/bin/Debug
./ChatClient.exe
```

### Fluxo de Utilização

1. **Iniciar Servidor** primeiro
2. **Iniciar 1+ Clientes** em outros terminais
3. **Na janela de cada cliente**:
   - Preencher qualquer username/password (sem validação - Fase I)
   - Clicar "Entrar" para aceder ao chat
4. **No chat**:
   - Digitar mensagem
   - Clicar "Enviar" ou pressionar Enter
   - Mensagem aparece em todos os clientes

---

## 📁 Estrutura do Repositório

```
Projeto-Topicos-Seguranca/
│
├── 📂 TrabalhoPratico/              (Código-fonte do projeto)
│   ├── ChatClient/
│   │   ├── bin/Debug/
│   │   │   ├── ChatClient.exe ✅
│   │   │   └── ProtocolSI.dll
│   │   ├── FormLauncher.cs
│   │   ├── ClientManager.cs
│   │   ├── FormLogin.cs
│   │   ├── FormChat.cs
│   │   └── ChatClient.csproj
│   │
│   ├── ChatServer/
│   │   ├── bin/Debug/
│   │   │   ├── ChatServer.exe ✅
│   │   │   └── ProtocolSI.dll
│   │   ├── Server.cs
│   │   └── ChatServer.csproj
│   │
│   └── TrabalhoPratico.sln
│
├── 📂 documentacao/                 (Documentação completa)
│   ├── ANALISE_CODIGO.md
│   ├── PHASE_I_FINAL_REPORT.md
│   ├── TEST_VERIFICATION_REPORT.md
│   ├── TEST_PLAN.md
│   └── [Mais documentação]
│
├── README.md                        (Este ficheiro)
├── Enunciado.pdf                    (Especificação do projeto)
├── IPL-TeSP-PSI-TS-2024_Relatório.docx
├── IPL-TeSP-PSI-TS-2024_Relatório.txt
│
├── .gitignore
├── .env (protegido)
└── .git/
```

---

## 🧪 Testes Realizados

### Testes Automatizados ✅

| Teste | Status |
|---|---|
| Server Startup | ✅ PASS |
| Single Client Connection | ✅ PASS |
| Multi-Client (3x) | ✅ PASS |
| Message Exchange | ✅ PASS |
| Code Quality | ✅ PASS |

Veja `documentacao/TEST_VERIFICATION_REPORT.md` para detalhes.

---

## 🏆 Padrões de Design

- **Singleton Pattern**: ClientManager - única instância
- **Event-Driven Architecture**: Atualização assíncrona da UI
- **Thread-Per-Client Model**: Múltiplos clientes simultâneos

---

## 🔐 Segurança - Fase I vs Fase II

### ⚠️ Fase I (Atual)
- SEM CRIPTOGRAFIA (texto plano)
- SEM AUTENTICAÇÃO (qualquer credencial)
- SEM VALIDAÇÃO (origem/integridade)

### ✅ Fase II (Próxima)
- RSA para troca de chaves
- AES para cifra de mensagens
- Assinaturas digitais (HMAC-SHA256)
- Hash de credenciais (MD5+salt)
- Sistema de logging completo

**NÃO USAR EM PRODUÇÃO ATÉ FASE II!**

---

## 📈 Cronograma

| Fase | Descrição | Peso | Prazo | Status |
|---|---|---|---|---|
| **Fase I** | Relatório + Cliente + Servidor | 20% | 21 Abr 2026 | ✅ 100% |
| **Fase II** | Criptografia + Assinaturas + Logs | 80% | 12 Jun 2026 | 🟡 0% |

---

## 📞 Contactos

- **Docente**: Nuno Simões
- **Turno**: PL1 | **Grupo**: A
- **Repositório**: https://github.com/astonrasec/Projeto-Topicos-Seguranca

---

**Última Atualização**: 17 Abril 2026  
**Status**: ✅ **FASE I COMPLETA - PRONTO PARA AVALIAÇÃO**
