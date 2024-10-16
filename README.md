
# Cashflow Management System

## Overview

Para a solu��o ser resiliente foi adotada uma arquitetura ass�ncrona, onde o servi�o de transa��es e o servi�o de consolida��o s�o independentes e se comunicam atrav�s de um barramento de eventos.
O servi�o de transa��es � respons�vel por processar as entradas de d�bito e cr�dito di�rias, enquanto o servi�o de consolida��o gera relat�rios de saldo consolidado diariamente.
Para garantir uma alta disponibilidade, o sistema foi projetado usando modelo de eventos, onde as mensagens s�o armazenadas em um barramento de eventos (Kafka) e processadas de forma ass�ncrona.
Dessa forma o su�rio n�o precisa esperar a consolida��o ser finalizada para realizar novas transa��es e permite que o sistema seja escal�vel.
Outra vantagem � que o sistema � tolerante a falhas, caso o servi�o de consolida��o fique indispon�vel, as mensagens ser�o processadas assim que o servi�o estiver dispon�vel novamente.
Caso aconte�a uma falha no servi�o de processamento de transa��es, essas s�o armazendas em uma fila de mensagens e para serem analisadas e reprocesadas posteriormente.
A arquitetura por mensagens tamb�m permite que o sistema seja facilmente integrado com outros sistemas, como um sistema de an�lise de dados ou um sistema de notifica��o.
Para seguran�a, o sistema utiliza de um servi�o externo de autentica��o e autoriza��o.
Essa autentifica��o � realizada no servi�o de API Gateway, que � respons�vel por rotear as requisi��es para os servi�os corretos, garantindo que apenas usu�rios autenticados possam acessar os servi�os e que n�o haja necessidade de repetir a l�gica de autentica��o em cada servi�o.


![Diagrama de Components](./public/components-diagram.drawio.png)]

### Features

- **Transaction Service**: Manages daily debit and credit entries.
- **Report Service**: Provides daily consolidated balance reporting.

---

## Diagrama de Requisi��o

Abaixo est� o diagrama de requisi��o do sistema, onde � poss�vel ver a intera��o entre os servi�os e o barramento de eventos.
A requisi��o realizada pelo usu�rio faz um processamento rapido pois n�o requer esperar consolida��es e nem respostas de outros components arquiteurais.
Com isso a resiliencia do sistema � garantido, pois dificulta um gargalo de requisi��es.
Caso aconteca um gargalo de requisi��es, o sistema � facilmente escal�vel, pois � possivel adicionar mais instancias.
Se o problema descer para a camada de processamento de transa��es, as mensagens s�o armazenadas em uma fila de mensagens e processadas posteriormente.

![Architecture Diagram](./public/create-request-uml.drawio.png)


## Componentes (POC)

- **API Asp Core 8**
- **Asp Core 8 Console App**
- **MSSQLServer**
- **Kafka Broker**
- **Kafka Zookeeper**


### Requirements

- **Docker** (for containerized deployment)
  
## Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/DMPatod/act-cashflow.git
   cd act-cashflow
   ```

2. **Publish Docker Container**:
   ```bash
   docker-compose up
   ```


### API Endpoints

- **Transaction Service**:
  - `POST /api/transactions` - Create a new transaction (debit/credit).
  - `GET /api/transactions/{id}` - Get transaction details by ID. [TODO]
  
- **Report Service**:
  - `GET /api/reports/{date}` - Get the consolidated balance for the day. [TODO]

## Future Improvements

The current solution is designed to be scalable and resilient, but there are a few areas for future enhancements:

1. **Monitoring**: Add monitoring and alerting features using tools like **Prometheus** and **Grafana** for better observability and troubleshooting.
2. **Enhanced Security**: Add Auth Container
3. **Improved Resilience**: Integrate Polly for advanced retry policies, circuit breakers, and bulkhead isolation patterns.

---
