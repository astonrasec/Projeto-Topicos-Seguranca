#!/usr/bin/env python3
"""
Sync JIRA tasks with local project
Pulls all issues from JIRA board and creates local TODO file
"""

import os
import json
import requests
from dotenv import load_dotenv
from datetime import datetime
from pathlib import Path

# Load environment variables from .env
load_dotenv()

JIRA_EMAIL = os.getenv('JIRA_EMAIL')
JIRA_API_TOKEN = os.getenv('JIRA_API_TOKEN')
JIRA_DOMAIN = os.getenv('JIRA_DOMAIN')
JIRA_PROJECT = os.getenv('JIRA_PROJECT')

if not all([JIRA_EMAIL, JIRA_API_TOKEN, JIRA_DOMAIN, JIRA_PROJECT]):
    print("❌ Erro: Faltam variáveis .env")
    print("Certifica-te que .env tem: JIRA_EMAIL, JIRA_API_TOKEN, JIRA_DOMAIN, JIRA_PROJECT")
    exit(1)

# JIRA API base URL
JIRA_API_URL = f"{JIRA_DOMAIN}/rest/api/3"

def get_jira_issues():
    """Fetch all issues from JIRA project"""
    url = f"{JIRA_API_URL}/projects/{JIRA_PROJECT}/issues"
    
    headers = {
        "Authorization": f"Basic {JIRA_EMAIL}:{JIRA_API_TOKEN}",
        "Content-Type": "application/json"
    }
    
    print(f"📡 Conectando a JIRA: {JIRA_DOMAIN}")
    
    try:
        # Use JQL to get all issues
        jql_url = f"{JIRA_API_URL}/search"
        params = {
            "jql": f"project = {JIRA_PROJECT}",
            "maxResults": 100,
            "fields": "key,summary,status,assignee,priority,created,updated"
        }
        
        response = requests.get(jql_url, headers=headers, params=params, auth=(JIRA_EMAIL, JIRA_API_TOKEN))
        response.raise_for_status()
        
        data = response.json()
        print(f"✅ Sincronizado: {len(data['issues'])} tarefas encontradas")
        return data['issues']
        
    except requests.exceptions.ConnectionError:
        print("❌ Erro: Não consegui conectar a JIRA. Verifica o JIRA_DOMAIN.")
        return []
    except requests.exceptions.HTTPError as e:
        print(f"❌ Erro HTTP: {e.response.status_code} - {e.response.text}")
        return []
    except Exception as e:
        print(f"❌ Erro: {str(e)}")
        return []

def format_issues_to_markdown(issues):
    """Convert JIRA issues to Markdown format"""
    if not issues:
        return "# 📋 JIRA Tasks\n\nNenhuma tarefa encontrada."
    
    md = f"# 📋 JIRA Tasks - {JIRA_PROJECT}\n\n"
    md += f"*Sincronizado em: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}*\n\n"
    
    # Group by status
    by_status = {}
    for issue in issues:
        status = issue['fields'].get('status', {}).get('name', 'Unknown')
        if status not in by_status:
            by_status[status] = []
        by_status[status].append(issue)
    
    # Sort statuses: To Do, In Progress, Done
    status_order = ['To Do', 'In Progress', 'Done']
    
    for status in status_order:
        if status in by_status:
            md += f"## {status}\n\n"
            for issue in by_status[status]:
                key = issue['key']
                summary = issue['fields'].get('summary', 'N/A')
                assignee = issue['fields'].get('assignee', {})
                assignee_name = assignee.get('displayName', 'Unassigned') if assignee else 'Unassigned'
                priority = issue['fields'].get('priority', {}).get('name', 'N/A')
                
                md += f"- **[{key}]** {summary}\n"
                md += f"  - Assignee: {assignee_name}\n"
                md += f"  - Priority: {priority}\n"
                md += f"  - Link: {JIRA_DOMAIN}/browse/{key}\n\n"
    
    # Add any other statuses not in the standard list
    for status in by_status:
        if status not in status_order:
            md += f"## {status}\n\n"
            for issue in by_status[status]:
                key = issue['key']
                summary = issue['fields'].get('summary', 'N/A')
                assignee = issue['fields'].get('assignee', {})
                assignee_name = assignee.get('displayName', 'Unassigned') if assignee else 'Unassigned'
                priority = issue['fields'].get('priority', {}).get('name', 'N/A')
                
                md += f"- **[{key}]** {summary}\n"
                md += f"  - Assignee: {assignee_name}\n"
                md += f"  - Priority: {priority}\n"
                md += f"  - Link: {JIRA_DOMAIN}/browse/{key}\n\n"
    
    return md

def main():
    """Main sync function"""
    print("🚀 Iniciando sincronização JIRA...\n")
    
    # Fetch issues
    issues = get_jira_issues()
    
    if not issues:
        print("⚠️ Nenhuma tarefa para sincronizar.")
        return
    
    # Format to Markdown
    markdown_content = format_issues_to_markdown(issues)
    
    # Write to file
    output_file = Path(__file__).parent / f"JIRA_TASKS_{JIRA_PROJECT}.md"
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(markdown_content)
    
    print(f"✅ Tarefas salvas em: {output_file}")
    print(f"📊 Total: {len(issues)} tarefas")
    
    # Print summary
    print("\n" + "="*50)
    print(markdown_content[:500] + "...")

if __name__ == "__main__":
    main()
