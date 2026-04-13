# 🔐 Projeto Tópicos de Segurança - Chat Seguro em C#

Projeto prático da disciplina de **Tópicos de Segurança** do Curso Técnico Superior Profissional de Programação de Sistemas de Informação (PSI-TS) do IPL.

## 📋 Descrição do Projeto

Desenvolvimento de um **sistema de chat seguro** em C# que implementa mecanismos robustos de criptografia, autenticação e validação de dados. O sistema é composto por:

- **Módulo Cliente**: Aplicação Windows Forms com interface gráfica
- **Módulo Servidor**: Aplicação Console que gerencia múltiplas conexões simultâneas

## 🎯 Objetivos

- ✅ Implementar comunicação segura entre cliente-servidor usando **Sockets TCP/IP**
- ✅ Aplicar **Criptografia Assimétrica (RSA)** para troca segura de chaves
- ✅ Aplicar **Criptografia Simétrica (AES)** para cifrar mensagens
- ✅ Implementar **Autenticação** com credenciais seguras (hash com salt)
- ✅ Validar integridade das mensagens com **Assinaturas Digitais**
- ✅ Suportar **múltiplos clientes simultâneos** no servidor
- ✅ Manter **logs do sistema** com todas as operações

## 🏗️ Arquitetura do Sistema

```
┌─────────────────────────────────────────────────────┐
│                   Base de Dados                      │
│         (Credenciais, Chaves Públicas, Logs)         │
└────────────┬──────────────────────────┬──────────────┘
             │                          │
      ┌──────▼─────────┐        ┌──────▼─────────┐
      │    Servidor    │        │    Servidor    │
      │   Console App  │        │   Console App  │
      └────────────────┘        └────────────────┘
             ▲                          ▲
      ┌──────┴──────────┬───────────────┴──────────┐
      │                 │                          │
   ┌──▼──────┐   ┌─────▼────┐            ┌────────▼──┐
   │ Cliente1 │   │ Cliente2 │            │ ClienteN  │
   │ WinForms │   │ WinForms │            │ WinForms  │
   └──────────┘   └──────────┘            └───────────┘
```

## 🛠️ Tecnologias Utilizadas

- **Linguagem**: C# (.NET Framework)
- **Interface**: Windows Forms
- **Comunicação**: Sockets TCP/IP
- **Criptografia**: 
  - RSA (Assimétrica)
  - AES (Simétrica)
  - MD5/SHA256 (Hash)
- **Base de Dados**: SQL Server / Entity Framework
- **Protocolo**: ProtocolSI.dll (fornecida)
- **Threading**: Múltiplas threads para clientes simultâneos

## 📝 Estrutura do Projeto

```
Projeto-Topicos-Seguranca/
├── README.md                          # Este ficheiro
├── contexto.txt                       # Resumo do enunciado
├── Enunciado.pdf                      # Enunciado completo
├── IPL-TeSP-PSI-TS-2024_Relatório.docx  # Template do relatório
├── .gitignore
└── .git/
```

## 🔄 Fluxo de Comunicação

### Fase 1: Autenticação

1. **Cliente** → Envia chave pública
2. **Servidor** → Recebe, armazena e gera chave simétrica
3. **Servidor** → Cifra chave simétrica com chave pública do cliente
4. **Cliente** → Recebe e decifra chave simétrica
5. **Cliente** → Envia credenciais cifradas com chave simétrica
6. **Servidor** → Valida credenciais

### Fase 2: Chat

1. **Cliente** → Envia mensagem cifrada + assinatura digital
2. **Servidor** → Valida assinatura e armazena
3. **Servidor** → Distribui para outros clientes
4. **Cliente** → Recebe e decifra mensagem

## 📋 Critérios de Avaliação

| Critério | Peso |
|---|---|
| Criptografia Assimétrica | 15% |
| Criptografia Simétrica | 15% |
| Troca de Mensagens | 15% |
| Threads | 15% |
| Autenticação | 10% |
| Validação de Dados | 10% |
| Apresentação do Código | 5% |
| User Interface | 10% |
| Lógica do Chat | 2.5% |
| Extra | 2.5% |

## 📅 Fases de Entrega

### **Fase I (20%)**
- Relatório de Análise de Requisitos
- Aplicação Windows Form (Cliente)
- Aplicação Console (Servidor)
- Troca básica de mensagens (sem cifragem)
- **Avaliação**: Prova oral

### **Fase II (80%)**
- Chat funcional com cifragem
- Validação de assinaturas digitais
- Log do sistema
- Interface final
- Vídeo demonstrativo
- **Avaliação**: Teste prático

## 🚀 Como Usar

### Pré-requisitos
- Microsoft Visual Studio 2019+
- .NET Framework 4.8+
- SQL Server (opcional)

### Instalação

1. Clone o repositório:
```bash
git clone https://github.com/astonrasec/Projeto-Topicos-Seguranca.git
cd Projeto-Topicos-Seguranca
```

2. Abra a solução em Visual Studio:
```bash
# Abrir arquivo .sln
```

3. Configure a base de dados (se aplicável)

4. Compile e execute

## 📖 Documentação

- `contexto.txt` - Resumo dos tópicos abordados
- `Enunciado.pdf` - Especificação completa do projeto
- `IPL-TeSP-PSI-TS-2024_Relatório.docx` - Template do relatório

### ✏️ Status do Projeto - Fase I (20%)

**Relatório - Secções Preenchidas:**
- ✅ 1. Introdução
- ✅ 2. Especificação do Sistema
- ✅ 2.1 Especificação de Requisitos
- ✅ 2.1.1 Requisitos Funcionais (RF)
- ✅ 2.1.2 Requisitos Não Funcionais (RNF)
  - ✅ 2.1.2.1 Usabilidade
  - ✅ 2.1.2.2 Fiabilidade
  - ✅ 2.1.2.3 Segurança
  - ✅ 2.1.2.4 Eficiência
  - ✅ 2.1.2.5 Disponibilidade
  - ✅ 2.1.2.6 Ambiente
  - ✅ 2.1.2.7 Desenvolvimento
- ❌ 3. Conclusão

**Fase I - O que falta implementar:**
- ❌ Relatório de Análise de Requisitos (completo com conclusão)
- ❌ Aplicação Windows Form (Cliente) com troca de mensagens
- ❌ Aplicação Console (Servidor) com suporte a múltiplos clientes
- ❌ Troca de mensagens Cliente1 ↔ Servidor ↔ Cliente2 (sem cifragem)

**Fase I - Para depois (Fase II):**
- ❌ Coluna "Implementado" nas tabelas de RF e RNF (marcar X após implementar)

## 👥 Equipa

- **Estudante 1**: [Número de Estudante]
- **Estudante 2**: [Número de Estudante]
- **Estudante 3**: [Número de Estudante]

## 🔗 Links Úteis

- [Microsoft .NET Documentation](https://docs.microsoft.com/pt-br/dotnet/)
- [C# Client-Server Example](http://snippetbank.blogspot.com/2014/04/csharp-client-server-broadcast-example-1.html)
- [ProtoIP Library](https://github.com/JoaoAJMatos/ProtoIP)

## 📄 Licença

Projeto académico - IPL (Instituto Politécnico de Leiria)

---

**Última atualização**: Abril 2026 | **Ano Letivo**: 2025/2026
