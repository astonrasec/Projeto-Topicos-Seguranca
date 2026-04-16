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

const userStories = [
  {
    summary: "US-1: Login com username + senha",
    description: "Como utilizador, quero fazer login fornecendo username e senha para aceder ao chat. RF-05, RF-06",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 1"
  },
  {
    summary: "US-2: Validação credenciais com MD5+salt",
    description: "Como servidor, quero validar credenciais com hash MD5 + salt aleatório. RNF-SEG-03",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 1"
  },
  {
    summary: "US-3: Enviar chave pública ao servidor",
    description: "Como cliente, quero enviar minha chave pública RSA ao servidor. RF-01, RNF-SEG-01",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 1"
  },
  {
    summary: "US-4: Receber e armazenar chave pública",
    description: "Como servidor, quero receber e armazenar chave pública do cliente. RF-02",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 1"
  },
  {
    summary: "US-5: Enviar mensagens de texto",
    description: "Como cliente, quero escrever e enviar mensagens de texto. RF-07",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 2"
  },
  {
    summary: "US-6: Receber mensagens do servidor",
    description: "Como cliente, quero receber mensagens dos outros clientes via servidor. RF-10",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 2"
  },
  {
    summary: "US-7: Broadcast de mensagens",
    description: "Como servidor, quero fazer broadcast de mensagens para todos clientes conectados. RF-09",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 2"
  },
  {
    summary: "US-8: Suportar múltiplos clientes simultâneos",
    description: "Como servidor, quero suportar múltiplos clientes simultâneos com threads. RNF-EFI-01, RNF-DIS-03",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 2"
  },
  {
    summary: "US-9: Documentação completa",
    description: "Como docente, quero relatório de análise de requisitos completo. RNF-DES-01, RNF-DES-02",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 3"
  },
  {
    summary: "US-10: Código bem documentado",
    description: "Como desenvolvedor, quero código com docstrings e comentários em seções críticas.",
    issuetype: "Story",
    priority: "Medium",
    sprint: "Sprint 3"
  },
  {
    summary: "US-11: Testar em ambiente novo",
    description: "Como avaliador, quero validar que sistema funciona em instalação fresh. RNF-DES-03",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 3"
  },
  {
    summary: "US-12: Cifrar chave AES com RSA",
    description: "Como servidor, quero cifrar chave AES com chave pública do cliente. RF-03, RNF-SEG-01",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 4"
  },
  {
    summary: "US-13: Decifrar chave AES",
    description: "Como cliente, quero decifrar chave AES com minha chave privada RSA. RF-04",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 4"
  },
  {
    summary: "US-14: Enviar mensagens cifradas AES",
    description: "Como cliente, quero cifrar mensagens com chave AES antes de enviar. RF-07, RNF-SEG-02",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 5"
  },
  {
    summary: "US-15: Decifrar mensagens AES",
    description: "Como cliente, quero decifrar mensagens recebidas com chave AES. RF-10, RNF-SEG-02",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 5"
  },
  {
    summary: "US-16: Assinar mensagens com HMAC/SHA256",
    description: "Como cliente, quero assinar mensagens com HMAC/SHA256 para garantir autenticidade. RNF-SEG-04",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 5"
  },
  {
    summary: "US-17: Validar assinaturas digitais",
    description: "Como servidor, quero validar assinatura digital de cada mensagem recebida. RF-08, RNF-SEG-04",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 5"
  },
  {
    summary: "US-18: Guardar mensagens cifradas",
    description: "Como servidor, quero armazenar mensagens cifradas de forma segura. RF-08",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 5"
  },
  {
    summary: "US-19: Logs de segurança",
    description: "Como administrador, quero logs de todas operações segurança (conexão, autenticação, cifragem). RNF-FIA-04",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 6"
  },
  {
    summary: "US-20: Relatório e vídeo demonstrativo",
    description: "Como docente, quero relatório final e vídeo mostrando chaves RSA, hashes, assinaturas.",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 6"
  }
];

async function createUserStories() {
  console.log("📖 Criando User Stories no JIRA...\n");

  let created = 0;
  let failed = 0;

  for (const story of userStories) {
    try {
      const response = await axios.post(
        `${JIRA_API}/issue`,
        {
          fields: {
            project: { key: "KAN" },
            summary: story.summary,
            description: {
              version: 1,
              type: "doc",
              content: [
                {
                  type: "paragraph",
                  content: [
                    {
                      type: "text",
                      text: story.description
                    }
                  ]
                }
              ]
            },
            issuetype: { name: story.issuetype },
            priority: { name: story.priority }
          }
        },
        { auth }
      );

      console.log(`✅ [${response.data.key}] ${story.summary}`);
      created++;
    } catch (error) {
      console.error(`❌ Erro: "${story.summary}"`, error.response?.data?.errors || error.message);
      failed++;
    }
  }

  console.log(`\n📊 Resumo:`);
  console.log(`   ✅ User Stories Criadas: ${created}`);
  console.log(`   ❌ Falhadas: ${failed}`);
  console.log(`   📅 Total: ${userStories.length}`);
}

createUserStories();
