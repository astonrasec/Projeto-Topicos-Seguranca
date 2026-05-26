# 🔄 Arquitetura de Threads - Projeto Chat Seguro

## 📌 Visão Geral

Este projeto utiliza um modelo **multi-threading** para suportar múltiplos clientes conectados simultaneamente ao servidor. Cada cliente e cada conexão servidor-cliente executa em threads separadas para não bloquear operações.

---

## 🎯 Conceitos Base

### Thread Principal (UI Thread)
- **Thread que executa a interface gráfica** (Windows Forms)
- **Responsável por**: Renderizar componentes, capturar eventos de utilizador
- **Limitação**: UI não pode ser atualizada de threads diferentes

### Thread de Background
- **Thread separada da thread de UI**
- **Responsável por**: Operações de I/O (read/write na rede)
- **Vantagem**: Não congela a interface enquanto faz download/upload de dados

### Sincronização
- **Lock**: Protege acesso a recursos partilhados entre threads
- **InvokeRequired/Invoke**: Garante que atualizações de UI ocorrem na thread correta
- **Volatile**: Garante que uma variável é sempre lida do estado global (não cache)

---

## 🖥️ LADO DO CLIENTE - Threads

### Estrutura

```
Application.Run()
    ↓
┌─────────────────────────────────────────────┐
│        Thread Principal da UI (Main)        │
├─────────────────────────────────────────────┤
│  FormLauncher.cs                            │
│  ├─ Renderiza interface                     │
│  ├─ Captura cliques do botão                │
│  ├─ Subscreve evento OnClientCountChanged   │
│  └─ Atualiza label com contador             │
│                                              │
│  FormLogin.cs (quando aberto)               │
│  ├─ Recolhe username e IP                   │
│  ├─ Valida campos                           │
│  └─ Chama buttonConnect_Click()             │
│                                              │
│  FormChat.cs (quando aberto)                │
│  ├─ Mostra caixa de mensagens              │
│  ├─ Captura tecla Enter                     │
│  ├─ Captura clique do botão Enviar          │
│  └─ Atualiza label de status                │
└─────────────────────────────────────────────┘
    ↑
    │ Calls → TcpClient.Connect() [BLOCKING]
    │
    └─ ProtocolSI.Write() [BLOCKING]
```

### FormLogin - Fluxo de Ligação

```
[UI Thread - FormLogin]
    ↓
buttonConnect_Click()
    ├─ Valida username (não vazio)
    ├─ Valida IP (formato correto)
    ├─ Desativa botão (evita cliques duplos)
    ├─ TcpClient.Connect(IP, 10000) ← BLOQUEANTE
    │
    ├─ Cria socket TCP ao servidor
    ├─ Envia username com ProtocolSI
    ├─ Lê resposta (ACK ou NACK) ← BLOQUEANTE
    │
    ├─ Se ACK: 
    │  ├─ Cria FormChat
    │  ├─ Registra cliente em ClientManager
    │  ├─ Subscreve evento FormClosed
    │  └─ Mostra FormChat
    │
    └─ Se erro: Mostra MessageBox e reativa botão
```

### FormChat - Dual Threading

