# 📊 JIRA - Plano Completo com Sprints, User Stories e Tasks

## 📋 Baseado em:
- ✅ IPL-TeSP-PSI-TS-2024_Relatório_TXT.txt (Requisitos oficiais)
- ✅ Enunciado.pdf (Critérios de avaliação)
- ✅ contexto.txt (Fases e timelines)

---

## 🎯 Cronograma Oficial IPL

| Fase | Avaliação | Peso | Status |
|------|-----------|------|--------|
| **Fase I** | Avaliação Contínua | 20% | 📅 Prova Oral + Entrega |
| **Fase II** | Avaliação Periódica | 80% | ⏳ Teste Prático |
| **Normal/Recurso/Especial** | 100% | 100% | ⏳ Extra features |

---

## 🔄 Sprint Planning (Timeline IPL)

### **SPRINT 1: Fase I - Análise & Autenticação (Semana 1-2)**
**Goal**: Implementar login e autenticação básica  
**Deadline**: Antes de Prova Oral Fase I

**User Stories**:
```
US-1: Como utilizador, quero fazer login com username + senha
US-2: Como servidor, quero validar credenciais com hash+salt (MD5)
US-3: Como cliente, quero enviar chave pública ao servidor
US-4: Como servidor, quero receber e armazenar chave pública
```

**Tasks** (KAN-1 a KAN-10):
- [ ] KAN-1: Design FormLogin (username + IP)
- [ ] KAN-2: Implementar validação de inputs
- [ ] KAN-3: Implementar TCP connection (TcpClient)
- [ ] KAN-4: Criar ProtocolSI USER_OPTION_1 para autenticação
- [ ] KAN-5: Implementar resposta ACK/NAK servidor
- [ ] KAN-6: Implementar armazenamento credenciais com MD5+salt
- [ ] KAN-7: Implementar geração de chaves RSA
- [ ] KAN-8: Implementar envio chave pública
- [ ] KAN-9: Implementar receção e armazenamento chave pública
- [ ] KAN-10: Testes de login (múltiplas tentativas, campos vazios)

**Story Points**: 13  
**Responsável**: Diogo  
**Timeline**: 1-2 semanas

---

### **SPRINT 2: Fase I - Chat Básico (Semana 2-3)**
**Goal**: Implementar troca de mensagens entre clientes (sem cifragem)  
**Deadline**: Antes de Prova Oral Fase I

**User Stories**:
```
US-5: Como cliente, quero enviar mensagens de texto
US-6: Como servidor, quero receber mensagens de múltiplos clientes
US-7: Como cliente, quero receber mensagens dos outros clientes
US-8: Como servidor, quero fazer broadcast de mensagens
```

**Tasks** (KAN-11 a KAN-26):
- [ ] KAN-11: Design FormChat (RichTextBox, TextBox, buttons)
- [ ] KAN-12: Implementar ProtocolSI DATA para mensagens
- [ ] KAN-13: Implementar thread de receção (background)
- [ ] KAN-14: Implementar envio de mensagens
- [ ] KAN-15: Implementar TcpListener servidor porta 10000
- [ ] KAN-16: Implementar ClientHandler (1 thread/cliente)
- [ ] KAN-17: Implementar lista de clientes conectados (thread-safe)
- [ ] KAN-18: Implementar broadcast mensagens
- [ ] KAN-19: Implementar protocol EOT (End of Transmission)
- [ ] KAN-20: Testes 2 clientes simultâneos
- [ ] KAN-21: Testes troca mensagem Client1↔Server↔Client2
- [ ] KAN-22: Testes desconexão limpa
- [ ] KAN-23: Testes 3+ clientes simultâneos
- [ ] KAN-24: Testes broadcast para todos clientes
- [ ] KAN-25: Testes reconexão
- [ ] KAN-26: Testes edge cases (timeout, IP inválido)

**Story Points**: 21  
**Responsável**: Diogo  
**Timeline**: 1.5-2 semanas

---

### **SPRINT 3: Fase I - Documentação & Testes Finais (Semana 3-4)**
**Goal**: Completar relatório e validações para Prova Oral  
**Deadline**: Data Oficial Fase I (calendário IPL)

**User Stories**:
```
US-9: Como docente, quero relatório completo de análise
US-10: Como desenvolvedor, quero código bem documentado
US-11: Como avaliador, quero validar sistema funciona em ambiente novo
```

