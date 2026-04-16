require('dotenv').config();
const axios = require('axios');

const JIRA_EMAIL = process.env.JIRA_EMAIL;
const JIRA_API_TOKEN = process.env.JIRA_API_TOKEN;
const JIRA_DOMAIN = process.env.JIRA_DOMAIN;

const auth = {
  username: JIRA_EMAIL,
  password: JIRA_API_TOKEN
};

async function testEndpoint(path) {
  try {
    const url = `${JIRA_DOMAIN}${path}`;
    const response = await axios.get(url, { auth });
    console.log(`✅ ${path} - ${response.status}`);
    return response.data;
  } catch (error) {
    console.log(`❌ ${path} - ${error.response?.status || error.message}`);
    return null;
  }
}

async function main() {
  console.log('🔍 Testando endpoints...\n');
  
  await testEndpoint('/rest/api/3/projects/KAN');
  await testEndpoint('/rest/api/3/projects');
  await testEndpoint('/rest/agile/1.0/board');
  await testEndpoint('/rest/agile/1.0/boards');
}

main();
