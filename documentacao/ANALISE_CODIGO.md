# 📊 ANÁLISE DO CÓDIGO C# - Fase I

## ✅ Avaliação Geral

O código está **MUITO BEM DOCUMENTADO** e **SIMPLES DE PERCEBER**. Tem:
- ✅ Comentários explicativos em Português
- ✅ Docstrings XML em todos os métodos
- ✅ Código limpo e sem complexidade desnecessária
- ✅ Padrões de design simples (Singleton, Event-Driven, Thread-Per-Client)

---

## 📁 Estrutura do Projeto

```
ChatClient/                    ChatServer/
├── FormLauncher.cs           ├── Server.cs
├── FormLogin.cs              │   ├── Main()        [TCP Listener]
├── FormChat.cs               │   └── ClientHandler [Per-thread]
├── ClientManager.cs          └── Properties/
└── Program.cs
```

---

## 🎯 Componentes Principais

### 1️⃣ **FormLauncher.cs** (107 linhas)
**Propósito**: Interface principal do cliente — ponto de entrada

**Responsabilidades**:
- Mostrar contador de clientes abertos
- Permitir adicionar novos clientes (botão "+ Adicionar Cliente")
- Pedir confirmação ao fechar se há clientes ativos

**Fluxo**:
```
FormLauncher abre
    ↓
Utilizador clica "+ Adicionar Cliente"
    ↓
FormLogin abre
    ↓
Quando FormLogin conecta → ClientManager.RegisterClient()
    ↓
FormLauncher recebe evento e atualiza label (0 → 1 → 2...)
    ↓
Quando FormChat fecha → ClientManager.UnregisterClient()
    ↓
FormLauncher atualiza label (2 → 1 → 0...)
```

**Conceitos utilizados**:
- ✅ Event-driven architecture (subscrição a eventos)
- ✅ Thread-safety com InvokeRequired/Invoke
- ✅ Validação de estado antes de fechar

---

### 2️⃣ **ClientManager.cs** (70 linhas)
**Propósito**: Gerenciador centralizado de estado dos clientes

**Responsabilidades**:
- Manter contador de clientes ativos
- Disparar eventos quando o contador muda
- Permitir FormLauncher consultar e sincronizar com UI

**Padrão**: Singleton estático com eventos

```csharp
public static event Action<int> OnClientCountChanged
{
    add { onClientCountChanged += value; }
    remove { onClientCountChanged -= value; }
}

public static void RegisterClient()     // Incrementa e dispara evento
public static void UnregisterClient()   // Decrementa e dispara evento
public static int GetActiveClientCount() // Consulta o valor
```

**Conceitos utilizados**:
- ✅ Padrão Singleton estático
- ✅ Event listeners (Observer Pattern)
- ✅ Controlo de estado centralizado

---

### 3️⃣ **FormLogin.cs** (263 linhas)
**Propósito**: Autenticação e conexão ao servidor

**Responsabilidades**:
- Recolher username e IP do servidor
- Validar campos (não vazio, IP válido)
- Estabelecer ligação TCP
- Enviar/receber protocolo ProtocolSI
- Abrir FormChat se autenticação bem-sucedida

**Fluxo de Autenticação**:
```
1. Utilizador preenche Username + IP
2. Clica "Conectar"
3. Valida campos
4. Cria IPEndPoint (IP + PORT 10000)
5. Cria TcpClient e Connect()
6. Obtém NetworkStream
7. Cria ProtocolSI
8. Envia USER_OPTION_1 (username)
9. Aguarda ACK do servidor
10. Se ACK recebido → Abre FormChat
11. Chama ClientManager.RegisterClient()
```

**Tratamento de Erros**:
```csharp
try {
    // Conexão TCP
} catch (FormatException) {
    // IP inválido
} catch (Exception ex) {
    // Servidor offline, porto errada, etc.
}
```

**Conceitos utilizados**:
- ✅ Validação de inputs
- ✅ Sockets TCP/IP (IPAddress, IPEndPoint, TcpClient)
- ✅ NetworkStream para comunicação
- ✅ ProtocolSI (biblioteca fornecida)
- ✅ Tratamento estruturado de exceções

---

### 4️⃣ **FormChat.cs** (464 linhas)
**Propósito**: Interface de conversação em tempo real

**Responsabilidades**:
- Mostrar histórico de mensagens
- Permitir ao utilizador enviar mensagens
- Receber mensagens em tempo real (thread background)
- Fechar ligação de forma limpa

**Arquitetura de Threads**:
```
┌─────────────────────────────────┐
│       UI THREAD (Main)          │
│  - Botão Enviar                 │
│  - Escrever no RichTextBox      │
│  - Botão Desligar              │
└─────────────┬───────────────────┘
              │ lock(sendLock)
              ├─────────────┐
              │             │
         Write to stream    │
                     Read from stream
                            │
              ┌─────────────┘
              │
┌─────────────▼───────────────────┐
│    RECEIVE THREAD (Background)  │
│  - Read continuamente           │
│  - Invocar UI quando chegam msgs│
└─────────────────────────────────┘
```

