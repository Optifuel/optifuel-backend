# Api
Install [PgAdmin 4](https://www.pgadmin.org/download/) and [postgres SQL](https://www.postgresql.org/download/)

Install [.net Core and SDK 7.0 ](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

In the project folder, execute the following commands

`dotnet tool install --global dotnet-ef --version 7.0.12`

`dotnet restore`

Now launch Postgres and PgAdmin 4

`dotnet ef migrations add InitialMigration --context ApiDbContext `

`dotnet ef database update`

# DEBUG

Run the project on Visual Studio (or execute `dotnet run`) and go to the following link: http://localhost:5294/index.html
Open this section
![image](https://github.com/GabrieleSantangelo/ApiCOS/assets/49369397/8ca7f6c9-a184-4bb5-83ca-1c78dce6c4f2)
Click on "Try it out" and paste the following text into the text section:

`{
  "businessName": "Reply S.p.A",
  "vatNumber": "08013390011",
  "name": "Reply",
  "address": {
    "street": "Corso Francia",
    "streetNumber": "110",
    "city": "Torino",
    "zipCode": "10143"
  }
}`


