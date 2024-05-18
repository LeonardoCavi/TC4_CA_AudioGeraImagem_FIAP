<h1 align="left">[Tech Challenge 04] - Arquitetura Limpa + TDD - Projeto de Microsserviços e comunicações por Mensageria - FIAP 2024 - Pós Tech</h1>
➤ O projeto 'Gerador de Imagem por Voz com Microsserviços e Mensageria' consiste em duas soluções que se comunicam por meio de mensageria. 
Os usuários têm a capacidade de fornecer um áudio, o qual é então transcrito para texto por meio de tecnologias 
de Inteligência Artificial (IA). Posteriormente, utilizando esse texto como base, é gerada a imagem correspondente e 
disponibilizada para consumo.

<h2 align="left">Integrantes</h3>
- ➤ <a href="https://github.com/talles2512">Hebert Talles de Jesus Silva</a> - RM352000 </br> 
- ➤ <a href="https://github.com/LeonardoCavi">Leonardo Cavichiolli de Oliveira</a> - RM351999 </br>

<h2 align="left">Projetos</h3>
- ➤ AudioGeraImagemAPI</br>
- ➤ AudioGeraImagemWorker</br>

<h2 align="left">Vídeo de Apresentação</h2>
<a href="https://youtu.be/JwXN584B2cg">Apresentação do Projeto</a></br> 

<h2 align="left">Diagrama do Projeto</h2>
<img width="1200" src="https://github.com/LeonardoCavi/TC4_CA_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Diagramas/Diagrama_integracoes.png"></img></br>

<h2 align="left">Requisitos do Sistema do Projeto</h2>

<h4 align="left">➤ Transcrição de Áudio</h4>
- ➤ 1. A API deve ser capaz de receber arquivos de áudio nos formatos suportados. Os áudios permitidos devem se limitar à extensão (.mp3).</br>
- ➤ 2. Deve utilizar serviços de transcrição de voz baseados em IA para converter o áudio em texto de maneira precisa e eficiente. O projeto usufruiu da tecnologia de IA do OpenAI utilizando o modelo treinado Whisper.</br>
- ➤ 3. A transcrição deve suportar áudios de até 5 minutos de duração, com uma taxa mínima de precisão de 90%.</br>
- ➤ 4. O áudio enviado pode conter diferentes linguagens, sendo compatível com 90 linguagens conforme documentação da OpenAI (maio de 2024).</br>

<h4 align="left">➤ Geração de Imagem</h4>
- ➤ 1. Utilizando o texto transcrito, a API deve ser capaz de gerar uma imagem relacionada ao conteúdo do áudio.</br>
- ➤ 2. A geração de imagem deve ser realizada por meio de modelos de IA utilizando o modelo treinado Dall-e 3 para realizar a criação da imagem.</br>
- ➤ 3. As imagens geradas devem refletir corretamente o conteúdo do áudio, de acordo com o contexto e as palavras-chave identificadas na transcrição.</br>
- ➤ 4. Será gerada uma imagem por áudio, com resolução de 1024x1024 pixels.</br>

<h4 align="left">➤ Comunicação por Mensageria</h4>
- ➤ 1. A comunicação entre a API e o Worker (Microsserviço) deve ser realizada por meio de um sistema de mensageria baseado em filas, como RabbitMQ.</br>
- ➤ 2. A exchange e a fila deverá ser criada de forma automatica pelos serviços de API ou Worker, onde será utilizado as configurações padrões de criação Direct (exchange) e durable (queue).</br>
- ➤ 3. As mensagens devem conter informações essenciais, como o identificador do áudio e o texto transcritos, para permitir o processamento adequado no Worker.</br>
- ➤ 4. Além disso, as mensagens devem incluir informações sobre o estado do processamento, permitindo que o Worker realize a devida tratativa internamente.</br>

