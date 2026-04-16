#!/usr/bin/env node

require('dotenv').config();
const axios = require('axios');
const fs = require('fs');
const path = require('path');

const JIRA_EMAIL = process.env.JIRA_EMAIL;
const JIRA_API_TOKEN = process.env.JIRA_API_TOKEN;
const JIRA_DOMAIN = process.env.JIRA_DOMAIN;

const auth = {
  username: JIRA_EMAIL,
  password: JIRA_API_TOKEN
};

async function getIssues(boardId) {
  try {
    const response = await axios.get(
      `${JIRA_DOMAIN}/rest/agile/1.0/board/${boardId}/issue`,
      { auth, params: { maxResults: 100 } }
    );
    return response.data.issues || [];
  } catch (error) {
    console.error(`❌ Erro: ${error.message}`);
    return [];
  }
}

function formatMarkdown(issues, boardName) {
  let md = `# 📋 ${boardName}\n\n`;
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

  Object.entries(byStatus).forEach(([status, statusIssues]) => {
    md += `## ${status}\n\n`;
    statusIssues.forEach(issue => {
      md += `- **[${issue.key}]** ${issue.fields.summary}\n`;
      md += `  - Assignee: ${issue.fields.assignee?.displayName || 'Unassigned'}\n`;
      md += `  - Priority: ${issue.fields.priority?.name || 'N/A'}\n`;
      md += `  - Link: [${JIRA_DOMAIN}/browse/${issue.key}](${JIRA_DOMAIN}/browse/${issue.key})\n\n`;
    });
  });

  return md;
}

async function main() {
  console.log('🚀 JIRA Sync\n');

  console.log('Buscando issues do EMAL board...');
  const issues = await getIssues(1);

  if (issues.length === 0) {
    console.log('⚠️ Nenhuma issue encontrada.');
    return;
  }

  console.log(`✅ ${issues.length} issues encontradas\n`);

  const markdown = formatMarkdown(issues, 'EMAL Board');
  const outputFile = path.join(__dirname, 'JIRA_TASKS.md');
  
  fs.writeFileSync(outputFile, markdown, 'utf-8');
  console.log(`✅ Salvo em: ${outputFile}`);
}

main();
