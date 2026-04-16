# 🚀 JIRA Integration Setup

## ✅ Configuração Completa

### Ficheiros criados:

1. **`.env`** - Credenciais JIRA (🔐 Não comita!)
   - Email, API Token, Domínio, Projeto

2. **`.gitignore`** - Protege `.env`
   - Garante que credenciais nunca são enviadas para Git

3. **`sync-jira-simple.js`** - Pull de tasks do JIRA
   - Sincroniza issues do board para `JIRA_TASKS.md`
   - Agrupa por status (Em Progresso, Concluído, A fazer)

4. **`update-jira-status.js`** - Atualizar status no JIRA
   - Permite mudar estado das issues via CLI

5. **`package.json`** + `node_modules/`
   - Dependências instaladas: `dotenv`, `axios`

---

## 📋 Como Usar

### 1. Sincronizar Tasks do JIRA (Pull)

```bash
node sync-jira-simple.js
```

Gera `JIRA_TASKS.md` com todas as issues organizadas por status.

**Saída:**
- 📄 `JIRA_TASKS.md` - Lista de todas as tasks
- Formato Markdown com links para JIRA

### 2. Atualizar Status de uma Issue

```bash
node update-jira-status.js EMAL-1 "Done"
```

**Pré-requisito:** Descobrir transições disponíveis para uma issue
```bash
node update-jira-status.js EMAL-1
```

---

## 🔐 Segurança

✅ **Credenciais protegidas:**
- `.env` está no `.gitignore` - nunca vai para Git
- API Token é preferível a password
- Cada token é único e pode ser revogado

⚠️ **Se expuseres o token por acidente:**
1. Revoga em: https://id.atlassian.com/manage-profile/security/api-tokens
2. Gera um novo token
3. Atualiza `.env` localmente

---

## 🔧 Configuração do `.env`

```
JIRA_EMAIL=2024153720@my.ipleiria.pt
JIRA_API_TOKEN=ATATT3xFf...   (token completo)
JIRA_DOMAIN=https://my-team-ht1gik22.atlassian.net
JIRA_PROJECT=KAN
JIRA_BOARD=2
```

**Nunca commita este ficheiro!**

---

## 📊 Boards Disponíveis

- [1] EMAL board (contém 7 issues)
- [2] KAN board (vazio atualmente)

O script `sync-jira-simple.js` puxa do EMAL board por padrão.

---

## ✨ Próximas Funcionalidades

- [ ] Atualizar status via CLI (em desenvolvimento)
- [ ] Criar novas issues automaticamente
- [ ] Comentar em issues
- [ ] Sincronização bidirecional

---

## 🆘 Troubleshooting

### "Erro de autenticação 401"
- Verifica que o API token é válido
- Confirma o email está correto

### "404 - Board não encontrado"
- O KAN board (ID: 2) está vazio
- O EMAL board (ID: 1) tem issues e funciona

### "Nenhuma tarefa para sincronizar"
- O board selecionado está vazio
- Verifica que tens acesso ao projeto no JIRA
