# 📋 JIRA KAN Board - Chat Seguro

*Sincronizado em: 17 de Abril de 2026*

**Total de Issues**: 84

---

## ✅ Issues Criadas

### Epics (4)
- **[KAN-60]** Fase I - Chat Funcional (COMPLETO)
- **[KAN-61]** Testes Fase I
- **[KAN-62]** Documentação Fase I
- **[KAN-63]** Fase II - Cifragem RSA/AES

### User Stories (20)
- **[KAN-64]** US-1: Login com username + senha
- **[KAN-65]** US-2: Validação credenciais com MD5+salt
- **[KAN-66]** US-3: Enviar chave pública ao servidor
- **[KAN-67]** US-4: Receber e armazenar chave pública
- **[KAN-68]** US-5: Enviar mensagens de texto
- **[KAN-69]** US-6: Receber mensagens do servidor
- **[KAN-70]** US-7: Broadcast de mensagens
- **[KAN-71]** US-8: Suportar múltiplos clientes simultâneos
- **[KAN-72]** US-9: Documentação completa
- **[KAN-73]** US-10: Código bem documentado
- **[KAN-74]** US-11: Testar em ambiente novo
- **[KAN-75]** US-12: Cifrar chave AES com RSA
- **[KAN-76]** US-13: Decifrar chave AES
- **[KAN-77]** US-14: Enviar mensagens cifradas AES
- **[KAN-78]** US-15: Decifrar mensagens AES
- **[KAN-79]** US-16: Assinar mensagens com HMAC/SHA256
- **[KAN-80]** US-17: Validar assinaturas digitais
- **[KAN-81]** US-18: Guardar mensagens cifradas
- **[KAN-82]** US-19: Logs de segurança
- **[KAN-83]** US-20: Relatório e vídeo demonstrativo

### Sprint Tasks (60)

**Sprint 1 - Testes (KAN-30 a KAN-37)**
- KAN-30: Teste conexão múltiplos clientes simultâneos
- KAN-31: Teste troca mensagem Client1 ↔ Server ↔ Client2
- KAN-32: Teste desconexão limpa com EOT protocol
- KAN-33: Teste validação username vazio e IP inválido
- KAN-34: Teste reconexão após desconexão
- KAN-35: Teste broadcast - mensagem para todos clientes
- KAN-36: Teste timeout de conexão
- KAN-37: Teste limite de clientes simultâneos

**Sprint 2 - Documentação (KAN-38 a KAN-43)**
- KAN-38: Completar relatório - Introdução + Especificação
- KAN-39: Completar relatório - Requisitos Funcionais
- KAN-40: Completar relatório - Requisitos Não-Funcionais
- KAN-41: Adicionar docstrings faltantes em classes
- KAN-42: Criar diagramas de sequência (Login, Chat, Broadcast)
- KAN-43: Documentar protocolo SI e fluxo de comunicação

**Sprint 3 - Design (KAN-44 a KAN-47)**
- KAN-44: Revisar enunciado Fase II completo
- KAN-45: Desenhar arquitetura RSA key exchange
- KAN-46: Desenhar fluxo AES cifra de mensagens
- KAN-47: Planejar estrutura de logs e assinaturas digitais

**Sprint 4+ - Implementação (KAN-48 a KAN-59)**
- KAN-48: Implementar geração de chaves RSA (cliente + servidor)
- KAN-49: Implementar envio de chave pública (handshake)
- KAN-50: Implementar cifragem RSA da chave simétrica
- KAN-51: Implementar decifragem RSA
- KAN-52: Implementar AES para cifra de mensagens
- KAN-53: Implementar AES para decifragem de mensagens
- KAN-54: Implementar geração de assinatura digital (HMAC/SHA256)
- KAN-55: Implementar validação de assinatura
- KAN-56: Implementar logs de eventos de segurança
- KAN-57: Implementar gestão de chaves privadas
- KAN-58: Testes de cifragem end-to-end
- KAN-59: Testes de segurança e validação

---

## 📊 Resumo

| Métrica | Quantidade |
|---------|-----------|
| **Epics** | 4 |
| **User Stories** | 20 |
| **Tasks** | 60 |
| **Total Issues** | 84 |

---

## 🔗 Links Rápidos

- [Ver Board KAN](https://projetotseg.atlassian.net/software/c/projects/KAN/boards/3)
- [Ver Backlog](https://projetotseg.atlassian.net/software/c/projects/KAN/backlog)

---

**Última sincronização**: 17 de Abril de 2026
**Projeto**: KAN - Chat Seguro (Tópicos de Segurança)
**Status**: ✅ Completo