<h4 align="left">➤ Armazenamento em Azure Blob Storage</h4>
- ➤ 1. O Worker deve armazenar os arquivos de áudio e as imagens geradas em uma conta Azure Blob Storage de forma segura e eficiente.</br>
- ➤ 2. Os arquivos devem ser organizados em estruturas de diretórios apropriadas para facilitar o acesso e a recuperação posterior, conforme necessário.</br>

<h4 align="left">➤ Armazenamento no Banco de Dados Relacional</h4>
- ➤ 1. A API e o Worker devem armazenar no banco de dados SQL Server informações importante para cada etapa de processamento até a geração da imagem.</br>
- ➤ 2. A cada troca de estado, o sistema armazena informações importante relacionadas a transcrição e geração da imagem.</br>
- ➤ 3. Deve ser utilizado o Entity Framework Core para interagir com o banco de dados de forma eficiente e segura.</br>
- ➤ 4. A escolha do banco de dados relacional SQL Server é devido as principais vantagens.: Segurança, Escalabilidade, Ferramenta de Gerenciamento e no caso principal de nosso projeto a fácil integração com o ecossistema .NET.</br>

<h4 align="left">➤ Funcionalidade de Retentativa (Worker - Microsserviço)</h4>
- ➤ 1. Em caso de falha durante o processamento de uma mensagem, o Worker deve ser capaz de reagendar a mensagem para a fila de retentativa.</br>
- ➤ 2. O sistema deve ser configurado para tentativas de retentativa automáticas, com um intervalo de pelo menos 20 segundos entre tentativas consecutivas.</br>
- ➤ 3. Após um máximo de 3 tentativas de retentativa sem sucesso, a mensagem deve ser marcada como "Falha" e registrada para análise e resolução manual.</br>

<h2 align="left">Critérios de Aceitação do Projeto</h2>
<h4 align="left">➤ Transcrição de Áudio</h4>
<h4 align="left">-➤ Critério</h4>
-- ➤ 1. Enviar um áudio, para que se inicie a etapa de geração de áudio em imagem. Nesse processo, o sistema irá criar um novo comando, no qual será processado por um microserviço, responsável por fazer o intermédio.</br>
-- ➤ 2. O sistema deve ser capaz de transcrever com precisão áudios em pelo menos 59 idiomas diferentes com precisão de 90%. Existem outros 31 idiomas porém tem taxa de precisão < 50%.</br>
-- ➤ 3. Deverá ser obrigatório a definição de uma descrição para a criação no ato de envio do áudio.</br>
-- ➤ 4. Os áudios permitidos devem se limitar à extensão (.mp3).</br>
<h4 align="left">➤ Verificação</h4>
-- ➤ 1. Realizar testes com áudios em idiomas variados e contextos descritos e verificar a precisão da transcrição em cada caso.</br>
-- ➤ 2. Criação de testes unitários para facilitar a validação.</br>

<h4 align="left">➤ Geração de Imagem</h4>
<h4 align="left">-➤ Critério</h4>
-- ➤ 1. Após a transcrição de áudio, com o texto coletado deverá se iniciar a geração da imagem a partir do texto.</br>
-- ➤ 2. Após a criação a imagem/criação será armazenada e o sistemas disponibilizará um Id para o usuário para que seja possível o resgate posterior.</br>
-- ➤ 3. O usuário também terá opção de listar as últimas 50 criações/imagens e partir da descrição identifica-la, coletar seu Id e resgata-la.</br>
<h4 align="left">➤ Verificação</h4>
-- ➤ 1. Realizar uma pesquisa de satisfação interno com a equipe de desenvolvimento.</br>
-- ➤ 2. Realizar uma pesquisa de satisfação com os usuários finais para obter feedback sobre a qualidade das imagens geradas.</br>
-- ➤ 3. Os testes unitários/integridade serviram apenas para garantir que a imagem está sendo criada, mas não irá garantir a qualidade e solicitação.</br>