```
┌─────────────────────────────────────────────────────────────────────┐
│                     FormChat (Dual Threading)                       │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  Thread Principal (UI)              Thread de Receção (Background) │
│  ─────────────────────              ────────────────────────────── │
│                                                                      │
│  • Renderiza TextBox                • Lê do NetworkStream          │
│  • Renderiza RichTextBox            • Enquanto running == true     │
│  • Captura eventos de Input         • Bloqueia em Read()           │
│  • buttonSend_Click()               • Se bytesRead == 0: quebra    │
│    └─ SendMessage()                 • Se dados: GetStringFromData()│
│       └─ lock(sendLock)             • AppendMessage(dados)         │
│          └─ Write() a socket        • Se erro: quebra              │
│                                                                      │
│                                    Chama InvokeRequired:           │
│                                    └─ AppendMessage() (UI Thread) │
│                                       └─ RichTextBox.AppendText()  │
│                                                                      │
│  ReceiveThread = new Thread()                                       │
│  receiveThread.IsBackground = true                                  │
│  receiveThread.Start() ← Inicia no construtor                       │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

### Sincronização no Cliente

#### 1. **Lock na Escrita**
```csharp
// FormChat.cs - SendMessage()
lock (sendLock)
{
    byte[] packet = sendProtocol.Make(ProtocolSICmdType.DATA, msg);
    networkStream.Write(packet, 0, packet.Length);
}
```
**Razão**: Impede que dois threads escrevam ao mesmo tempo no socket (corrumperia dados)

#### 2. **InvokeRequired na Leitura**
```csharp
// FormChat.cs - ReceiveMessages() [Background]
private void AppendMessage(string message)
{
    if (richTextBoxChat.InvokeRequired)  // Se não está na UI thread
    {
        richTextBoxChat.Invoke(new Action(() => AppendMessage(message)));
        return;
    }
    richTextBoxChat.AppendText(message + Environment.NewLine);
}
```
**Razão**: Windows Forms não permite atualizar controles de threads diferentes

#### 3. **Volatile Flag**
```csharp
private volatile bool running = true;
```
**Razão**: Garante que quando uma thread altera `running`, a outra vê a alteração imediatamente (não lê do cache)

---

## 🖧 LADO DO SERVIDOR - Threads

### Arquitetura Thread-Per-Client

```
┌──────────────────────────────────────────────────────┐
│        Thread Principal (Main)                       │
├──────────────────────────────────────────────────────┤
│                                                      │
│  Main()                                             │
│  ├─ TcpListener.Start() [Porta 10000]              │
│  ├─ Console.WriteLine("SERVER READY")              │
│  │                                                  │
│  └─ while(true)                                    │
│     ├─ TcpClient tcpClient = listener.AcceptTcpClient() ← BLOQUEANTE
│     ├─ clientCounter++                             │
│     ├─ Console.WriteLine("Client X connected")     │
│     │                                              │
│     └─ ClientHandler handler = new ClientHandler() │
│        └─ handler.Start() ← Cria nova thread       │
│           (volta ao loop para aceitar próximo)     │
│                                                    │
└──────────────────────────────────────────────────────┘
         ↓                    ↓                  ↓
    ┌────────────┐     ┌────────────┐     ┌────────────┐
    │ Thread 1   │     │ Thread 2   │     │ Thread N   │
    │ [Client 1] │     │ [Client 2] │     │ [Client N] │
    ├────────────┤     ├────────────┤     ├────────────┤
    │ Handle()   │     │ Handle()   │     │ Handle()   │
    ├────────────┤     ├────────────┤     ├────────────┤
    │ • Read()   │     │ • Read()   │     │ • Read()   │
    │ • Broadcast│     │ • Broadcast│     │ • Broadcast│
    │ • Close()  │     │ • Close()  │     │ • Close()  │
    └────────────┘     └────────────┘     └────────────┘
```

### ClientHandler - Handle() Method

```csharp
public void Handle()
{
    // ClientHandler.cs
    Thread thread = new Thread(threadHandler);  // Cria thread
    thread.IsBackground = true;                 // Thread de background
    thread.Start();                             // Inicia thread
}

private void threadHandler()
{
    // Executa numa thread separada
    
    NetworkStream networkStream = this.client.GetStream();
    ProtocolSI protocolSI = new ProtocolSI();
    
    while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
    {
        int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
        
        if (bytesRead == 0) break;  // Cliente desconectou
        
        switch (protocolSI.GetCmdType())
        {
            case ProtocolSICmdType.USER_OPTION_1:
                // Recebe username do cliente
                username = protocolSI.GetStringFromData();
                byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                networkStream.Write(ack, 0, ack.Length);
                break;
                
            case ProtocolSICmdType.DATA:
                // Recebe mensagem do cliente
                string msg = protocolSI.GetStringFromData();
                Broadcast($"[{username}]: {msg}");
                break;
        }
    }
    
    // Quando sai do loop: cliente desconectou
    networkStream.Close();
    client.Close();
}
```

### Sincronização no Servidor

#### 1. **Lock na Lista de Clientes**
```csharp
private static readonly List<ClientHandler> clients = new List<ClientHandler>();
private static readonly object clientsLock = new object();

