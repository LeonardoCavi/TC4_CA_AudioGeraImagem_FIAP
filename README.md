<h1 align="left">Tech Challenge 03 -  Gerador de Imagem por Voz com Microsserviços e Mensageria - FIAP 2024</h1>
➤ O projeto 'Gerador de Imagem por Voz com Microsserviços e Mensageria' consiste em duas soluções que se comunicam por meio de mensageria. 
Os usuários têm a capacidade de fornecer um áudio, o qual é então transcrito para texto por meio de tecnologias 
de Inteligência Artificial (IA). Posteriormente, utilizando esse texto como base, é gerada a imagem correspondente e 
disponibilizada para consumo.

<h3 align="left">Integrantes</h3>
- ➤ <a href="https://github.com/talles2512">Hebert Talles de Jesus Silva</a> - RM352000 </br> 
- ➤ <a href="https://github.com/LeonardoCavi">Leonardo Cavichiolli de Oliveira</a> - RM351999 </br>

<h3 align="left">Projetos</h3>
- ➤ AudioGeraImagemAPI</br>
- ➤ AudioGeraImagemWorker</br>

<h4 align="left">Projeto - AudioGeraImagemAPI</h4>
➤ A API foi criada utilizando o framework .NET 7 Core, com o desenvolvimento realizado na 
IDE Visual Studio 2022. Para gerenciar o banco de dados, foi adotado o Entity Framework Core, 
utilizando o SQL Server como banco de dados. Além disso, a API incorpora o uso de pacotes MassTransit.RabbitMQ para 
facilitar a publicação de mensagens em filas, permitindo que o segundo projeto consuma os dados de forma eficiente.

<h4 align="left">AudioGeraImagemWorker</h4>
➤ O Projeto Worker foi desenvolvido utilizando o framework .NET Core 7 e a IDE Visual Studio 2022. 
Ele emprega o Entity Framework para gerenciamento do SQL Server e segue a arquitetura DDD (Domain-Driven Design). 
Sua função principal é atuar como consumidor, processando mensagens publicadas e realizando integrações com APIs externas. 
Neste caso específico, integra-se com o OpenAI para transcrição de áudios (whisper) e geração de imagens (dalle-3). 
Além disso, o Worker armazena tanto os áudios quanto as imagens em uma conta Azure Storage Account (Blob). Finalizando, 
o Worker possui uma funcionalidade de retentativa. Em caso de falhas durante qualquer processo, o consumidor cria 
e encaminha a mensagem para uma fila de 'retentativa'. Após um intervalo de 20 segundos, agendado pelo próprio Worker, 
a mensagem é novamente processada. No entanto, se ocorrerem mais de 3 falhas consecutivas no mesmo estado, 
a tarefa é marcada como 'Falha' e finalizada.

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
➤ Executar o seguinte script na base de dados.: <a href="https://github.com/talles2512/TC3_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Tabelas/script%20tabelas.sql">Script das Tabelas</a></br>

<h4 align="left">Iniciando o projeto</h4>
➤ Realizado todas as configurações, ambos projetos devem ser iniciados. Após esse processo, é possível realizar testes com áudios previamente gravados.: <a href="https://github.com/talles2512/TC3_AudioGeraImagem_FIAP/tree/developer/Documenta%C3%A7%C3%B5es/%C3%81udios%20de%20exemplo"> Áudios de Exemplo</a>.</br>
➤ Swagger para testar a API.: <code>https://localhost:[porta]/swagger/index.html</code></br>

<img width="1200" src="https://github.com/talles2512/TC3_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Util/SwaggerAPI.png"></img></br>
<img width="1200" src="https://github.com/talles2512/TC3_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Util/GerandoImagemSwaggerAPI.png"></img></br>
<img width="1200" src="https://github.com/talles2512/TC3_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Util/ObtendoImagemSwaggerAPI.png"></img></br>

<h4 align="left">Diagrama do banco de dados</h4>
<img width="1200" src="https://github.com/talles2512/TC3_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Tabelas/diagramaServices.png"></img>

<h4 align="left">Diagrama de funcionamento dos microsserviços</h4>
<img width="1200" src="https://github.com/talles2512/TC3_AudioGeraImagem_FIAP/blob/consumidor/audio-gera-imagem-worker/Documenta%C3%A7%C3%B5es/Diagramas/TC3.drawio.png"></img>

<h4 align="left">Documentações</h4>
<a href="https://github.com/talles2512/TC3_AudioGeraImagem_FIAP/blob/developer/Documenta%C3%A7%C3%B5es/Tabelas/Descritivo%20das%20Tabelas.txt">Descritivo das Tabelas do Projeto</a>.</br>

