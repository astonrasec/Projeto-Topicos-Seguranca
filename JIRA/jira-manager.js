#!/usr/bin/env node

require('dotenv').config();
const axios = require('axios');
const fs = require('fs');
const path = require('path');
const readline = require('readline');

const JIRA_EMAIL = process.env.JIRA_EMAIL;
const JIRA_API_TOKEN = process.env.JIRA_API_TOKEN;
const JIRA_DOMAIN = process.env.JIRA_DOMAIN;

const auth = {
  username: JIRA_EMAIL,
  password: JIRA_API_TOKEN
};

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

function question(prompt) {
  return new Promise(resolve => rl.question(prompt, resolve));
}

async function listAllBoards() {
  try {
    const response = await axios.get(`${JIRA_DOMAIN}/rest/agile/1.0/board`, { auth });
    return response.data.values || [];
  } catch (error) {
    console.error(`❌ Erro: ${error.message}`);
    return [];
  }
}

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

async function updateIssueStatus(issueKey, transitionId) {
  try {
    await axios.post(
      `${JIRA_DOMAIN}/rest/api/3/issues/${issueKey}/transitions`,
      { transition: { id: transitionId } },
      { auth }
    );
    return true;
  } catch (error) {
    console.error(`❌ Erro ao atualizar: ${error.message}`);
    return false;
  }
}

async function main() {
  console.log('🚀 JIRA Manager\n');

  // List boards
  console.log('📋 Boards disponíveis:');
  const boards = await listAllBoards();
  boards.forEach((b, i) => {
    console.log(`[${i}] ${b.name} (ID: ${b.id}, Tipo: ${b.type})`);
  });

  if (boards.length === 0) {
    console.log('❌ Nenhum board encontrado');
    rl.close();
    return;
  }

  const boardChoice = await question('\nEscolha um board (número): ');
  const selectedBoard = boards[parseInt(boardChoice)];

  if (!selectedBoard) {
    console.log('❌ Opção inválida');
    rl.close();
    return;
  }

  // Get issues
  console.log(`\n📡 Carregando issues do board "${selectedBoard.name}"...`);
  const issues = await getIssues(selectedBoard.id);

  if (issues.length === 0) {
    console.log('⚠️ Nenhuma issue encontrada neste board.');
    rl.close();
    return;
  }

  console.log(`\n✅ ${issues.length} issues encontradas:\n`);
  issues.forEach((issue, i) => {
    console.log(`[${i}] [${issue.key}] ${issue.fields.summary} (${issue.fields.status.name})`);
  });

  // Save to file
  const md = issues
    .map(i => `- **[${i.key}]** ${i.fields.summary} - ${i.fields.status.name}`)
    .join('\n');

  const outputFile = path.join(__dirname, `JIRA_TASKS_${selectedBoard.name.replace(/\s/g, '_')}.md`);
  fs.writeFileSync(outputFile, `# ${selectedBoard.name}\n\n${md}`, 'utf-8');
  console.log(`\n✅ Salvo em: ${outputFile}`);

  rl.close();
}

main().catch(console.error);