**Sincronização**:
- ✅ `volatile bool running` — flag para parar thread
- ✅ `object sendLock` — protege escrita simultânea
- ✅ `InvokeRequired/Invoke` — atualizar UI com segurança

**Conceitos utilizados**:
- ✅ Multi-threading (thread de receção)
- ✅ Sincronização com locks
- ✅ Flags volatile
- ✅ Thread-safety na UI
- ✅ Leitura bloqueante (blocking read)

---

### 5️⃣ **Server.cs** (448 linhas)
**Propósito**: Servidor TCP que aceita múltiplos clientes

**Arquitetura**:
```
┌──────────────────────────────────────┐
│      TcpListener (PORT 10000)        │
│  Ciclo infinito: AcceptTcpClient()   │
└────────────┬─────────────────────────┘
             │
             ├─ Cliente 1 → Thread 1 → ClientHandler 1
             ├─ Cliente 2 → Thread 2 → ClientHandler 2
             └─ ClienteN → ThreadN → ClientHandlerN
                    ↓
              Lista<ClientHandler> (partilhada)
              Lock para sincronização
```

**ClientHandler - Responsabilidades**:
- Ler mensagens do cliente
- Processar tipos: USER_OPTION_1 (login), DATA (mensagem), EOT (sair)
- Fazer broadcast para outros clientes
- Limpar recursos ao desligar

**Tipos de Protocolo ProtocolSI**:
```
USER_OPTION_1 → Autenticação (username)
ACK          → Confirmação positiva
DATA         → Dados/mensagens
EOT          → End of Transmission (encerramento)
```

**Fluxo de Broadcast**:
```
Cliente 1 envia DATA "Olá"
    ↓
ClientHandler 1 recebe
    ↓
Itera por todos os clientes na lista
    ↓
Envia "Cliente1: Olá" para Cliente 2, 3, ...
    ↓
Clientes 2, 3, ... recebem na sua thread de receção
```

**Sincronização**:
- ✅ `lock(clientsLock)` ao adicionar/remover clientes
- ✅ `lock(clientsLock)` ao fazer broadcast

**Conceitos utilizados**:
- ✅ TcpListener (servidor TCP)
- ✅ Aceitar múltiplas conexões
- ✅ Thread-per-client (scalabilidade)
- ✅ Broadcast (enviar para todos)
- ✅ Sincronização com locks

---

## 🔑 Conceitos Principais (Explicáveis na Prova Oral)

### 1. **Sockets TCP/IP**
- IPAddress, IPEndPoint, TcpClient, TcpListener
- Connect() vs AcceptTcpClient()
- NetworkStream (bidirecional)

### 2. **Protocolo ProtocolSI**
- Tipos: USER_OPTION_1, ACK, DATA, EOT
- Make() para construir pacotes
- GetCmdType() para interpretar

### 3. **Threading**
- `Thread(ReceiveMessages)` — thread background
- `IsBackground = true` — termina com programa
- `volatile bool running` — flag segura

### 4. **Sincronização**
- `lock()` para proteger estruturas compartilhadas
- `InvokeRequired/Invoke` para thread-safety na UI
- `sendLock` para proteger escrita no stream

### 5. **Event-Driven Architecture**
- `ClientManager.OnClientCountChanged` — event listener
- `FormChat.FormClosed` — event handler
- Desacoplamento entre componentes

### 6. **Padrões de Design**
- **Singleton**: ClientManager (estático)
- **Observer**: Eventos (onClientCountChanged)
- **Thread-Per-Client**: Cada cliente → Thread

---

## ✅ Qualidades do Código

| Aspecto | Status | Notas |
|---------|--------|-------|
| **Documentação** | ✅ Excelente | Comentários em todas secções críticas |
| **Simplicidade** | ✅ Muito simples | Sem patterns complexos, apenas padrões básicos |
| **Thread-Safety** | ✅ Correto | Locks, volatile flags, InvokeRequired |
| **Tratamento de Erros** | ✅ Robusto | Try/catch com mensagens claras |
| **Legibilidade** | ✅ Muito boa | Nomes descritivos, métodos curtos |
| **Separação de Responsabilidades** | ✅ Boa | Cada classe tem uma função clara |

---

## 🎓 Prova Oral - Tópicos Fáceis de Explicar

1. **"Como é que o servidor aceita múltiplos clientes?"**
   → TcpListener + Thread-per-client + ClientHandler

2. **"Como sincroniza o contador de clientes na UI?"**
   → ClientManager (singleton) + eventos + InvokeRequired

3. **"Porquê uma thread separada para receber mensagens?"**
   → Não bloqueia UI enquanto espera por dados

4. **"Como faz broad
