#!/usr/bin/env node

require('dotenv').config();
const axios = require('axios');

const JIRA_DOMAIN = process.env.JIRA_DOMAIN;
const JIRA_EMAIL = process.env.JIRA_EMAIL;
const JIRA_API_TOKEN = process.env.JIRA_API_TOKEN;
const JIRA_API = `${JIRA_DOMAIN}/rest/api/3`;

const auth = {
  username: JIRA_EMAIL,
  password: JIRA_API_TOKEN
};

const epics = [
  {
    summary: "Fase I - Chat Funcional (COMPLETO)",
    description: "Chat TCP multi-cliente implementado a 100%. ClientManager (Singleton) + FormLauncher + FormLogin + FormChat + Server.cs + ProtocolSI (USER_OPTION_1, DATA, ACK, EOT). Sem cifragem (Fase II). Issues: KAN-1 a KAN-29",
    issuetype: "Epic",
    priority: "Highest"
  },
  {
    summary: "Testes Fase I",
    description: "Testes unitários e integração para validar múltiplos clientes simultâneos, troca de mensagens Cliente1↔Server↔Cliente2, desconexão limpa com EOT, validação de inputs. Issues: KAN-30 a KAN-37",
    issuetype: "Epic",
    priority: "High"
  },
  {
    summary: "Documentação Fase I",
    description: "Completar relatório de análise (introdução, especificação, requisitos, conclusão), adicionar docstrings, criar diagramas de sequência, documentar protocolo SI. Issues: KAN-38 a KAN-43",
    issuetype: "Epic",
    priority: "High"
  },
  {
    summary: "Fase II - Cifragem RSA/AES",
    description: "Implementar cifragem assimétrica (RSA), simétrica (AES), assinaturas digitais (HMAC/SHA256), logs de segurança, gestão de chaves privadas. Issues: KAN-44 a KAN-59",
    issuetype: "Epic",
    priority: "Medium"
  }
];

async function createEpics() {
  console.log("📋 Criando Epics no JIRA...\n");

  let created = 0;
  let failed = 0;

  for (const epic of epics) {
    try {
      const response = await axios.post(
        `${JIRA_API}/issue`,
        {
          fields: {
            project: { key: "KAN" },
            summary: epic.summary,
            description: {
              version: 1,
              type: "doc",
              content: [
                {
                  type: "paragraph",
                  content: [
                    {
                      type: "text",
                      text: epic.description
                    }
                  ]
                }
              ]
            },
            issuetype: { name: epic.issuetype },
            priority: { name: epic.priority }
          }
        },
        { auth }
      );

      console.log(`✅ [${response.data.key}] ${epic.summary}`);
      created++;
    } catch (error) {
      console.error(`❌ Erro ao criar epic "${epic.summary}":`, error.response?.data?.errors || error.message);
      failed++;
    }
  }

  console.log(`\n📊 Resumo:`);
  console.log(`   ✅ Criados: ${created}`);
  console.log(`   ❌ Falhados: ${failed}`);
  console.log(`   📅 Total: ${epics.length}`);
}

createEpics();