**Tasks** (KAN-27 a KAN-29 + novos):
- [ ] KAN-27: Completar Relatório - Introdução + Especificação
- [ ] KAN-28: Completar Relatório - Requisitos Funcionais (RF-01 a RF-10)
- [ ] KAN-29: Completar Requisitos Não-Funcionais (todas categorias)
- [ ] KAN-64: Adicionar docstrings em todas classes
- [ ] KAN-65: Adicionar comentários em seções críticas
- [ ] KAN-66: Criar diagramas de sequência (Login, Chat, Broadcast)
- [ ] KAN-67: Documentar protocolo SI e fluxo comunicação
- [ ] KAN-68: Criar guia de instalação
- [ ] KAN-69: Testar em ambiente clean (PC novo, só VS instalado)
- [ ] KAN-70: Validar BD (se usar) - ficheiro .sql

**Story Points**: 16  
**Responsável**: Diogo  
**Timeline**: 0.5-1 semana

---

## 🔐 SPRINT 4: Fase II - Cifragem RSA (Semana 5-6)

**Goal**: Implementar troca de chaves simétricas com RSA  
**Deadline**: 1 semana após Prova Oral Fase I

**User Stories**:
```
US-12: Como servidor, quero cifrar chave AES com RSA cliente
US-13: Como cliente, quero decifrar chave AES com minha chave privada
US-14: Como sistema, quero validar integridade chaves
```

**Tasks** (KAN-48 a KAN-52):
- [ ] KAN-48: Implementar geração chaves RSA (cliente + servidor)
- [ ] KAN-49: Implementar envio chave pública (handshake seguro)
- [ ] KAN-50: Implementar cifragem RSA da chave AES
- [ ] KAN-51: Implementar decifragem RSA (cliente)
- [ ] KAN-71: Testes handshake RSA end-to-end

**Story Points**: 13  
**Responsável**: Diogo  
**Timeline**: 1 semana

---

## 🔒 SPRINT 5: Fase II - Cifragem AES & Assinaturas (Semana 6-7)

**Goal**: Cifrar mensagens com AES e validar com assinaturas digitais  
**Deadline**: Antes de Teste Prático Fase II

**User Stories**:
```
US-15: Como cliente, quero enviar mensagens cifradas com AES
US-16: Como cliente, quero assinar mensagens com HMAC/SHA256
US-17: Como servidor, quero validar assinaturas digitais
US-18: Como servidor, quero guardar mensagens cifradas seguramente
```

**Tasks** (KAN-52 a KAN-59):
- [ ] KAN-52: Implementar AES para cifra mensagens
- [ ] KAN-53: Implementar AES para decifragem mensagens
- [ ] KAN-54: Implementar HMAC/SHA256 assinatura digital
- [ ] KAN-55: Implementar validação assinatura digital
- [ ] KAN-56: Implementar armazenamento mensagens cifradas
- [ ] KAN-72: Testes cifragem AES end-to-end
- [ ] KAN-73: Testes validação assinatura (tampering detection)
- [ ] KAN-74: Testes rejeição mensagens inválidas

**Story Points**: 18  
**Responsável**: Diogo  
**Timeline**: 1 semana

---

## 📝 SPRINT 6: Fase II - Logs & Relatório Final (Semana 7-8)

**Goal**: Implementar logs de segurança e documentação final  
**Deadline**: Data Oficial Fase II (teste prático)

**User Stories**:
```
US-19: Como administrador, quero logs de todas operações segurança
US-20: Como docente, quero relatório final com vídeo demonstrativo
US-21: Como avaliador, quero interface polida e funcional
```

**Tasks**:
- [ ] KAN-56: Implementar logs eventos de segurança (.txt)
- [ ] KAN-57: Implementar gestão chaves privadas (armazenamento seguro)
- [ ] KAN-75: Completar Relatório - Implementação Fase II
- [ ] KAN-76: Criar vídeo demonstrativo (com chaves RSA, hashes, etc.)
- [ ] KAN-77: Polir interface final (UX/UI)
- [ ] KAN-78: Testes integração completa
- [ ] KAN-79: Teste em ambiente novo (validação final)

**Story Points**: 16  
**Responsável**: Diogo  
**Timeline**: 1 semana

---

## 🎁 SPRINT 7: Extra Features (Normal/Recurso/Especial)

**Goal**: Implementar features extra para máxima nota  
**Deadline**: Conforme calendário IPL

**User Stories**:
```
US-22: Como cliente, quero enviar/receber ficheiros cifrados
US-23: Como admin, quero módulo web ASP.NET com autenticação
US-24: Como utilizador, quero histórico de conversação persistente
```

