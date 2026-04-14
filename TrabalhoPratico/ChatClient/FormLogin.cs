// ============================================================
// BIBLIOTECAS / NAMESPACES UTILIZADOS
// ============================================================
// EI.SI           → biblioteca ProtocolSI para construir/interpretar pacotes
// System.Net      → IPAddress, IPEndPoint
// System.Net.Sockets → TcpClient, NetworkStream
// System.Windows.Forms → Form, MessageBox, TextBox, Button, etc.
// ============================================================
using EI.SI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ChatClient
{
    // ============================================================
    // CONCEITO: Windows Forms
    //
    // Uma aplicação Windows Forms é baseada em eventos.
    // O programa corre num ciclo de mensagens (message loop) e
    // quando o utilizador faz algo (clica, escreve, fecha a janela),
    // é chamado o "event handler" correspondente.
    //
    // A classe FormLogin é "partial" — o código dos controlos
    // gráficos (botões, textboxes, etc.) está no ficheiro
    // FormLogin.Designer.cs, gerado automaticamente.
    // ============================================================

    /// <summary>
    /// Formulário de Login — primeiro ecrã da aplicação cliente.
    ///
    /// RESPONSABILIDADE:
    ///   - Recolher o nome de utilizador e o IP do servidor
    ///   - Validar os campos antes de tentar ligar
    ///   - Estabelecer a ligação TCP com o servidor
    ///   - Enviar o username ao servidor (handshake inicial)
    ///   - Aguardar confirmação (ACK) e abrir o FormChat
    /// </summary>
    public partial class FormLogin : Form
    {
        // --------------------------------------------------------
        // A porta tem de ser IGUAL à do servidor.
        // Ambos devem usar a porta 10000 para comunicar.
        // --------------------------------------------------------
        private const int PORT = 10000;

        /// <summary>
        /// Construtor: chamado quando a aplicação arranca.
        /// InitializeComponent() cria todos os controlos gráficos
        /// definidos no FormLogin.Designer.cs.
        /// </summary>
        public FormLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// EVENT HANDLER do botão "Conectar".
        ///
        /// Este método é chamado AUTOMATICAMENTE pelo Windows quando
        /// o utilizador clica no botão. O event handler recebe dois
        /// parâmetros por convenção: o objeto que gerou o evento (sender)
        /// e informação adicional sobre o evento (e).
        ///
        /// FLUXO COMPLETO:
        ///   1. Ler e validar os campos do formulário
        ///   2. Criar ligação TCP ao servidor (IPEndPoint + TcpClient)
        ///   3. Obter o NetworkStream (canal de comunicação)
        ///   4. Criar instância de ProtocolSI
        ///   5. Enviar pacote USER_OPTION_1 com o username
        ///   6. Aguardar ACK do servidor
        ///   7. Se ACK recebido → abrir FormChat, esconder este formulário
        /// </summary>
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            // Ler os valores dos campos e remover espaços em branco extra
            string username = textBoxUsername.Text.Trim();
            string ip       = textBoxIP.Text.Trim();

            // --------------------------------------------------------
            // VALIDAÇÃO DOS CAMPOS
            // É boa prática validar inputs do utilizador ANTES de tentar
            // qualquer operação de rede — evita erros desnecessários.
            // --------------------------------------------------------
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Por favor introduza um nome de utilizador.",
                    "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxUsername.Focus(); // Colocar cursor no campo em falta
                return;                 // Sair do método sem fazer nada
            }

            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("Por favor introduza o endereço IP do servidor.",
                    "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxIP.Focus();
                return;
            }

            // Desativar o botão enquanto tenta ligar — impede cliques duplos
            // que criariam duas ligações ao mesmo tempo
            buttonConnect.Enabled = false;

            try
            {
                // --------------------------------------------------------
                // PASSO 1: CRIAR O ENDPOINT
                //
                // IPAddress.Parse(ip) → converte a string "127.0.0.1"
                //                       para um objeto IPAddress
                // IPEndPoint           → combina IP + Porta num único objeto
                //                       que identifica o destino da ligação
                // --------------------------------------------------------
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), PORT);

                // --------------------------------------------------------
                // PASSO 2: CRIAR E LIGAR O TcpClient
                //
                // TcpClient → implementa o protocolo TCP do lado do cliente.
                // Connect() → tenta estabelecer a ligação TCP ao servidor.
                //             BLOQUEIA até conseguir ligar ou falhar.
                //             Se o servidor não estiver a correr, lança exceção.
                //
                // TCP (Transmission Control Protocol):
                //   - Protocolo orientado à ligação (connection-oriented)
                //   - Garante entrega ordenada e sem erros
                //   - Usa o mecanismo de "three-way handshake" (SYN, SYN-ACK, ACK)
                // --------------------------------------------------------
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(endpoint);

                // --------------------------------------------------------
                // PASSO 3: OBTER O NETWORKSTREAM
                //
                // GetStream() devolve o canal bidirecional de leitura/escrita.
                // É através deste stream que enviamos e recebemos todos os dados.
                // --------------------------------------------------------
                NetworkStream stream = tcpClient.GetStream();

                // --------------------------------------------------------
                // PASSO 4: CRIAR INSTÂNCIA DO PROTOCOLSI
                //
                // ProtocolSI é a biblioteca fornecida que define:
                //   - Como construir pacotes (Make)
                //   - Como interpretar pacotes recebidos (GetCmdType, GetStringFromData)
                //   - O Buffer onde os dados chegam (Buffer)
                // --------------------------------------------------------
                ProtocolSI protocol = new ProtocolSI();

                // --------------------------------------------------------
                // PASSO 5: ENVIAR O USERNAME AO SERVIDOR
                //
                // Make(USER_OPTION_1, username) → cria um pacote com:
                //   - Cabeçalho: tipo USER_OPTION_1
                //   - Dados: o texto do username
                //
                // stream.Write(packet, 0, packet.Length):
                //   - packet   → o array de bytes a enviar
                //   - 0        → índice de início no array
                //   - packet.Length → quantos bytes enviar
                //
                // USER_OPTION_1 é usado por convenção para identificação.
                // O servidor ao receber USER_OPTION_1 sabe que são
                // credenciais/identificação do cliente.
                // --------------------------------------------------------
                byte[] packet = protocol.Make(ProtocolSICmdType.USER_OPTION_1, username);
                stream.Write(packet, 0, packet.Length);

                // --------------------------------------------------------
                // PASSO 6: AGUARDAR ACK DO SERVIDOR
                //
                // stream.Read(buffer, offset, count) → BLOQUEIA até chegarem
                // dados. Quando chegam, preenche protocol.Buffer com os bytes.
                //
                // ACK (Acknowledgment) = confirmação positiva.
                // O servidor envia ACK para dizer "username aceite, podes entrar".
                // --------------------------------------------------------
                stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);

                // --------------------------------------------------------
                // PASSO 7: VERIFICAR A RESPOSTA
                // --------------------------------------------------------
                if (protocol.GetCmdType() == ProtocolSICmdType.ACK)
                {
                    // --------------------------------------------------------
                    // SERVIDOR ACEITOU → Abrir o formulário de chat
                    //
                    // Passamos ao FormChat tudo o que ele precisa:
                    //   - tcpClient: para fechar a ligação no fim
                    //   - stream: para continuar a comunicação
                    //   - protocol: para construir pacotes de envio
                    //   - username: para mostrar na interface
                    // --------------------------------------------------------
                    FormChat chatForm = new FormChat(tcpClient, stream, protocol, username);

                    // Quando o FormChat fechar, terminar a aplicação completamente.
                    // Como o FormLogin ficou escondido (Hide), sem isto a aplicação
                    // ficaria "presa" em memória mesmo sem janelas visíveis.
                    chatForm.FormClosed += (s, args) => Application.Exit();

                    chatForm.Show(); // Mostrar a janela de chat
                    this.Hide();    // Esconder o ecrã de login (não fechar!)
                }
                else
                {
                    // O servidor respondeu com outro tipo — ligação recusada
                    MessageBox.Show("O servidor não aceitou a ligação.",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    stream.Close();
                    tcpClient.Close();
                    buttonConnect.Enabled = true; // Permitir nova tentativa
                }
            }
            catch (FormatException)
            {
                // IPAddress.Parse() lança FormatException se o IP for inválido
                // Ex: "abc.def.ghi.jkl" não é um IP válido
                MessageBox.Show("Endereço IP inválido. Exemplo: 127.0.0.1",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonConnect.Enabled = true;
            }
            catch (Exception ex)
            {
                // Qualquer outro erro: servidor não está a correr, porta errada, etc.
                MessageBox.Show("Erro ao conectar ao servidor:\n" + ex.Message,
                    "Erro de Ligação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonConnect.Enabled = true;
            }
        }

        /// <summary>
        /// EVENT HANDLER de tecla no campo Username.
        ///
        /// Quando o utilizador prime Enter neste campo, move o foco
        /// para o campo IP (navegação conveniente sem rato).
        ///
        /// e.KeyChar == (char)Keys.Return → verifica se foi a tecla Enter
        /// e.Handled = true → diz ao Windows para NÃO processar mais
        ///                     esta tecla (evita som de "beep" ou nova linha)
        /// </summary>
        private void textBoxUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                textBoxIP.Focus(); // Mover cursor para o campo do IP
            }
        }

        /// <summary>
        /// EVENT HANDLER de tecla no campo IP.
        ///
        /// Quando o utilizador prime Enter no campo do IP, é como
        /// clicar no botão Conectar — chama diretamente o handler.
        /// </summary>
        private void textBoxIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                buttonConnect_Click(sender, e); // Simular clique no botão
            }
        }
    }
}
