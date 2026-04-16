#!/usr/bin/env node
/**
 * create-epics.js — Estrutura de Epics para Chat Seguro
 *
 * 1. Actualiza KAN-1 (Epic Fase I existente)
 * 2. Cria EPIC-2: Testes Fase I
 * 3. Cria EPIC-3: Documentação Fase I
 * 4. Cria EPIC-4: Fase II - Cifragem RSA/AES
 * 5. Liga KAN-2..KAN-29 ao Epic correspondente
 * 6. Guarda mapeamento em jira-state.json para scripts seguintes
 *
 * Uso: node JIRA/create-epics.js  (a partir da raiz do projecto)
 */

require('dotenv').config({ path: require('path').resolve(__dirname, '../.env') });
const axios = require('axios');
const fs2   = require('fs');
const path  = require('path');

const JIRA_EMAIL     = process.env.JIRA_EMAIL;
const JIRA_API_TOKEN = process.env.JIRA_API_TOKEN;
const JIRA_DOMAIN    = process.env.JIRA_DOMAIN;
const PROJECT_KEY    = process.env.JIRA_PROJECT || 'KAN';
const STATE_FILE     = path.join(__dirname, 'jira-state.json');
const auth           = { username: JIRA_EMAIL, password: JIRA_API_TOKEN };

const log = (e, m) => console.log(e + '  ' + m);

function loadState() {
  return fs2.existsSync(STATE_FILE)
    ? JSON.parse(fs2.readFileSync(STATE_FILE, 'utf-8'))
    : {};
}

function saveState(data) {
  const merged = { ...loadState(), ...data };
  fs2.writeFileSync(STATE_FILE, JSON.stringify(merged, null, 2), 'utf-8');
  log('💾', 'Estado guardado em jira-state.json');
}

async function api(method, endpoint, data) {
  try {
    const res = await axios({ method, url: JIRA_DOMAIN + endpoint, auth, ...(data && { data }) });
    return res.data;
  } catch (err) {
    const detail = err.response?.data
      ? JSON.stringify(err.response.data).substring(0, 300)
      : err.message;
    throw new Error(method.toUpperCase() + ' ' + endpoint + ' -> ' + detail);
  }
}

async function createIssue(fields) {
  try {
    const res = await api('post', '/rest/api/2/issue', { fields });
    return { key: res.key, id: res.id };
  } catch (err) {
    log('❌', 'Erro ao criar  + fields.summary + : ' + err.message);
    return null;
  }
}

async function updateIssue(issueKey, fields) {
  try {
    await api('put', '/rest/api/2/issue/' + issueKey, { fields });
    return true;
  } catch (err) {
    log('⚠️ ', 'Erro ao actualizar ' + issueKey + ': ' + err.message);
    return false;
  }
}

async function linkToEpic(issueKey, epicKey) {
  const ok = await updateIssue(issueKey, { customfield_10014: epicKey });
  if (ok) return true;
  return updateIssue(issueKey, { parent: { key: epicKey } });
}

async function getIssue(issueKey) {
  try { return await api('get', '/rest/api/2/issue/' + issueKey + '?fields=summary'); }
  catch { return null; }
}

const EPICS = {
  faseI: {
    summary: 'Fase I - Chat Funcional (COMPLETO)',
    description: 'Chat TCP multi-cliente implementado a 100%.

✅ ClientManager (Singleton) + FormLauncher
✅ FormLogin (username + IP, TCP, USER_OPTION_1)
✅ FormChat (receiveThread, RichTextBox, envio DATA)
✅ Server.cs (TcpListener :10000, ClientHandler por thread, broadcast)
✅ ProtocolSI: USER_OPTION_1, DATA, ACK, EOT
❌ Sem cifragem (previsto em Fase II)

Issues: KAN-1 a KAN-29',
  },
  testes: {
    summary: 'Testes Fase I',
    description: 'Cobertura de testes para o chat básico implementado.

• Testes de múltiplos clientes simultâneos
• Fluxo completo Cliente1 <-> Server <-> Cliente2
• Desconexão limpa via EOT
• Validação de inputs no login
• Edge cases: cliente cai a meio, servidor reinicia',
    priority: 'High',
    epicNameField: 'Testes Fase I',
  },
  documentacao: {
    summary: 'Documentação Fase I',
    description: 'Documentação técnica e relatório académico da Fase I.

• Completar relatório (secção Conclusão em falta)
• Diagramas de sequência UML
• Docstrings C# em todos os métodos públicos
• Documentar protocolo ProtocolSI
• Wireframes actualizados',
    priority: 'High',
    epicNameField: 'Documentação Fase I',
  },
  faseII: {
    summary: 'Fase II - Cifragem RSA/AES',
    description: 'Segurança completa sobre o chat da Fase I.

• RSA key exchange (cliente envia chave pública)
• AES para cifrar mensagens
• Assinaturas digitais por mensagem
• Autenticação com hash+salt
• Log de segurança
• Validação de integridade

Peso: 80% da nota final.',
    priority: 'Medium',
    epicNameField: 'Fase II Cifragem RSA/AES',
  },
};

