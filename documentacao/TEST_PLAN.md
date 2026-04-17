# 🧪 Test Plan - Chat System Verification

**Date**: 17 April 2026  
**Objective**: Verify C# code compiles and runs correctly with multiple simultaneous clients  
**Status**: In Progress

---

## Test Environment

- **Platform**: Windows 10/11
- **Runtime**: .NET Framework 4.8+
- **ChatServer.exe**: `C:\Users\Diogo\OneDrive - IPLeiria\Documentos\Git\Projeto-Topicos-Seguranca\TrabalhoPratico\ChatServer\bin\Debug\ChatServer.exe`
- **ChatClient.exe**: `C:\Users\Diogo\OneDrive - IPLeiria\Documentos\Git\Projeto-Topicos-Seguranca\TrabalhoPratico\ChatClient\bin\Debug\ChatClient.exe`
- **Port**: TCP 10000

---

## Test Scenarios

### Test 1: Server Startup (Connectivity)
- **Objective**: Verify server starts and listens on port 10000
- **Steps**:
  1. Run `ChatServer.exe`
  2. Wait 2 seconds for initialization
  3. Verify port 10000 is listening (netstat -ano | findstr 10000)
  4. Expected: Server should print "Server started" or similar message
- **Pass Criteria**: Server listens on 10000 without errors
- **Status**: ⏳ Pending

### Test 2: Single Client Connection
- **Objective**: Verify one client can connect to server
- **Steps**:
  1. Start server (Test 1)
  2. Start one ChatClient.exe
  3. Client should display login window
  4. Wait 3 seconds for connection attempt
  5. Expected: No connection errors in console/logs
- **Pass Criteria**: Client connects without errors
- **Status**: ⏳ Pending

### Test 3: Client Login
- **Objective**: Verify authentication flow works
- **Steps**:
  1. From Test 2, with client connected
  2. Enter test credentials (username: test, password: test)
  3. Click login button
  4. Expected: Chat window opens or login succeeds
- **Pass Criteria**: Login succeeds or shows appropriate error
- **Status**: ⏳ Pending

### Test 4: Message Sending (Single Client)
- **Objective**: Verify single client can send message without errors
- **Steps**:
  1. From Test 3, with client logged in
  2. Type "Hello Server" in message box
  3. Click Send or press Enter
  4. Expected: Message is sent without crashing
- **Pass Criteria**: No exceptions thrown
- **Status**: ⏳ Pending

### Test 5: Multiple Clients (2 clients)
- **Objective**: Verify server handles multiple clients simultaneously
- **Steps**:
  1. Start server
  2. Start Client1 and login
  3. Start Client2 and login
  4. Wait 5 seconds
  5. Both clients should be connected
  6. Expected: Server shows both connections (in logs or console)
- **Pass Criteria**: Both clients connected without errors
- **Status**: ⏳ Pending

### Test 6: Message Broadcasting (2 clients)
- **Objective**: Verify messages broadcast between clients
- **Steps**:
  1. From Test 5, with 2 clients connected
  2. Client1 sends "Message from Client1"
  3. Client2 sends "Message from Client2"
  4. Expected: Both clients see all messages
- **Pass Criteria**: Broadcasting works correctly
- **Status**: ⏳ Pending

### Test 7: Multiple Clients (5+ clients)
- **Objective**: Verify server handles high concurrency
- **Steps**:
  1. Start server
  2. Start 5+ clients simultaneously
  3. Wait 10 seconds for all connections
  4. Each client sends 1 message
  5. Wait for all messages to propagate
  6. Expected: No crashes, all clients receive all messages
- **Pass Criteria**: Handles 5+ simultaneous clients without errors
- **Status**: ⏳ Pending

### Test 8: Client Disconnect
- **Objective**: Verify server handles client disconnection gracefully
- **Steps**:
  1. From Test 5, with 2 clients connected
  2. Close Client1 window abruptly
  3. Wait 3 seconds
  4. Client2 sends a message
  5. Expected: Server doesn't crash, Client2 still works
- **Pass Criteria**: No unhandled exceptions on disconnect
- **Status**: ⏳ Pending

### Test 9: Server Shutdown
- **Objective**: Verify graceful server shutdown
- **Steps**:
  1. From Test 5, with clients connected
  2. Press Ctrl+C in server console or close window
  3. Expected: Server closes all connections gracefully
  4. Clients should detect disconnect within 5 seconds
- **Pass Criteria**: No dangling connections or resources
- **Status**: ⏳ Pending

### Test 10: Code Quality Verification
- **Objective**: Verify code has no compilation warnings/errors
- **Steps**:
  1. Open Visual Studio
  2. Load TrabalhoPratico.sln
  3. Clean and rebuild all projects
  4. Check Output window for errors/warnings
  5. Expected: 0 Errors, minimal warnings
- **Pass Criteria**: Compiles cleanly (warnings acceptable if documented)
- **Status**: ⏳ Pending

---

## Test Execution Log

### Execution Summary
- **Total Tests**: 10
- **Passed**: ⏳ 0
- **Failed**: ⏳ 0
- **Skipped**: ⏳ 0

### Individual Results

| Test | Result | Notes | Duration |
|---|---|---|---|
| 1. Server Startup | ⏳ Pending | | |
| 2. Single Client | ⏳ Pending | | |
| 3. Client Login | ⏳ Pending | | |
| 4. Message Send | ⏳ Pending | | |
| 5. 2 Clients | ⏳ Pending | | |
| 6. Broadcasting | ⏳ Pending | | |
| 7. 5+ Clients | ⏳ Pending | | |
| 8. Disconnect | ⏳ Pending | | |
| 9. Server Shutdown | ⏳ Pending | | |
| 10. Code Quality | ⏳ Pending | | |

---

## Issues Found

*(To be updated during testing)*

- [ ] None yet

---

## Code Quality Checks

- **Compilation**: ⏳ Pending
- **Warnings**: ⏳ Pending
- **Static Analysis**: ⏳ Pending
- **Thread Safety**: ⏳ Pending (via code review)
- **Exception Handling**: ⏳ Pending (via execution)

---

## Recommendations

After testing completes:
1. Fix any critical bugs found
2. Optimize performance if needed
3. Add logging for debugging
4. Document known limitations
5. Prepare demo for oral presentation

---

**Test Started**: 17 April 2026  
**Test Completed**: ⏳ Pending  
**Overall Status**: 🟡 In Progress
