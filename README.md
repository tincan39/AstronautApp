# How to Run

Go to the exercise1 folder.


## 1. Create the database
Run this command

```
cd api
dotnet ef database update
```

## 2. Run the API

The Angular dev server proxies `/api` to `https://localhost:7204`, so start the API with the `https` profile:

```
dotnet dev-certs https --trust
cd api
dotnet run --launch-profile https
```

Or you can run it in an IDE like Rider or Visual Studio in DEBUG mode

## 3. Run the UI

In a second terminal:

```
cd client
npm install
npm start
```

Open http://localhost:4200.

## 4. Run the tests

Backend (NUnit):

```
cd StargateTestBed
dotnet test
```

### Code coverage report

Install report generator tool

```
dotnet tool install --global dotnet-reportgenerator-globaltool
```


```
cd StargateTestBed

dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

Note: make sure to delete any prior test coverage report before running last two commands if you generated them