<h4 align="left">➤ Comunicação por Mensageria</h4>
<h4 align="left">-➤ Critério</h4>
-- ➤ 1. O tempo médio de processamento das mensagens, do envio à conclusão do processamento, não deve exceder 2 minutos.</br>
-- ➤ 2. O projeto consta com retentativas e reprocessamentos, porém depende de sistemas externos (Azure Blob e Endpoint OpenAI). Esses fatores externos podem interferir no tempo total.</br>
<h4 align="left">➤ Verificação</h4>
-- ➤ 1. Monitorar o tempo de processamento e estados das mensagens em diferentes momentos e condições para garantir que esteja dentro do limite aceitável.</br>
-- ➤ 2. Realizar testes de integração com os serviços externos de armazenamento (Azure Blob) e de transcrição e geração de imagem.</br>

<h4 align="left">➤ Armazenamento em Azure Blob Storage</h4>
<h4 align="left">-➤ Critério</h4>
-- ➤ 1. Os arquivos de áudio e imagens devem estar disponíveis para acesso e download a qualquer momento, com uma taxa de disponibilidade de pelo menos 99%.</br>
-- ➤ 2. Os arquivos de áudio e vídeo serão armazenados em subpastas de áudios e imagens e nomeados com Id de criação mais sua respectiva extensão (.mp3 ou .jpeg).</br>
<h4 align="left">➤ Verificação</h4>
-- ➤ 1. Monitorar a disponibilidade e a integridade dos arquivos armazenados na conta Azure Blob Storage e registrar qualquer ocorrência de falha ou inacessibilidade (Via ticket/suporte Microsoft).</br>
-- ➤ 2. Realizar testes de integração para garantir que o projeto consegue buscar/obter o objeto e inserir/armazenar um novo.</br>

<h4 align="left">➤ Armazenamento no Banco de Dados Relacional</h4>
<h4 align="left">-➤ Critério</h4>
-- ➤ 1. O banco de dados terá duas tabelas distintas, uma para armazenar as criações e outra para administração de estados e data/hora da mesma.</br>
-- ➤ 2. O banco também irá conta com uma tabela destinada apenas para armazenar as versões e alterações da estrutura das tabelas do projeto que utiliza EF Core.</br>
<h4 align="left">➤ Verificação</h4>
-- ➤ 1. Realizar testes de carga/integração para garantir que os dados estão sendo inseridos de forma adequada no banco de dados.</br>

<h4 align="left">➤ Funcionalidade de Retentativa (Worker - Microsserviço)</h4>
<h4 align="left">-➤ Critério</h4>
-- ➤ 1. Todas as mensagens que falharem devem ser reprocessadas, sendo encaminhadas para a fila de tentativas novamente.</br>
-- ➤ 2. O reprocessamente tem como objetivo garantir que caso haja alguma instabilidade, após 20 segundos, será reprocessado.</br>
<h4 align="left">➤ Verificação</h4>
-- ➤ 1. Analisar os registros de mensagens com falha e verificar a taxa de sucesso das tentativas para garantir que está de acordo.</br>
-- ➤ 2. Realizar testes para garantir que caso haja mais de 3 tentativas, o processo seja movido para falha.</br>

<h2 align="left">Testes</h3>
➤ <a href="https://leonardocavi.github.io/TC4_CA_AudioGeraImagem_FIAP/AudioGeraImagemAPI/AudioGeraImagemAPI.Test/coveragereport/index.html"> Testes de Cobertura da API</a></br>
➤ <a href="https://leonardocavi.github.io/TC4_CA_AudioGeraImagem_FIAP/AudioGeraImagemWorker/AudioGeraImagemWorker.Test/coveragereport/index.html"> Testes de Cobertura do Microsserviço (Worker)</a>

<h2 align="left">Documentação do Projeto</h2>
<h4 align="left">Projeto - AudioGeraImagemAPI</h4>
➤ A API foi construída com o framework .NET Core 7, seguindo a abordagem de Arquitetura Limpa, e desenvolvida na IDE Visual Studio 2022. Para gerenciar o banco de dados, optamos pelo Entity Framework Core, utilizando o SQL Server como sistema de gerenciamento de banco de dados.

