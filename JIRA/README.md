# 📋 JIRA Project Structure - Chat Seguro

## Overview
Documentação centralizada do projeto Chat Seguro em C# com rastreamento de issues, tasks e epics via JIRA.

**Board**: [JIRA KAN Board](https://projetotseg.atlassian.net/software/c/projects/KAN/boards/3)

---

## 📁 Estrutura de Pastas

### `/01_Analise/`
Documentação de requisitos e análise do projeto.
- Requisitos Funcionais (RF)
- Requisitos Não-Funcionais (RNF)
- Especificações do protocolo
- Diagramas de arquitetura

### `/02_Implementacao/`
Guias e tracking de implementação.
- Fase I: Chat Seguro (sem cifragem)
- Fase II: Cifragem RSA/AES + Assinaturas Digitais
- Notas de progresso por módulo
- Checklist de funcionalidades

### `/03_Testes/`
Planos e resultados de testes.
- Testes unitários
- Testes de integração
- Casos de teste
- Resultados de execução

### `/04_Documentacao/`
Documentação técnica do código.
- Comentários e docstrings
- Diagramas de sequência
- Guias de API
- Troubleshooting

### `/05_Fase_II/`
Recursos específicos para cifragem e segurança.
- Especificação RSA/AES
- Assinaturas digitais
- Logs de segurança
- Gestão de chaves

---

## 🎯 Issues JIRA - Fase I

Total: **29 issues** criadas e prontas para desenvolvimento.

### Epics
- **KAN-1**: Fase I - Chat Seguro com Múltiplos Clientes

### Stories & Tasks
1. **Análise & Documentação** (KAN-2 até KAN-4)
   - Validação de requisitos
   - Documentação RF/RNF

2. **Cliente - Arquitetura** (KAN-5 até KAN-10)
   - FormLauncher e ClientManager (Singleton)
   - Autenticação (USER_OPTION_1)
   - FormLogin

3. **Cliente - Troca de Mensagens** (KAN-11 até KAN-14)
   - FormChat
   - Thread de receção
   - Protocol SI (DATA)

4. **Servidor** (KAN-15 até KAN-19)
   - TcpListener (porta 10000)
   - ClientHandler
   - Broadcast
   - Protocol EOT

5. **Documentação do Código** (KAN-20 até KAN-22)
   - Docstrings
   - Comentários críticos

6. **Testes** (KAN-23 até KAN-26)
   - Múltiplos clientes simultâneos
   - Troca de mensagens (Client1 ↔ Server ↔ Client2)
   - Desconexão limpa

7. **Fase II - Preparação** (KAN-27 até KAN-29)
   - Revisão de enunciado
   - Design arquitetura RSA/AES

---

## 📊 Status de Progresso

| Fase | Status | Progresso |
|------|--------|-----------|
| Fase I - Implementação | ✅ Concluída | 100% |
| Fase I - Testes | 🔄 Planejado | 0% |
| Fase I - Documentação | 🔄 Planejado | 0% |
| Fase II - Design | 🔄 Planejado | 0% |
| Fase II - Implementação | ⏳ Bloqueado | 0% |

---

## 🔗 Sincronização

Para atualizar a lista de issues localmente:
```bash
node sync-jira-simple.js
```

Resultado: `JIRA_TASKS.md` com issues atualizadas.

---

## 📝 Convenções

- **Issue ID**: KAN-N (ex: KAN-5)
- **Prioridade**: Ordem numérica (KAN-1 > KAN-2)
- **Status**: TO DO → IN PROGRESS → DONE
- **Responsável**: Diogo (atualmente)

---

## 🚀 Próximos Passos

1. ✅ Issues JIRA criadas (29/29)
2. ✅ Estrutura de pastas pronta
3. ⏳ Executar testes da Fase I
4. ⏳ Gerar documentação técnica
5. ⏳ Início da Fase II

---

**Última atualização**: 2025-04-16  
**Responsável**: Diogo  
**Versão**: 1.0