lock (clientsLock)
{
    clients.Add(handler);  // Adiciona novo cliente
}
```
**Razão**: Múltiplas threads ClientHandler podem aceder à lista simultaneamente

#### 2. **Lock na Escrita (Broadcast)**
```csharp
public void Broadcast(string message)
{
    lock (clientsLock)
    {
        foreach (ClientHandler handler in clients)
        {
            handler.SendMessage(message);  // Envia a todos
        }
    }
}

private void SendMessage(string msg)
{
    lock (sendLock)  // Lock individual por cliente
    {
        byte[] packet = protocol.Make(ProtocolSICmdType.DATA, msg);
        networkStream.Write(packet, 0, packet.Length);
    }
}
```
**Razão**: 
- `clientsLock`: Protege iteração da lista durante broadcast
- `sendLock`: Impede que múltiplas mensagens sobrescrevam o socket de um cliente

---

## 📊 Fluxo Completo (Multi-Client)

### Cenário: 2 Clientes Enviando Mensagens Simultaneamente

```
┌─── CLIENTE 1 ────────────────────────────┐
│ Thread: FormChat.SendMessage()           │
│ Action: lock(sendLock) → Write("Oi")    │
└──────────────────┬──────────────────────┘
                   │
                   ├─→ SERVIDOR (Main Thread)
                   │   └─ [Aguardando próximo cliente]
                   │
                   └─→ SERVIDOR (Thread 1)
                       └─ ClientHandler.Handle()
                           │
                           ├─ Read() [recepciona "Oi"]
                           ├─ lock(clientsLock)
                           ├─ Broadcast("Oi") a todos
                           │  ├─→ Envia a Cliente 1
                           │  └─→ Envia a Cliente 2 (Thread 2)
                           │
                           └─ Volta ao loop
                               
┌─── CLIENTE 2 ────────────────────────────┐
│ Thread: FormChat.ReceiveMessages()       │
│ Action: Recebe via networkStream         │
│ Calls: InvokeRequired → AppendMessage()  │
└──────────────────┬──────────────────────┘
                   │
                   └─→ SERVIDOR (Thread 2)
                       └─ ClientHandler.Handle()
                           │
                           ├─ Read() [recepciona mensagem]
                           ├─ lock(clientsLock)
                           ├─ Broadcast() a todos
                           │  ├─→ Envia a Cliente 1
                           │  └─→ Envia a Cliente 2
                           │
                           └─ Volta ao loop
```

---

## ⚠️ Problemas Comuns & Soluções

### Problema 1: UI Congela Durante Read()
```csharp
// ❌ ERRADO
private void Form_Load()
{
    networkStream.Read(buffer, 0, buffer.Length);  // Bloqueia UI
}

// ✅ CORRETO
private void Form_Load()
{
    Thread recvThread = new Thread(ReceiveMessages);
    recvThread.IsBackground = true;
    recvThread.Start();  // Thread separada não bloqueia UI
}
```

### Problema 2: Cross-Thread Exception
```csharp
// ❌ ERRADO
private void ReceiveMessages()  // Background thread
{
    richTextBox.AppendText("Olá");  // CRASH: UI thread violation
}

