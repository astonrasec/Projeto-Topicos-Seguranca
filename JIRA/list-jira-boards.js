#!/usr/bin/env node

require('dotenv').config();
const axios = require('axios');

const JIRA_EMAIL = process.env.JIRA_EMAIL;
const JIRA_API_TOKEN = process.env.JIRA_API_TOKEN;
const JIRA_DOMAIN = process.env.JIRA_DOMAIN;

const client = axios.create({
  baseURL: `${JIRA_DOMAIN}/rest/api/2`,
  headers: {
    'Authorization': `Basic ${Buffer.from(`${JIRA_EMAIL}:${JIRA_API_TOKEN}`).toString('base64')}`,
    'Content-Type': 'application/json'
  }
});

async function listBoards() {
  try {
    console.log('📋 Listando boards disponíveis...\n');
    const response = await client.get('/boards');
    const boards = response.data.values;
    
    if (boards.length === 0) {
      console.log('❌ Nenhum board encontrado.');
      return;
    }

    boards.forEach(board => {
      console.log(`[ID: ${board.id}] ${board.name} (${board.type})`);
    });

    console.log(`\n✅ Total: ${boards.length} boards`);
  } catch (error) {
    console.error(`❌ Erro: ${error.response?.status} - ${error.message}`);
  }
}

listBoards();