Uma característica importante da API é a integração dos pacotes MassTransit.RabbitMQ, que facilitam a publicação de mensagens em filas. Essa integração permite que o segundo projeto consuma os dados de forma eficiente, contribuindo para a robustez e eficácia do sistema como um todo.

<h4 align="left">AudioGeraImagemWorker</h4>
➤ O Projeto Worker foi desenvolvido utilizando o framework .NET Core 7 e a IDE Visual Studio 2022, adotando a arquitetura de Arquitetura Limpa. Ele faz uso do Entity Framework para gerenciar o SQL Server e sua principal função é atuar como consumidor, processando mensagens publicadas e realizando integrações com APIs externas.

Neste contexto específico, ele se integra com o OpenAI para transcrição de áudios (whisper) e geração de imagens (dalle-3). Além disso, o Worker armazena tanto os áudios quanto as imagens em uma conta Azure Storage Account (Blob).

<h3 align="left">Instruções do projeto - Preparação</h3>
<h4 align="left">1. RabbitMQ</h4>
- ➤ Requisitos.: Docker Desktop instalado e em execução em seu sistema.</br>
- ➤ 1. Abra um terminal ou prompt de comando.</br>
- ➤ 2. Execute o comando.: <code>docker pull masstransit/rabbitmq:latest</code></br>
- ➤ 3. Aguarde a instalação da imagem.</br>
- ➤ 4. Execute o comando.: <code>docker run -d --name meu-rabbitmq -p 5672:5672 -p 15672:15672 masstransit/rabbitmq</code></br>

<h4 align="left">2. Azure Storage Accontou BLOB</h4>
- ➤ Requisitos.: Uma conta cadastrada no portal do Azure.</br>
- ➤ 1. Acesse o portal do azure em <a href="https://portal.azure.com">Azure Portal</a> e faça login na sua conta.</br>
- ➤ 2. Selecione "Criar um recurso".</br>
- ➤ 3. Selecione "Conta de Armazenamento" nos resultados da pesquisa e clique em "Criar".</br>
- ➤ 4. Após criar sua conta de armazenamento, acesse o recurso recem criado.</br>
- ➤ 5. Após isso, novamente na aba á esquerda selecione "Chave de acesso de armazenamento".</br>
- ➤ 6. Clique no ícone de cópia ao lado da cadeia de conexão para copiá-la para a área de transferência (Salve essa string de conexão
pois iremos utilizar posteriormente na configuração do Worker).</br>
- ➤ 7. Finalizando, navegue até a guia Configurações.</br>
- ➤ 8. Marque para habilitar a opção "Permitir acesso anonimo ao Blob".</br>

<h4 align="left">3. AudioGeraImagemAPI</h4>
➤ Existem alguns passos iniciais antes de começar utilizar o projeto, primeiramente é importante verificar o arquivo de configuração 
da API (<code>appsettings.json</code> ou <code>appsettings.Development.json</code>) e lá tem algumas informações importantes que devemos prestar atenção.: </br>
- ➤ <i>ConnectionStrings:ApplicationConnectionString</i>.: String de conexão do banco de dados.</br>
- ➤ <i>MassTransit:NomeFila</i>.: Defina aqui o nome da fila na qual a API publicará mensagens no RabbitMQ. 
É importante observar que este nome deve corresponder ao definido no Worker.</br>
- ➤ <i>MassTransit:Servidor</i>.: Especifique o servidor onde o RabbitMQ está em execução. 
Por padrão, é configurado como localhost no projeto.</br>
- ➤ <i>MassTransit:Usuario</i>.: Forneça o nome de usuário para a conexão com o RabbitMQ. 
Se você estiver utilizando algo diferente de um ambiente local, será necessário criar um usuário 
personalizado, pois o usuário padrão "guest" não funcionará.</br>
- ➤ <i>MassTransit:Senha</i>.: Insira a senha para a conexão com o RabbitMQ. Da mesma forma que o 
usuário, se você estiver usando um ambiente diferente do local, é necessário criar uma 
senha personalizada, pois a senha padrão "guest" não funcionará.</br>

