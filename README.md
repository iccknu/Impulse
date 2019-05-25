# Impulse
Program for sending notifications through different communication channels (such as Telegram, Facebook and similar).

## Supported providers
At the moment we support next providers:
* [Telegram](https://telegram.org)
* [Slack](https://slack.com)
* Email

## API set up
For using this API you have to provide only two things:
1. [servicesettings.json](https://github.com/iccknu/Impulse/blob/0.0.1-dev/ImpulseAPI/servicesettings.json) (You can provide only those providers which will be used);
2. Connection string to PostgreSQL (for now it's only one supported storage), by using environment variables: POSTGRESQL_HOST, POSTGRESQL_DATABASE, POSTGRESQL_USER_ID and POSTGRESQL_PASSWORD.

## How to write servicesettings.json
Template of full version of file you can find [here](https://github.com/iccknu/Impulse/blob/0.0.1-dev/ImpulseAPI/servicesettings.json). And now let's see how to get all needed information for each provider.
### Telegram provider
1. Create a [developer account](https://my.telegram.org) in Telegram.
2. Goto [API development tools](https://my.telegram.org/apps) and copy API_ID and API_HASH from your account.
```JSON
{
  "services": {
    "telegram": {
      "apiId": 999999,
      "apiHash": "af664c********************9aef32",
      "phoneNumber": "+38091*****46",
      "messagesPerSecond": 0.1
    }
  }
}
```
---
### Slack provider
1. Create an [account](https://slack.com) in Slack.
2. Generate your [legacy token](https://api.slack.com/custom-integrations/legacy-tokens) and copy it.
```JSON
{
  "services": {
    "slack": {
      "userToken": "xoxp-6213********-************-************-********************9213174ca7da",
      "messagesPerSecond": 1
    }
  }
}
```
---
### Email provider
Just register an account at any email service, and find required information about it's smtp server. For example Gmail's smtp server is `smtp.gmail.com`, and smtp port is `465`. (May be some small additional settings required, such as [Enable less secure apps to access accounts](https://support.google.com/a/answer/6260879?hl=en), depends on email service)
```JSON
{
  "services": {
    "email": {
      "smtpServer": "smtp.gmail.com",
      "smtpPort": 465,
      "userName": "userName",
      "password": "password",
      "email": "userName@gmail.com",
      "messagesPerSecond": 1
    }
  }
}
```
---
Field `messagesPerSecond` is positive float number that used for calculating delay between sending direct messages to users, because every provider has a different restrictions for detecting spam.

## PostgreSQL
Everything that you need to do with database, it's create it and specify information about connection in environment variables: POSTGRESQL_HOST, POSTGRESQL_DATABASE, POSTGRESQL_USER_ID and POSTGRESQL_PASSWORD.

## Last preparation for Telegram provider
If you want to use Telegram provider then one more step required.
> Only a small portion of the API methods are available to unauthorized users. ([full description](https://core.telegram.org/api/auth))

Everything that you need to do, it's run API, then make two requests to `RegisterTelegramService`. First time without sending any data. This will make request to Telegram API to send code on your phone. And then second time, but now with code that you got. After it you will be able to work with Telegram provider.

# Docker
Also is possible to run this API on Docker, for it you need:
1. (Optional) Run PostgreSQL:
```powershell
docker run -e POSTGRES_USER=sa -e POSTGRES_PASSWORD=Testing123 -e POSTGRES_DB=Impulse -p 5432:5432 --name postgresql -d postgres
```
2. Build docker image (from solution folder):
```powershell
docker build -t impulse -f .\ImpulseAPI\Dockerfile .
```
3. Run this image (use `--link postgresql` only if you run PostgreSQL in docker too):
```powershell
docker run -it --name impulseapi -p 50000:50000 --link postgresql -e POSTGRESQL_HOST=postgresql -e POSTGRESQL_DATABASE=Impulse -e POSTGRESQL_USER_ID=sa -e POSTGRESQL_PASSWORD=Testing123 -v "$pwd\ImpulseApi\servicesettings.json:/app/servicesettings.json" -d impulse
```

## Docker Compose
Or you can change environment variables in `docker-compose.yml` file and just run next two commands (from solution folder):
```powershell
docker-compose build
docker-compose up -d
```

# Hangfire Dashboard
For enabling Hangfire dashboard you need to go to `\ImpulseAPI\Extensions\MyAuthorizationFilter.cs` file, comment `httpContext.User.Identity.IsAuthenticated;` and uncomment `true;`. **But be careful this will gives access to dashboard page to everyone. So use it for debug only.** After it, dashboard page accessible by `/hangfire`.

# Now, when you are ready
For getting detailed information about API's methods, you can go to `/swagger` page.
