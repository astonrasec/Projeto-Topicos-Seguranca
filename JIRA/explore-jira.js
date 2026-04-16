require('dotenv').config();
const axios = require('axios');

const auth = {
  username: process.env.JIRA_EMAIL,
  password: process.env.JIRA_API_TOKEN
};

async function main() {
  try {
    console.log('📊 Explorando estrutura JIRA...\n');
    
    // Get board info
    const boardRes = await axios.get(`${process.env.JIRA_DOMAIN}/rest/agile/1.0/board`, { auth });
    console.log('Boards encontrados:', boardRes.data.values?.length || 0);
    
    if (boardRes.data.values && boardRes.data.values.length > 0) {
      boardRes.data.values.forEach(b => {
        console.log(`  - [${b.id}] ${b.name} (${b.type})`);
      });
    }

    // Try to get issues from first board
    if (boardRes.data.values && boardRes.data.values[0]) {
      const boardId = boardRes.data.values[0].id;
      console.log(`\n📋 Buscando issues do board ${boardId}...`);
      
      const issuesRes = await axios.get(
        `${process.env.JIRA_DOMAIN}/rest/agile/1.0/board/${boardId}/issue`,
        { auth }
      );
      
      console.log(`Encontradas: ${issuesRes.data.issues?.length || 0} issues`);
      if (issuesRes.data.issues && issuesRes.data.issues.length > 0) {
        issuesRes.data.issues.slice(0, 3).forEach(issue => {
          console.log(`  - [${issue.key}] ${issue.fields.summary}`);
        });
      }
    }
  } catch (error) {
    console.error(`❌ Erro: ${error.message}`);
    if (error.response?.data) {
      console.error('Resposta:', JSON.stringify(error.response.data, null, 2).substring(0, 500));
    }
  }
}

main();