**Tasks**:
- [ ] KAN-80: Implementar envio ficheiros cifrados
- [ ] KAN-81: Implementar receção ficheiros cifrados
- [ ] KAN-82: Criar módulo web ASP.NET
- [ ] KAN-83: Implementar autenticação web
- [ ] KAN-84: Implementar download logs (autenticados)
- [ ] KAN-85: Implementar persistência conversação
- [ ] KAN-86: Criar estatísticas processamento servidor

**Story Points**: 20+  
**Responsável**: Diogo  
**Timeline**: 2-3 semanas (conforme disponibilidade)

---

## 📊 Requisitos Mapeados para Sprints

### **Requisitos Funcionais (RF)**:
| RF | Descrição | Sprint | Status |
|----|-----------|--------|--------|
| RF-01 | Enviar chave pública cliente | S1 | 📅 |
| RF-02 | Servidor armazenar chave pública | S1 | 📅 |
| RF-03 | Servidor gerar/enviar chave AES cifrada | S4 | ⏳ |
| RF-04 | Cliente decifrar chave AES | S4 | ⏳ |
| RF-05 | Cliente autenticar com credenciais cifradas | S1 | 📅 |
| RF-06 | Servidor validar credenciais | S1 | 📅 |
| RF-07 | Cliente enviar mensagens cifradas AES | S5 | ⏳ |
| RF-08 | Servidor validar assinatura e guardar | S5 | ⏳ |
| RF-09 | Servidor distribuir mensagens cifradas | S5 | ⏳ |
| RF-10 | Cliente receber e decifrar mensagens | S5 | ⏳ |

### **Requisitos Não-Funcionais (RNF)**:
| Categoria | Requisitos | Sprint | Peso |
|-----------|-----------|--------|------|
| Usabilidade | UI Windows Forms clara | S2-S6 | - |
| Fiabilidade | Servidor continuar com falha cliente | S2 | Alto |
| Segurança | RSA + AES + Assinaturas | S4-S5 | **Alto** |
| Eficiência | Threads múltiplos clientes | S2 | Alto |
| Disponibilidade | Suportar 10+ clientes | S2 | Médio |
| Ambiente | .NET 4.8, Windows 7+ | S1-S7 | - |
| Desenvolvimento | Código comentado, testes | S3 | Alto |

---

## 🎓 Critérios Avaliação IPL (Pesos)

| Critério | Peso | Sprint | Status |
|----------|------|--------|--------|
| Criptografia Assimétrica (RSA) | 15% | S4 | ⏳ |
| Criptografia Simétrica (AES) | 15% | S5 | ⏳ |
| Troca de Mensagens | 15% | S2 | 📅 |
| Threads | 15% | S2 | 📅 |
| Autenticação | 10% | S1 | 📅 |
| Validação de Dados | 10% | S5 | ⏳ |
| Apresentação Código | 5% | S3 | 📅 |
| User Interface | 10% | S6 | 📅 |
| Lógica Chat | 2.5% | S2 | 📅 |
| Extra | 2.5% | S7 | ⏳ |
| **Total** | **100%** | - | - |

---

## 📅 Timeline Consolidado

```
Semana 1-2: SPRINT 1 (Análise, Autenticação) → Fase I ~40%
Semana 2-3: SPRINT 2 (Chat Básico) → Fase I ~80%
Semana 3-4: SPRINT 3 (Documentação) → Fase I 100% ✅ PROVA ORAL

[INTERVALO]

Semana 5-6: SPRINT 4 (RSA) → Fase II ~20%
Semana 6-7: SPRINT 5 (AES + Assinaturas) → Fase II ~70%
Semana 7-8: SPRINT 6 (Logs + Polimento) → Fase II 100% ✅ TESTE PRÁTICO

[OPCIONAL]

Semana 9+: SPRINT 7 (Extra Features) → Época Normal/Recurso/Especial
```

---

## ✅ Próximos Passos no JIRA

1. ✅ Criar 4 Epics (já feito: KAN-60 a KAN-63)
2. ⏳ **Criar User Stories** para cada sprint
3. ⏳ **Mapear Tasks** aos requisitos RF/RNF
4. ⏳ **Atribuir Story Points**
5. ⏳ **Definir Sprints** com datas oficiais IPL

---

**Versão**: 2.0  
**Última atualização**: Abril 2026  
**Status**: 🔄 Pronto para Criação de Issues
