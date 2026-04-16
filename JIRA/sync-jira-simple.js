#!/usr/bin/env node

require('dotenv').config();
const axios = require('axios');
const fs = require('fs');
const path = require('path');

const JIRA_EMAIL = process.env.JIRA_EMAIL;
const JIRA_API_TOKEN = process.env.JIRA_API_TOKEN;
const JIRA_DOMAIN = process.env.JIRA_DOMAIN;
const JIRA_API = `${JIRA_DOMAIN}/rest/api/3`;

const auth = {
  username: JIRA_EMAIL,
  password: JIRA_API_TOKEN
};

async function getIssues() {
  try {
    const response = await axios.get(
      `${JIRA_API}/issues/search`,
      { 
        auth, 
        params: { 
          jql: 'project = KAN',
          maxResults: 100,
          expand: 'changelog'
        } 
      }
    );
    return response.data.issues || [];
  } catch (error) {
    console.error(`❌ Erro: ${error.response?.status} - ${error.message}`);
    return [];
  }
}

function formatMarkdown(issues) {
  let md = `# 📋 JIRA KAN Board - Chat Seguro\n\n`;
  md += `*Sincronizado em: ${new Date().toLocaleString('pt-PT')}*\n\n`;
  md += `**Total de Issues**: ${issues.length}\n\n`;

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

  Object.entries(byStatus).forEach(([status, statusIssues]) => {
    md += `## ${status} (${statusIssues.length})\n\n`;
    statusIssues.sort((a, b) => {
      const numA = parseInt(a.key.split('-')[1]);
      const numB = parseInt(b.key.split('-')[1]);
      return numA - numB;
    });
    
    statusIssues.forEach(issue => {
      md += `- **[${issue.key}]** ${issue.fields.summary}\n`;
      md += `  - Tipo: ${issue.fields.issuetype?.name || 'N/A'}\n`;
      md += `  - Prioridade: ${issue.fields.priority?.name || 'N/A'}\n`;
      if (issue.fields.assignee) {
        md += `  - Atribuído a: ${issue.fields.assignee.displayName}\n`;
      }
      md += `  - Link: [Abrir](${JIRA_DOMAIN}/browse/${issue.key})\n\n`;
    });
  });

  return md;
}

async function main() {
  console.log('🚀 JIRA Sync\n');

  console.log('Buscando issues do projeto KAN...');
  const issues = await getIssues();

  if (issues.length === 0) {
    console.log('⚠️ Nenhuma issue encontrada.');
    return;
  }

  console.log(`✅ ${issues.length} issues encontradas\n`);

  const markdown = formatMarkdown(issues);
  const outputFile = path.join(__dirname, 'JIRA_TASKS.md');
  
  fs.writeFileSync(outputFile, markdown, 'utf-8');
  console.log(`✅ Salvo em: ${outputFile}`);
}

main();
