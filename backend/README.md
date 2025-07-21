# Exams.PwC - Backend

## 1. Install Prerequisites

Make sure you have:

- **Node.js** installed

## 1. Requirements
- Docker (for database and email system).
- Azurite (for eMail system).

## 2. Run the solution

Make sure you have Docker and Azurite started.

### 2.1 Start Requirements

#### Start Azurite

> To start Azurite
> 
> Run __Azurite.exe__ on a console on the next path: _\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\Extensions\Microsoft\Azure Storage Emulator_

#### Start Docker compose

> To start docker compose
>
> Run in a console on the path _.\backend_
> ```bash
> docker compose up
> ```

### 2.2 Run the solution locally

#### Navigate to the App

```bash
cd .\backend\src\PwC.CarRental.Api
```

#### Run the App locally

```bash
dotnet run --launch-profile https
```

#### Navigate to the eMail Function

```bash
cd .\backend\src\PwC.CarRental.EmailFunction
```

Inside your email function folder, install the packages:

```bash
npm install -g azure-functions-core-tools@4 --unsafe-perm true
```

#### Run the eMail function locally

Before running the app, make sure this configuration is set in the Azure Function (__PwC.CarRental.EmailFunction__) in the _local.settings.json_.

> ```json
> {
>   "IsEncrypted": false,
>   "Values": {
>     "AzureWebJobsStorage": "UseDevelopmentStorage=true",
>     "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
>     "MailConfiguration:Port": "3025",
>     "MailConfiguration:Host": "localhost",
>     "MailConfiguration:User": "test",
>     "MailConfiguration:Password": "test",
>     "MailConfiguration:From": "noreply@gmail.com",
>     "MailConfiguration:FromName": "No Reply"
>   }
> }
> ```

> 
```bash
func start --port 7187
```

## Info
- The API will be execute on __https://localhost:7256/api__, with swagger on __https://localhost:7256/index.html__.
- The Azure Ffunction will be execute on the port __7187__.
- The email messages can be checked in the __http://localhost:8080/__, with user 'test' and password 'test'.