// ✅ CORRETO
private void ReceiveMessages()  // Background thread
{
    if (richTextBox.InvokeRequired)
    {
        richTextBox.Invoke(new Action(() => 
            richTextBox.AppendText("Olá")
        ));
    }
    else
    {
        richTextBox.AppendText("Olá");
    }
}
```

### Problema 3: Corrupção de Socket
```csharp
// ❌ ERRADO - Múltiplas threads escrevem simultaneamente
private void SendMessage(string msg)
{
    byte[] packet = protocol.Make(ProtocolSICmdType.DATA, msg);
    networkStream.Write(packet, 0, packet.Length);  // RACE CONDITION
}

// ✅ CORRETO - Lock protege escrita
private void SendMessage(string msg)
{
    lock (sendLock)
    {
        byte[] packet = protocol.Make(ProtocolSICmdType.DATA, msg);
        networkStream.Write(packet, 0, packet.Length);
    }
}
```

### Problema 4: Acesso Simultâneo à Lista
```csharp
// ❌ ERRADO
private static List<ClientHandler> clients = new List<ClientHandler>();

// Thread 1 adiciona enquanto Thread 2 itera
clients.Add(newClient);
foreach (var client in clients) { ... }  // CRASH

// ✅ CORRETO
private static readonly object clientsLock = new object();

lock (clientsLock)
{
    clients.Add(newClient);
}

lock (clientsLock)
{
    foreach (var client in clients) { ... }
}
```

---

## 📈 Diagrama Timeline

### Sequência de Eventos Completa

```
T0:  Servidor Start
     └─ Main Thread aguarda ligações (AcceptTcpClient bloqueante)

T1:  Cliente 1 Liga
     └─ Main Thread: Cria Thread-1 para Cliente 1
     └─ Thread-1: Lê username de Cliente 1
     └─ Main Thread: Volta a aguardar (loop)

T2:  Cliente 1 Envia "Oi"
     ├─ Cliente 1 UI Thread: lock(sendLock) → Write("Oi")
     └─ Thread-1: lock(clientsLock) → Broadcast("Oi")
        ├─ Envia a todos (só Cliente 1 neste momento)
        └─ Thread-1: Volta a Read()

T3:  Cliente 2 Liga
     └─ Main Thread: Cria Thread-2 para Cliente 2
     └─ Thread-2: Lê username de Cliente 2
     └─ Main Thread: Volta a aguardar

T4:  Cliente 2 Envia "Olá"
     ├─ Cliente 2 UI Thread: lock(sendLock) → Write("Olá")
     └─ Thread-2: lock(clientsLock) → Broadcast("Olá")
        ├─ lock(clientsLock)
        ├─ Envia a Cliente 1 (thread separada, Background)
        ├─ Envia a Cliente 2 (ele próprio)
        └─ Thread-2: Volta a Read()

T5:  Cliente 1 Recebe "Olá"
     └─ Cliente 1 Background Thread: networkStream.Read()
     └─ Recebe "Olá" do servidor
     └─ InvokeRequired → AppendMessage("Olá")
     └─ UI atualiza RichTextBox (sem congelar)

T6:  Cliente 1 Desconecta
     └─ Cliente 1 Envia EOT
     └─ Thread-1: while loop detecta EOT
     └─ Thread-1: Close() e sai
     └─ Main Thread: Continua aguardando próxima ligação
```

---

## 🎓 Aprendizados-Chave

1. **Thread-Per-Client é eficiente** para 10-100 clientes
2. **Locks são necessários** para proteger recursos partilhados
3. **InvokeRequired é mandatório** em Windows Forms
4. **IsBackground = true** garante saída limpa quando o processo termina
5. **Volatile é importante** para flags entre threads
6. **Read() é bloqueante** — deve estar em thread separada

---

## 📚 Referências

- [Microsoft: Threading in C#](https://learn.microsoft.com/en-us/dotnet/api/system.threading)
- [Thread Synchronization](https://learn.microsoft.com/en-us/dotnet/standard/threading/overview-of-synchronization-primitives)
- [Windows Forms Thread Safety](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls)
- [TcpClient Async Pattern](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient)
