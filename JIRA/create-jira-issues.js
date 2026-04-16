#!/usr/bin/env node

require('dotenv').config();
const axios = require('axios');

const JIRA_EMAIL = process.env.JIRA_EMAIL;
const JIRA_API_TOKEN = process.env.JIRA_API_TOKEN;
const JIRA_DOMAIN = process.env.JIRA_DOMAIN;

const auth = {
  username: JIRA_EMAIL,
  password: JIRA_API_TOKEN
};

async function createIssue(issueData) {
  try {
    const response = await axios.post(
      `${JIRA_DOMAIN}/rest/api/2/issue`,
      issueData,
      { auth }
    );
    return response.data;
  } catch (error) {
    console.error(`❌ Erro ao criar issue: ${error.response?.data?.errors?.summary || error.message}`);
    return null;
  }
}

async function main() {
  console.log('📝 Criando User Stories e Tasks para Chat Seguro\n');

  const issues = [
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Epic' },
        summary: 'Fase I - Chat Seguro com Múltiplos Clientes',
        description: 'Implementar suporte para múltiplos clientes simultâneos com troca de mensagens (sem cifragem)',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Story' },
        summary: 'Validação de Requisitos e Análise',
        description: 'Como analista, quero documentar todos os requisitos funcionais e não-funcionais do projeto para garantir alinhamento com o cliente',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Documentar Requisitos Funcionais (RF)',
        description: 'Criar lista completa de RF com base no enunciado do projeto',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Documentar Requisitos Não-Funcionais (RNF)',
        description: 'Criar lista completa de RNF (Usabilidade, Fiabilidade, Segurança, Eficiência, etc)',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Story' },
        summary: 'Implementar Cliente com Múltiplos Clientes',
        description: 'Como utilizador, quero abrir múltiplas instâncias do cliente simultâneas para enviar mensagens a partir de diferentes sessões',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Criar FormLauncher para gerenciar múltiplos clientes',
        description: 'Interface principal que permite abrir novos clientes e visualizar contador de clientes ativos',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Implementar ClientManager (Singleton)',
        description: 'Gerenciador centralizado para rastrear clientes ativos e notificar FormLauncher de mudanças',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Story' },
        summary: 'Implementar Autenticação de Utilizador',
        description: 'Como cliente, quero autenticar-me no servidor usando username e password de forma segura',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Criar formulário de login (FormLogin)',
        description: 'Interface para entrada de credenciais (username, IP do servidor)',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Implementar protocolo de autenticação (USER_OPTION_1)',
        description: 'Enviar username ao servidor usando o tipo de mensagem USER_OPTION_1',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Story' },
        summary: 'Implementar Troca de Mensagens Entre Clientes',
        description: 'Como utilizador, quero enviar e receber mensagens entre Cliente1 e Cliente2 através do servidor',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Criar FormChat para visualizar e enviar mensagens',
        description: 'Interface de chat com RichTextBox para histórico e TextBox para entrada de mensagens',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Implementar thread de receção de mensagens (background)',
        description: 'Thread dedicada para receber mensagens continuamente sem bloquear UI',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Implementar envio de mensagens com ProtocolSI (DATA)',
        description: 'Serializar mensagens usando ProtocolSI.Make(DATA, msg) e enviar via NetworkStream',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Story' },
        summary: 'Implementar Servidor com Suporte a Múltiplos Clientes',
        description: 'Como servidor, quero aceitar múltiplas conexões simultâneas e fazer broadcast de mensagens',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Implementar TcpListener na porta 10000',
        description: 'Server que escuta conexões na porta padrão do projeto',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Criar ClientHandler para gerenciar cada cliente',
        description: 'Classe que representa um cliente conectado e gerencia sua comunicação',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Implementar broadcast de mensagens',
        description: 'Função que distribui mensagens de um cliente para todos os outros conectados',
        priority: { name: 'High' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Implementar protocol EOT (End of Transmission)',
        description: 'Lidar com desconexão limpa de clientes via mensagem EOT',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Story' },
        summary: 'Documentação e Comentários do Código',
        description: 'Como mantedor, quero código bem documentado com docstrings e comentários explicativos',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Adicionar docstrings a todas as classes e métodos',
        description: 'Documentar responsabilidades e objetivos de cada função',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Adicionar comentários em seções críticas',
        description: 'Explicar lógica de threading, sincronização e protocolo de comunicação',
        priority: { name: 'Low' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Story' },
        summary: 'Testes e Validação',
        description: 'Como testador, quero validar que múltiplos clientes podem se conectar e trocar mensagens',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Testar conexão de múltiplos clientes simultâneos',
        description: 'Validar que 2+ clientes podem conectar e interagir simultaneamente',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Testar troca de mensagens Cliente1 ↔ Servidor ↔ Cliente2',
        description: 'Validar que mensagens chegam corretamente entre clientes',
        priority: { name: 'Medium' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Testar desconexão limpa com EOT',
        description: 'Validar que clientes podem desconectar sem erros',
        priority: { name: 'Low' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Story' },
        summary: 'Preparação para Fase II - Cifragem',
        description: 'Como arquiteto, quero preparar a arquitetura para implementação de RSA e AES na Fase II',
        priority: { name: 'Low' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Revisar enunciado da Fase II',
        description: 'Ler e documentar requisitos de cifragem, assinaturas digitais e logs',
        priority: { name: 'Low' }
      }
    },
    {
      fields: {
        project: { key: 'KAN' },
        issuetype: { name: 'Task' },
        summary: 'Desenhar arquitetura para suporte a RSA/AES',
        description: 'Planejar integração de criptografia sem quebrar código existente',
        priority: { name: 'Low' }
      }
    }
  ];

  let created = 0;
  for (const issue of issues) {
    const result = await createIssue(issue);
    if (result) {
      console.log(`✅ [${result.key}] ${issue.fields.summary}`);
      created++;
    }
  }

  console.log(`\n✅ Total: ${created}/${issues.length} issues criadas`);
}

main();
