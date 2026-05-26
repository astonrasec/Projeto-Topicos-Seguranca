# 🔐 Chat Seguro - Tópicos de Segurança

**Disciplina**: Tópicos de Segurança  
**Ano Letivo**: 2024/2025 | **Turno**: PL1 | **Grupo**: A  
**Docente**: Nuno Simões

---

## 👥 Equipa

- Diogo Gomes Almeida (2024153720) - Desenvolvedor
- Vitor Emanuel Louro Leonardo (2025152794) - Desenvolvedor
- Kelly Ferreira (2025153175) - Desenvolvedor

---

## 📋 Projeto

Sistema de **chat seguro em C#** com cliente (Windows Forms) e servidor (Console).

**Fase I (20%)**: Chat básico sem criptografia - ✅ COMPLETO  
**Fase II (80%)**: Criptografia RSA/AES + Assinaturas - ⏳ Planejado

---

## 🏗️ Componentes

### Cliente (ChatClient.exe)
- `FormLauncher.cs` - Inicialização
- `ClientManager.cs` - Gerenciamento de conexão (Singleton)
- `FormLogin.cs` - Autenticação
- `FormChat.cs` - Interface de chat

### Servidor (ChatServer.exe)
- `Server.cs` - Multi-cliente com threads

**Total**: ~1,600 linhas de código

---

## 🚀 Como Usar

### Compilação
```bash
cd TrabalhoPratico
msbuild TrabalhoPratico.sln
```

### Execução

**Terminal 1 - Servidor:**
```bash
cd TrabalhoPratico/ChatServer/bin/Debug
ChatServer.exe
```

**Terminal 2+ - Clientes:**
```bash
cd TrabalhoPratico/ChatClient/bin/Debug
ChatClient.exe
```

---

## 🔧 Requisitos

- Windows 7+
- .NET Framework 4.8+
- Visual Studio 2019+ (opcional)

---

## 📁 Estrutura

```
Projeto-Topicos-Seguranca/
├── README.md
├── TrabalhoPratico/
│   ├── ChatClient/
│   ├── ChatServer/
│   └── TrabalhoPratico.sln
├── documentacao/
├── Enunciado.pdf
└── IPL-TeSP-PSI-TS-2024_Relatório.docx
```

---

## 📚 Documentação

Veja pasta `documentacao/` para análise técnica completa, testes e relatórios.

---

**Status**: ✅ Fase I Completa | 📅 Próximo: Prova Oral (21 Abr 2026)
