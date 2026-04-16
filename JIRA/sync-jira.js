#!/usr/bin/env node

require('dotenv').config();
const axios = require('axios');
const fs = require('fs');
const path = require('path');

const JIRA_EMAIL = process.env.JIRA_EMAIL;
const JIRA_API_TOKEN = process.env.JIRA_API_TOKEN;
const JIRA_DOMAIN = process.env.JIRA_DOMAIN;
const JIRA_PROJECT = process.env.JIRA_PROJECT;

if (!JIRA_EMAIL || !JIRA_API_TOKEN || !JIRA_DOMAIN || !JIRA_PROJECT) {
  console.error('❌ Erro: Faltam variáveis .env');
  process.exit(1);
}

const auth = {
  username: JIRA_EMAIL,
  password: JIRA_API_TOKEN
};

async function testAuth() {
  try {
    console.log('🔐 Testando autenticação...');
    const response = await axios.get(`${JIRA_DOMAIN}/rest/api/3/myself`, { auth });
    console.log(`✅ Autenticado como: ${response.data.displayName}\n`);
    return true;
  } catch (error) {
    console.error(`❌ Erro de autenticação: ${error.response?.status}`);
    return false;
  }
}

async function findBoardByProject() {
  try {
    const response = await axios.get(`${JIRA_DOMAIN}/rest/agile/1.0/board`, { auth });
    const boards = response.data.values || [];
    
    const board = boards.find(b => b.name.toUpperCase().includes(JIRA_PROJECT));
    if (board) {
      console.log(`📌 Board encontrado: [${board.id}] ${board.name}`);
      return board.id;
    }
    
    if (boards.length > 0) {
      console.log(`⚠️ Projeto "${JIRA_PROJECT}" não encontrado, usando primeiro board: ${boards[0].name}`);
      return boards[0].id;
    }
    
    return null;
  } catch (error) {
    console.error(`❌ Erro ao buscar boards: ${error.message}`);
    return null;
  }
}

async function getIssues(boardId) {
  try {
    console.log(`📡 Buscando issues do board ${boardId}...`);
    
    const response = await axios.get(
      `${JIRA_DOMAIN}/rest/agile/1.0/board/${boardId}/issue`,
      { auth, params: { maxResults: 100 } }
    );

    const issues = response.data.issues || [];
    console.log(`✅ ${issues.length} issues encontradas\n`);
    return issues;
  } catch (error) {
    console.error(`❌ Erro ao buscar issues: ${error.message}`);
    return [];
  }
}

function formatMarkdown(issues) {
  let md = `# 📋 JIRA Tasks - ${JIRA_PROJECT}\n\n`;
  md += `*Sincronizado em: ${new Date().toLocaleString('pt-PT')}*\n\n`;

  if (issues.length === 0) {
    md += 'Nenhuma tarefa encontrada.\n';
    return md;
  }

  const byStatus = {};
  issues.forEach(issue => {
    const status = issue.fields.status?.name || 'Unknown';
    if (!byStatus[status]) byStatus[status] = [];
    byStatus[status].push(issue);
  });

  const statusOrder = ['To Do', 'In Progress', 'In Review', 'Done'];
  
  statusOrder.forEach(status => {
    if (byStatus[status]) {
      md += `## ${status}\n\n`;
      byStatus[status].forEach(issue => {
        md += `- **[${issue.key}]** ${issue.fields.summary}\n`;
        md += `  - Assignee: ${issue.fields.assignee?.displayName || 'Unassigned'}\n`;
        md += `  - Priority: ${issue.fields.priority?.name || 'N/A'}\n`;
        md += `  - Link: [${JIRA_DOMAIN}/browse/${issue.key}](${JIRA_DOMAIN}/browse/${issue.key})\n\n`;
      });
    }
  });

  Object.keys(byStatus).forEach(status => {
    if (!statusOrder.includes(status)) {
      md += `## ${status}\n\n`;
      byStatus[status].forEach(issue => {
        md += `- **[${issue.key}]** ${issue.fields.summary}\n`;
        md += `  - Assignee: ${issue.fields.assignee?.displayName || 'Unassigned'}\n`;
        md += `  - Priority: ${issue.fields.priority?.name || 'N/A'}\n`;
        md += `  - Link: [${JIRA_DOMAIN}/browse/${issue.key}](${JIRA_DOMAIN}/browse/${issue.key})\n\n`;
      });
    }
  });

  return md;
}

async function main() {
  console.log('🚀 JIRA Sync - KAN Board\n');

  const authOk = await testAuth();
  if (!authOk) return;

  const boardId = await findBoardByProject();
  if (!boardId) {
    console.error('❌ Nenhum board encontrado');
    return;
  }

  const issues = await getIssues(boardId);
  if (issues.length === 0) {
    console.log('⚠️ Nenhuma tarefa para sincronizar.');
    return;
  }

  const markdown = formatMarkdown(issues);
  const outputFile = path.join(__dirname, `JIRA_TASKS_${JIRA_PROJECT}.md`);
  
  fs.writeFileSync(outputFile, markdown, 'utf-8');
  console.log(`✅ Tarefas salvas em: ${outputFile}`);
  console.log(`📊 Total: ${issues.length} issues\n`);
}

main();
