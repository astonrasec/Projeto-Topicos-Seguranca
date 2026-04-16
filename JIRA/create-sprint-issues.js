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

// Issues para cada sprint
const issues = [
  // Sprint 1: Testes Fase I
  {
    summary: "Teste conexão múltiplos clientes simultâneos",
    description: "Validar que 2-3 clientes conseguem conectar ao mesmo tempo ao servidor",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 1"
  },
  {
    summary: "Teste troca mensagem Client1 ↔ Server ↔ Client2",
    description: "Validar que mensagens chegam corretamente de um cliente para outro passando pelo servidor",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 1"
  },
  {
    summary: "Teste desconexão limpa com EOT protocol",
    description: "Validar que EOT (End of Transmission) funciona corretamente e liberta recursos",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 1"
  },
  {
    summary: "Teste validação username vazio e IP inválido",
    description: "Validar que sistema rejeita username vazio e IP em formato inválido",
    issuetype: "Task",
    priority: "Medium",
    sprint: "Sprint 1"
  },
  {
    summary: "Teste reconexão após desconexão",
    description: "Validar que cliente consegue reconectar após desconexão normal",
    issuetype: "Task",
    priority: "Medium",
    sprint: "Sprint 1"
  },
  {
    summary: "Teste broadcast - mensagem para todos clientes",
    description: "Validar que quando um cliente envia, todos recebem",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 1"
  },
  {
    summary: "Teste timeout de conexão",
    description: "Validar comportamento quando servidor não responde",
    issuetype: "Task",
    priority: "Low",
    sprint: "Sprint 1"
  },
  {
    summary: "Teste limite de clientes simultâneos",
    description: "Verificar se há limite de clientes que podem conectar",
    issuetype: "Task",
    priority: "Low",
    sprint: "Sprint 1"
  },

  // Sprint 2: Documentação Fase I
  {
    summary: "Completar relatório - Introdução + Especificação",
    description: "Preencher seções 1 e 2 do relatório com texto final",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 2"
  },
  {
    summary: "Completar relatório - Requisitos Funcionais",
    description: "Completar seção 2.1.1 com análise final e adicionar coluna Implementado",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 2"
  },
  {
    summary: "Completar relatório - Requisitos Não-Funcionais",
    description: "Completar seção 2.1.2 com análise final de RNF",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 2"
  },
  {
    summary: "Adicionar docstrings faltantes em classes",
    description: "Completar comentários de documentação em todos os métodos públicos",
    issuetype: "Task",
    priority: "Medium",
    sprint: "Sprint 2"
  },
  {
    summary: "Criar diagramas de sequência (Login, Chat, Broadcast)",
    description: "Desenhar diagramas UML de sequência para os 3 fluxos principais",
    issuetype: "Task",
    priority: "Medium",
    sprint: "Sprint 2"
  },
  {
    summary: "Documentar protocolo SI e fluxo de comunicação",
    description: "Criar documento com explicação do protocolo SI e tipos de mensagem",
    issuetype: "Task",
    priority: "Medium",
    sprint: "Sprint 2"
  },

  // Sprint 3: Design Fase II
  {
    summary: "Revisar enunciado Fase II completo",
    description: "Ler e analisar enunciado completo da Fase II (RSA, AES, assinaturas, logs)",
    issuetype: "Story",
    priority: "High",
    sprint: "Sprint 3"
  },
  {
    summary: "Desenhar arquitetura RSA key exchange",
    description: "Criar diagrama de handshake RSA para troca de chaves simétricas",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 3"
  },
  {
    summary: "Desenhar fluxo AES cifra de mensagens",
    description: "Criar diagrama de fluxo de cifragem/decifragem AES de mensagens",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 3"
  },
  {
    summary: "Planejar estrutura de logs e assinaturas digitais",
    description: "Definir como implementar logs de segurança e assinaturas digitais",
    issuetype: "Task",
    priority: "Medium",
    sprint: "Sprint 3"
  },

  // Sprint 4: Implementação Fase II
  {
    summary: "Implementar geração de chaves RSA (cliente + servidor)",
    description: "Criar métodos para gerar pares de chaves RSA no cliente e servidor",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 4"
  },
  {
    summary: "Implementar envio de chave pública (handshake)",
    description: "Implementar troca segura de chaves públicas entre cliente e servidor",
    issuetype: "Story",
    priority: "Highest",
    sprint: "Sprint 4"
  },
  {
    summary: "Implementar cifragem RSA da chave simétrica",
    description: "Servidor cifra chave AES com chave pública do cliente",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 4"
  },
  {
    summary: "Implementar decifragem RSA",
    description: "Cliente decifra chave AES com sua chave privada",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 4"
  },
  {
    summary: "Implementar AES para cifra de mensagens",
    description: "Cifrar mensagens com AES antes de enviar",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 4"
  },
  {
    summary: "Implementar AES para decifragem de mensagens",
    description: "Decifrar mensagens com AES ao receber",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 4"
  },
  {
    summary: "Implementar geração de assinatura digital (HMAC/SHA256)",
    description: "Assinar mensagens com HMAC ou SHA256 para garantir autenticidade",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 4"
  },
  {
    summary: "Implementar validação de assinatura",
    description: "Validar assinatura digital ao receber mensagem",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 4"
  },
  {
    summary: "Implementar logs de eventos de segurança",
    description: "Registrar em ficheiro todos os eventos: conexão, desconexão, cifragem, validação",
    issuetype: "Task",
    priority: "Medium",
    sprint: "Sprint 4"
  },
  {
    summary: "Implementar gestão de chaves privadas",
    description: "Armazenar e proteger chaves privadas (ficheiro ou armazenamento seguro)",
    issuetype: "Task",
    priority: "Medium",
    sprint: "Sprint 4"
  },
  {
    summary: "Testes de cifragem end-to-end",
    description: "Validar que cifragem RSA + AES funciona de ponta a ponta",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 4"
  },
  {
    summary: "Testes de segurança e validação",
    description: "Validar que assinaturas digitais funcionam e logs são registados",
    issuetype: "Task",
    priority: "High",
    sprint: "Sprint 4"
  }
];

async function createIssues() {
  console.log("📝 Criando issues nos Sprints...\n");

  let created = 0;
  let failed = 0;

  for (const issue of issues) {
    try {
      const response = await axios.post(
        `${JIRA_API}/issue`,
        {
          fields: {
            project: { key: "KAN" },
            summary: issue.summary,
            description: {
              version: 1,
              type: "doc",
              content: [
                {
                  type: "paragraph",
                  content: [
                    {
                      type: "text",
                      text: issue.description
                    }
                  ]
                }
              ]
            },
            issuetype: { name: issue.issuetype },
            priority: { name: issue.priority }
          }
        },
        { auth }
      );

      console.log(`✅ [${response.data.key}] ${issue.summary}`);
      created++;
    } catch (error) {
      console.error(`❌ Erro: "${issue.summary}"`, error.response?.data?.errors || error.message);
      failed++;
    }
  }

  console.log(`\n📊 Resumo:`);
  console.log(`   ✅ Criados: ${created}`);
  console.log(`   ❌ Falhados: ${failed}`);
  console.log(`   📅 Total: ${issues.length}`);
}

createIssues();
