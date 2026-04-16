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

async function getTransitions(issueKey) {
  try {
    const response = await axios.get(
      `${JIRA_DOMAIN}/rest/api/2/issues/${issueKey}/transitions`,
      { auth }
    );
    return response.data.transitions || [];
  } catch (error) {
    console.error(`❌ Erro ao buscar transições: ${error.message}`);
    return [];
  }
}

async function updateStatus(issueKey, transitionId) {
  try {
    await axios.post(
      `${JIRA_DOMAIN}/rest/api/2/issues/${issueKey}/transitions`,
      { transition: { id: transitionId } },
      { auth }
    );
    console.log(`✅ [${issueKey}] Status atualizado`);
    return true;
  } catch (error) {
    console.error(`❌ Erro: ${error.message}`);
    return false;
  }
}

async function main() {
  const args = process.argv.slice(2);
  
  if (args.length < 2) {
    console.log('Uso: node update-jira-status.js <ISSUE_KEY> <STATUS>');
    console.log('');
    console.log('Exemplos:');
    console.log('  node update-jira-status.js EMAL-1 "In Progress"');
    console.log('  node update-jira-status.js EMAL-1 "Done"');
    console.log('');
    
    const transitions = await getTransitions('EMAL-1');
    if (transitions.length > 0) {
      console.log('Transições disponíveis para EMAL-1:');
      transitions.forEach(t => {
        console.log(`  - "${t.name}" (ID: ${t.id})`);
      });
    }
    return;
  }

  const issueKey = args[0];
  const desiredStatus = args[1];

  console.log(`🔍 Buscando transições para ${issueKey}...`);
  const transitions = await getTransitions(issueKey);

  if (transitions.length === 0) {
    console.log('❌ Nenhuma transição disponível');
    return;
  }

  const transition = transitions.find(t => t.name.toLowerCase() === desiredStatus.toLowerCase());

  if (!transition) {
    console.log(`❌ Status "${desiredStatus}" não encontrado. Disponíveis:`);
    transitions.forEach(t => {
      console.log(`  - "${t.name}"`);
    });
    return;
  }

  console.log(`📝 Atualizando para "${transition.name}"...`);
  await updateStatus(issueKey, transition.id);
}

main();