<h4 align="left">4. AudioGeraImagemWorker</h4>
➤ Existem alguns passos iniciais antes de começar utilizar o projeto, primeiramente é importante verificar o arquivo de configuração 
do Worker (<code>appsettings.json</code> ou <code>appsettings.Development.json</code>) e lá tem algumas informações importantes que devemos prestar atenção.: </br>
- ➤ <i>ConnectionStrings:ApplicationConnectionString</i>.: String de conexão do banco de dados (Igual a string de conexão da API).</br>
- ➤ <i>MassTransit:NomeFila</i>.: Defina aqui o mesmo nome da fila fornecido na API, para que seja possível o Worker conseguir fazer seu papel de consumidor.</br>
- ➤ <i>MassTransit:Servidor</i>.: Especifique o servidor onde o RabbitMQ está em execução. 
Por padrão, é configurado como localhost no projeto.</br>
- ➤ <i>MassTransit:Usuario</i>.: Forneça o nome de usuário para a conexão com o RabbitMQ. 
Se você estiver utilizando algo diferente de um ambiente local, será necessário criar um usuário 
personalizado, pois o usuário padrão "guest" não funcionará.</br>
- ➤ <i>MassTransit:Senha</i>.: Insira a senha para a conexão com o RabbitMQ. Da mesma forma que o 
usuário, se você estiver usando um ambiente diferente do local, é necessário criar uma 
senha personalizada, pois a senha padrão "guest" não funcionará.</br>
- ➤ <i>AzureBlobConfiguration:ConnectionString</i>.: Insira aqui a string de conexão que você copiou ou salvou após criar a conta de armazenamento do azure.</br>
- ➤ <i>AzureBlobConfiguration:ContainerName</i>.: Defina qual o nome desejado para a criação automática do contêiner</br>
- ➤ <i>OpenAI:SecretKey</i>.: Chave secreta para o consumo das APIS do OpenAI.</br>

<h4 align="left">5. Scripts de Banco de Dados</h4>
➤ Executar o seguinte script na base de dados.: <a href="https://github.com/LeonardoCavi/TC4_CA_AudioGeraImagem_FIAP/tree/developer/Documenta%C3%A7%C3%B5es/Tabelas/script%20tabelas.sql">Script das Tabelas</a></br>

<h4 align="left">Iniciando o projeto</h4>
➤ Realizado todas as configurações, ambos projetos devem ser iniciados. Após esse processo, é possível realizar testes com áudios previamente gravados.: <a href="https://github.com/LeonardoCavi/TC4_CA_AudioGeraImagem_FIAP/tree/developer/Documenta%C3%A7%C3%B5es/%C3%81udios%20de%20exemplo"> Áudios de Exemplo</a>.</br>
➤ Swagger para testar a API.: <code>https://localhost:[porta]/swagger/index.html</code></br>

<img width="1200" src="https://github.com/LeonardoCavi/TC4_CA_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Util/SwaggerAPI.png"></img></br>
<img width="1200" src="https://github.com/LeonardoCavi/TC4_CA_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Util/GerandoImagemSwaggerAPI.png"></img></br>
<img width="1200" src="https://github.com/LeonardoCavi/TC4_CA_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Util/ObtendoImagemSwaggerAPI.png"></img></br>

<h4 align="left">Diagrama do banco de dados</h4>
<img width="1200" src="https://github.com/LeonardoCavi/TC4_CA_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Tabelas/diagramaServices.png"></img>

<h4 align="left">Diagrama de funcionamento dos microsserviços</h4>
<img width="1200" src="https://github.com/LeonardoCavi/TC4_CA_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Diagramas/TC4.drawio.png"></img>

<h4 align="left">Documentações</h4>
<a href="https://github.com/LeonardoCavi/TC4_CA_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Tabelas/Descritivo%20das%20Tabelas.txt">Descritivo das Tabelas do Projeto</a>.</br>