const ISSUE_EPIC_MAP = {
  'KAN-2': 'faseI', 'KAN-3': 'faseI', 'KAN-4': 'faseI',
  'KAN-5': 'faseI', 'KAN-6': 'faseI', 'KAN-7': 'faseI',
  'KAN-8': 'faseI', 'KAN-9': 'faseI', 'KAN-10': 'faseI',
  'KAN-11': 'faseI', 'KAN-12': 'faseI', 'KAN-13': 'faseI', 'KAN-14': 'faseI',
  'KAN-15': 'faseI', 'KAN-16': 'faseI', 'KAN-17': 'faseI', 'KAN-18': 'faseI', 'KAN-19': 'faseI',
  'KAN-20': 'documentacao', 'KAN-21': 'documentacao', 'KAN-22': 'documentacao',
  'KAN-23': 'testes', 'KAN-24': 'testes', 'KAN-25': 'testes', 'KAN-26': 'testes',
  'KAN-27': 'faseII', 'KAN-28': 'faseII', 'KAN-29': 'faseII',
};

async function main() {
  console.log('='.repeat(60));
  log('🎯', 'create-epics.js — Estrutura de Epics para Chat Seguro');
  console.log('='.repeat(60));

  if (!JIRA_EMAIL || !JIRA_API_TOKEN || !JIRA_DOMAIN) {
    log('❌', 'Variáveis de ambiente em falta! Verifica o ficheiro .env');
    process.exit(1);
  }

  const state = loadState();
  if (!state.epicKeys) state.epicKeys = {};

  log('📌', 'EPIC-1 — Fase I (KAN-1 existente)');
  const kan1 = await getIssue('KAN-1');
  if (kan1) {
    await updateIssue('KAN-1', { summary: EPICS.faseI.summary, description: EPICS.faseI.description });
    log('✅', 'KAN-1 actualizado:  + EPICS.faseI.summary + ');
  } else {
    log('⚠️ ', 'KAN-1 nao encontrado — o epic Fase I pode ter outra key');
  }
  state.epicKeys.faseI = 'KAN-1';

  for (const alias of ['testes', 'documentacao', 'faseII']) {
    const epic = EPICS[alias];
    console.log();
    log('📌', 'EPIC — ' + epic.summary);

    if (state.epicKeys[alias]) {
      log('ℹ️ ', 'Ja existe: ' + state.epicKeys[alias] + ' — a saltar criacao');
      continue;
    }

    const result = await createIssue({
      project:           { key: PROJECT_KEY },
      issuetype:         { name: 'Epic' },
      summary:           epic.summary,
      description:       epic.description,
      priority:          { name: epic.priority },
      customfield_10011: epic.epicNameField,
    });

    if (result) {
      state.epicKeys[alias] = result.key;
      log('✅', 'Criado ' + result.key + ':  + epic.summary + ');
    } else {
      log('❌', 'Falha ao criar epic  + epic.summary + ');
    }
  }

  console.log();
  log('🔗', 'A ligar 28 issues existentes aos seus Epics...');
  let linked = 0, failed = 0;

  for (const [issueKey, epicAlias] of Object.entries(ISSUE_EPIC_MAP)) {
    const epicKey = state.epicKeys[epicAlias];
    if (!epicKey) {
      log('⚠️ ', issueKey + ': epic  + epicAlias +  sem key — a saltar');
      failed++;
      continue;
    }
    const ok = await linkToEpic(issueKey, epicKey);
    if (ok) { log('✅', issueKey + ' -> ' + epicKey); linked++; }
    else     { log('❌', issueKey + ' -> falha ao ligar a ' + epicKey); failed++; }
    await new Promise(r => setTimeout(r, 150));
  }

  console.log();
  console.log('='.repeat(60));
  log('📊', 'SUMÁRIO');
  console.log('  Epics criados/actualizados : ' + Object.keys(state.epicKeys).length);
  console.log('  Issues ligadas com sucesso : ' + linked);
  console.log('  Issues com falha           : ' + failed);
  console.log();
  log('🗂️ ', 'Mapeamento de Epics:');
  for (const [alias, key] of Object.entries(state.epicKeys)) {
    console.log('    ' + key.padEnd(8) + ' -> ' + (EPICS[alias]?.summary ?? alias));
  }
  saveState(state);
  console.log();
  log('🚀', 'Proximo passo: node JIRA/create-backlog.js');
  console.log('='.repeat(60));
}

main().catch(err => { console.error('
❌ Erro fatal:', err.message); process.exit(1); });
