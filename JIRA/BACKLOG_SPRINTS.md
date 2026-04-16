# 📊 JIRA Backlog & Sprints - Chat Seguro

## Status Atual (Baseado em Análise do Código C#)

### ✅ Implementado (Fase I - 100%)
- **Cliente**: FormLauncher + FormLogin + FormChat + ClientManager (Singleton)
- **Servidor**: TcpListener + ClientHandler + Broadcast
- **Threading**: Receção em background thread (cliente) + thread por cliente (servidor)
- **Protocolo**: ProtocolSI (USER_OPTION_1, DATA, ACK, EOT)
- **Comunicação**: Múltiplos clientes simultâneos funcionando

### ❌ Falta (Fase I)
- Testes automatizados
- Documentação/Relatório
- Edge cases (desconexões abruptas, timeout, etc.)

### ⏳ Futuro (Fase II)
- Cifragem RSA/AES
- Assinaturas digitais
- Logs de segurança

---

## 🎯 Epics Propostos

### EPIC-1: Fase I - Chat Funcional ✅ COMPLETO
- **Status**: Review
- **Prioridade**: Highest
- **Issues Ligadas**: KAN-1 até KAN-29 (29 issues)
- **Descrição**: Chat com múltiplos clientes, sem cifragem
- **Progresso**: 95% (código pronto, testes faltam)

### EPIC-2: Testes Fase I 🆕
- **Status**: To Do
- **Prioridade**: High
- **Estimativa**: 1 semana
- **Issues a Criar**: 8-10
- **Descrição**: Testes unitários, integração, validação de edge cases

### EPIC-3: Documentação Fase I 🆕
- **Status**: To Do
- **Prioridade**: High
- **Estimativa**: 1 semana
- **Issues a Criar**: 5-7
- **Descrição**: Relatório, docstrings, diagramas, documentação técnica

### EPIC-4: Fase II - Cifragem RSA/AES 🆕
- **Status**: To Do
- **Prioridade**: Medium
- **Estimativa**: 3-4 semanas
- **Issues a Criar**: 15-20
- **Descrição**: Implementar RSA, AES, assinaturas digitais, logs

---

## 📅 Sprint Planning

### Sprint 1: Testes Fase I (Semana 1)
**Goal**: Validar que o sistema funciona com múltiplos clientes, edge cases
**Issues**: ~8

```
[ ] TEST-1: Teste conexão múltiplos clientes simultâneos (2-3 clientes)
[ ] TEST-2: Teste troca mensagem Client1 ↔ Server ↔ Client2
[ ] TEST-3: Teste desconexão limpa com EOT protocol
[ ] TEST-4: Teste username vazio / IP inválido (validação)
[ ] TEST-5: Teste reconexão após desconexão
[ ] TEST-6: Teste broadcast - mensagem para todos os clientes
[ ] TEST-7: Teste timeout de conexão
[ ] TEST-8: Teste limite de clientes simultâneos
```

**Estimativa**: 8-13 points
**Responsável**: Diogo

---

### Sprint 2: Documentação Fase I (Semana 2)
**Goal**: Completar documentação para entrega Fase I
**Issues**: ~6

```
[ ] DOC-1: Completar relatório - Introdução + Especificação
[ ] DOC-2: Completar relatório - Requisitos Funcionais
[ ] DOC-3: Completar relatório - Requisitos Não-Funcionais
[ ] DOC-4: Adicionar docstrings faltantes em classes
[ ] DOC-5: Criar diagramas de sequência (Login, Chat, Broadcast)
[ ] DOC-6: Documentar protocolo SI e fluxo de comunicação
```

**Estimativa**: 8-10 points
**Responsável**: Diogo

---

### Sprint 3: Fase II - Design & Arquitetura (Semana 3-4)
**Goal**: Planejar implementação de cifragem e segurança
**Issues**: ~4

```
[ ] DESIGN-1: Revisar enunciado Fase II completo
[ ] DESIGN-2: Desenhar arquitetura RSA key exchange
[ ] DESIGN-3: Desenhar fluxo AES cifra de mensagens
[ ] DESIGN-4: Planejar estrutura de logs e assinaturas digitais
```

**Estimativa**: 5-8 points
**Responsável**: Diogo

---

### Sprint 4: Fase II - Implementação (Semana 5-7)
**Goal**: Implementar cifragem, assinaturas e logs
**Issues**: ~12

```
[ ] CRIPT-1: Implementar geração de chaves RSA (cliente + servidor)
[ ] CRIPT-2: Implementar envio de chave pública (handshake)
[ ] CRIPT-3: Implementar cifragem RSA da chave simétrica
[ ] CRIPT-4: Implementar decifragem RSA
[ ] CRIPT-5: Implementar AES para cifra de mensagens
[ ] CRIPT-6: Implementar AES para decifragem de mensagens
[ ] CRIPT-7: Implementar geração de assinatura digital (HMAC/SHA256)
[ ] CRIPT-8: Implementar validação de assinatura
[ ] CRIPT-9: Implementar logs de eventos de segurança
[ ] CRIPT-10: Implementar gestão de chaves privadas
[ ] CRIPT-11: Testes de cifragem end-to-end
[ ] CRYPT-12: Testes de segurança e validação
```

**Estimativa**: 13-20 points
**Responsável**: Diogo

---

## 📈 Timeline Total

| Sprint | Duração | Issues | Status |
|--------|---------|--------|--------|
| Sprint 1 (Testes I) | 1 semana | 8 | 📅 Planning |
| Sprint 2 (Docs I) | 1 semana | 6 | 📅 Planning |
| Sprint 3 (Design II) | 2 semanas | 4 | ⏳ Backlog |
| Sprint 4 (Impl II) | 3 semanas | 12 | ⏳ Backlog |
| **Total** | **7 semanas** | **30+** | - |

---

## 🔄 Backlog Geral (Priorizado)

### Tier 1: Crítico (Fase I - Testes + Docs)
1. Testes múltiplos clientes
2. Testes troca mensagens
3. Testes desconexão
4. Completar relatório
5. Adicionar docstrings

### Tier 2: Importante (Fase II - Design)
6. Revisar Fase II
7. Design RSA
8. Design AES

### Tier 3: Futuro (Fase II - Implementação)
9-30. Tasks de cifragem, assinaturas, logs

---

## 🛠️ Como Usar Este Plano

1. **Criar Issues manualmente no JIRA** baseado neste backlog
2. **Atribuir a sprints** seguindo as datas propostas
3. **Estimar pontos** (Story Points) conforme progresso
4. **Atualizar status** conforme trabalho avança
5. **Sincronizar** com `JIRA/sync-jira-simple.js`

---

**Última atualização**: Abril 2026
**Responsável**: Diogo
**Status**: 📋 Pronto para implementação
