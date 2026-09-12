# 🔎 Diagnóstico de Conexão

Aplicação desktop desenvolvida em **C# / .NET** para diagnóstico de conectividade de rede, testes de portas e análise básica da superfície de rede local.

O projeto foi criado com o objetivo de unir **programação, redes de computadores e fundamentos de segurança**, servindo também como projeto prático de estudo e portfólio.

> ⚠️ **Projeto em desenvolvimento:** esta versão ainda não é a versão final. A interface e algumas funcionalidades continuam sendo ajustadas, refatoradas e aprimoradas.

---

## 📌 Sobre o projeto

O **Diagnóstico de Conexão** realiza uma série de verificações para ajudar a identificar problemas de conectividade e fornecer uma visão geral da configuração de rede da máquina.

A aplicação começou como uma aplicação de console e posteriormente ganhou uma interface gráfica utilizando **WPF**, permitindo organizar os diagnósticos em diferentes telas.

O projeto também está sendo utilizado para aprofundar conhecimentos em:

* C#
* .NET
* WPF
* Redes de computadores
* TCP/IP
* DNS
* ICMP
* Traceroute
* Portas TCP
* Diagnóstico de conectividade
* Fundamentos de segurança de redes
* Arquitetura e organização de código

---

## 🚀 Funcionalidades

### 🌐 Diagnóstico de conexão

A aplicação identifica informações da interface de rede e realiza testes de conectividade.

Entre as informações verificadas:

* Interface de rede utilizada
* Tipo da conexão
* Endereço IPv4
* Máscara de sub-rede
* CIDR
* Endereço da rede
* Gateway
* Servidores DNS
* Disponibilidade da internet
* Latência
* Perda de pacotes

---

### 📡 Teste de conectividade

São realizados testes utilizando **ICMP/Ping** para verificar:

* Comunicação com o gateway
* Resolução de DNS
* Comunicação com a internet
* Latência média
* Perda de pacotes

A aplicação também apresenta uma análise resumida dos resultados encontrados.

---

### 🛰️ Traceroute

Permite analisar o caminho percorrido pelos pacotes até um determinado destino.

Exemplo:

```text
TRACEROUTE

SALTO     ENDEREÇO IP          LATÊNCIA
1         192.168.15.1        1 ms
2         *                   *
3         10.x.x.x            12 ms
...
```

O recurso utiliza diferentes valores de **TTL (Time To Live)** para identificar os saltos existentes entre a máquina local e o destino.

---

### 🔌 Teste de portas

Permite verificar a disponibilidade de uma porta TCP em um determinado host.

Exemplo:

```text
Host: google.com
Porta: 443

Resultado: Porta aberta
```

Esse recurso pode ser utilizado para entender melhor a comunicação através de portas TCP e como diferentes serviços podem ser acessados em uma rede.

---

### 🛡️ Análise de segurança

A aplicação também realiza uma análise básica das portas que estão em estado de **LISTENING** na máquina local.

Os resultados são classificados em níveis:

| Nível          | Descrição                                           |
| -------------- | --------------------------------------------------- |
| 🟢 Safe        | Serviço considerado seguro dentro das regras atuais |
| 🔵 Information | Informação relevante sobre o serviço                |
| 🟡 Attention   | Situação que merece atenção                         |
| 🔴 Critical    | Situação potencialmente crítica                     |

A análise considera informações como:

* Porta utilizada
* Endereço de escuta
* Processo associado
* Nome do processo
* Tipo de exposição
* Regras básicas de classificação

> A análise não representa uma ferramenta completa de pentest ou detecção de ataques. Seu objetivo atual é **educacional e diagnóstico**, ajudando a identificar possíveis pontos de atenção na configuração local.

---

## 🖥️ Interface

A interface gráfica foi desenvolvida utilizando **WPF**, com uma proposta visual baseada em:

* Tema escuro
* Destaques em rosa
* Cards informativos
* Indicadores visuais
* Botões personalizados
* Tabelas para apresentação dos resultados
* Janelas separadas para cada funcionalidade

A interface ainda está passando por ajustes visuais e refatoração.

---

## 🏗️ Estrutura do projeto

O projeto utiliza uma separação entre modelos, serviços e interface gráfica.

```text
ProjetoDiagnosticoConexao
│
├── DiagnosticoConexao
│   │
│   ├── Models
│   │   ├── NetworkInfo.cs
│   │   ├── PingResult.cs
│   │   ├── DnsResult.cs
│   │   ├── NetworkDiagnosticResult.cs
│   │   ├── TracerouteHop.cs
│   │   ├── PortTestResult.cs
│   │   ├── SecurityFinding.cs
│   │   ├── SecuritySummary.cs
│   │   ├── SecuritySeverity.cs
│   │   └── SecuritySource.cs
│   │
│   ├── Services
│   │   ├── NetworkService.cs
│   │   ├── PingService.cs
│   │   ├── DnsService.cs
│   │   ├── DiagnosticService.cs
│   │   ├── AnalysisService.cs
│   │   ├── TracerouteService.cs
│   │   ├── PortService.cs
│   │   ├── LocalPortService.cs
│   │   ├── SecurityAnalysisService.cs
│   │   └── SecurityPresentationService.cs
│   │
│   └── Program.cs
│
└── DiagnosticoConexao.WPF
    │
    ├── MainWindow.xaml
    ├── SecurityWindow.xaml
    ├── TracerouteWindow.xaml
    └── PortWindow.xaml
```

A separação em **Models / Services / WPF** permite manter a lógica de diagnóstico independente da interface gráfica.

---

## 🛠️ Tecnologias utilizadas

### Linguagem

* **C#**

### Plataforma

* **.NET 10**
* **WPF**

### Conceitos e tecnologias de rede

* TCP/IP
* ICMP
* DNS
* TTL
* TCP Ports
* Traceroute
* Netstat

### Arquitetura

* Programação orientada a objetos
* Separação de responsabilidades
* Services
* Models
* Interface gráfica desacoplada da lógica de diagnóstico

---

## 🎯 Objetivos de aprendizado

Este projeto está sendo desenvolvido principalmente como uma forma prática de estudar e aplicar conceitos de:

**Programação + Redes + Segurança**

A ideia é continuar evoluindo a aplicação conforme novos conhecimentos forem adquiridos, adicionando funcionalidades e melhorando a arquitetura.

---

## 🔮 Próximos passos

Algumas melhorias planejadas para versões futuras:

* [ ] Refatoração dos estilos do WPF
* [ ] Centralização dos estilos visuais
* [ ] Melhorias na responsividade da interface
* [ ] Melhor apresentação dos resultados
* [ ] Novas análises de segurança
* [ ] Mais testes de portas
* [ ] Melhor tratamento de erros
* [ ] Exportação dos resultados
* [ ] Histórico de diagnósticos
* [ ] Novos testes de rede
* [ ] Evolução da análise de segurança

---

## ⚠️ Status do projeto

**Em desenvolvimento 🚧**

A versão disponível neste repositório representa o estado atual do projeto e **não deve ser considerada a versão final**.

Novas funcionalidades, melhorias de código, refatorações e ajustes visuais ainda serão realizados.

---

## 👩‍💻 Autora

Desenvolvido por **Kelly Vencionek** como projeto de estudo e portfólio, explorando principalmente **C#, .NET, redes de computadores e fundamentos de segurança da informação**.
