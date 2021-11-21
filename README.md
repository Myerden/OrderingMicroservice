# OrderingMicroservice

This is an example of a very simplified ordering system made in a microservice architecture using .NET 5

## Overview

This project consists of following partitions
- AppGateway
- CustomerService.Api
- CustomerService.Application
- CustomerService.Test
- OrderService.Api
- OrderService.Application
- OrderService.Test

### AppGateway
AppGateway is a microservice that responsible for communication with other services.

This service includes:
* Ocelot is used for Gateway implementation
* Containerization

### CustomerService.Api
Customer microservice that responsible for basic CRUD implementations over Customer Entity.

This service includes:
* .NET 5 Web Api implementation
* REST API Principles, CRUD operations
* PostgreSQL as Database Provider
* Repository Pattern implementation
* Containerization

### OrderService.Api
Order microservice that responsible for basic CRUD implementations over Order Entity.

This service includes:
* .NET 5 Web Api implementation
* REST API Principles, CRUD operations
* PostgreSQL as Database Provider
* Repository Pattern implementation
* Containerization

## Run The Project
You will need the following tools:

* [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)
* [.Net Core 5 or later](https://dotnet.microsoft.com/download/dotnet-core/5)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Installing
Follow these steps to get your development environment set up: (Before Run Start the Docker Desktop)
1. Clone the repository
2. (Optional) If you want, you can limit the Wsl2's memory and cpu usage. ([see also](https://github.com/microsoft/WSL/issues/4166))
  - Go to User's Document Folder ( Press "WinKey + R" and run "%UserProfile%" )
  - Create a file named *.wslconfig*
  - Write its content as follows (Windows restart may be required)
```
[wsl2]
memory=8GB
processors=2
swap=0
localhostForwarding=true
```
3. At the root directory which include **docker-compose.yml** and **docker-compose.override.yml** files, run below command:
```
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
```
4. Wait for docker compose all microservices. 

5. You can **launch microservices** as below urls:

* **Gateway -> http://localhost:5000**
* **Customer Microservice -> http://localhost:5002**
* **Order Microservice -> http://localhost:5004**
* **pgAdmin PostgreSQL -> http://localhost:5050**   -- admin@admin.com / admin

If you want to use the API with Postman, click below link.

[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/18286593-161a149f-1640-4034-9fed-ee282e00253f?action=collection%2Ffork&collection-url=entityId%3D18286593-161a149f-1640-4034-9fed-ee282e00253f%26entityType%3Dcollection)

## Testing
To run unit tests, you have to run microservices with test environment. We will also use Docker for unit tests.
1. At the root directory which include **docker-compose-tests.yml** and **docker-compose-tests.override.yml** files, run below command:
```
docker-compose  -f docker-compose-tests.yml -f docker-compose-tests.override.yml up
```
2. Wait for docker compose all microservices.
3. From the Test menu item, run the all tests.

![image](https://user-images.githubusercontent.com/15304742/141701962-ea730d56-119a-42e7-9883-bead95ec21c4.png)

4. Open the Test Explorer window, and notice the results of the tests.

![image](https://user-images.githubusercontent.com/15304742/142767566-d597d8eb-c508-4d1a-b217-e4b9ea5c8ab2.png)
