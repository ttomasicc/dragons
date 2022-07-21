# Dragons :dragon:

## About

Dragons is a small text-based role-playing game where different users can register and create their own characters like a Magician or a Ranger, add some skills and a weapon, and also let the characters fight against each other to see who is the best of them all.

### Detailed Overview of the Game

In order to manage Characters, the User must login. Prior to log in, the user must first register. After successful login, the user obtains a JSON Web Token which is used to access the Character part of the game. This includes:
+ Displaying all User's Characters
+ Displaying single User's Character
+ Adding Character
+ Updating User's Character information
+ Deleting User's Character
+ Adding Skills to specific User Character
+ Adding a Weapon to a specific User's Character

Each Character is tied to the specific User, and only that User can see / manage it. Characters must be one of the following RPG types:
+ Fighter
+ Rogue
+ Magician
+ Ranger
+ Cleric

Characters must have one and only one weapon (User custom). Skills are predefined, and Character can but doesn't need to have a skill. Available skills are the following:
+ Fireball (30 damage)
+ Frenzy (22 damage)
+ Blizzard (24 damage)
+ Charm (28 damage)
+ Play an instrument (32 damage)

The Fighting part of the game is public so everyone can start / watch a battle. We can choose to either manually battle another Character - using either a Weapon or a Skill - or start an automatic fight between multiple Characters. Battle Highscore of all the battles is also publicly available.

## Fight simulation

Let us have two users - Luna & Soundwalk.

Luna has only one dragon `Darksmoke`.

```json
{
  "data": [
    {
      "id": 1,
      "name": "Darksmoke",
      "hitPoints": 100,
      "strength": 12,
      "defense": 10,
      "intelligence": 11,
      "class": "Magician",
      "weapon": {
        "name": "Dagger",
        "damage": 15
      },
      "skills": [
        {
          "name": "Blizzard",
          "damage": 24
        },
        {
          "name": "Charm",
          "damage": 28
        }
      ],
      "fights": 0,
      "victories": 0,
      "defeats": 0
    }
  ],
  "success": true,
  "message": ""
}
```

Soundwalk has two dragons - `Aiden` and `Jormungand`.

```json
{
  "data": [
    {
      "id": 2,
      "name": "Aiden",
      "hitPoints": 100,
      "strength": 15,
      "defense": 8,
      "intelligence": 15,
      "class": "Ranger",
      "weapon": {
        "name": "Bolts",
        "damage": 17
      },
      "skills": [
        {
          "name": "Fireball",
          "damage": 30
        }
      ],
      "fights": 0,
      "victories": 0,
      "defeats": 0
    },
    {
      "id": 3,
      "name": "Jormungand",
      "hitPoints": 100,
      "strength": 10,
      "defense": 13,
      "intelligence": 16,
      "class": "Cleric",
      "weapon": {
        "name": "Piano strings",
        "damage": 20
      },
      "skills": [
        {
          "name": "Frenzy",
          "damage": 22
        },
        {
          "name": "Play an instrument",
          "damage": 32
        }
      ],
      "fights": 0,
      "victories": 0,
      "defeats": 0
    }
  ],
  "success": true,
  "message": ""
}
```

Luna's `Darksmoke` starts a fight with Soundwalk's `Aiden`. We can look at the logs.

```json
{
  "data": {
    "log": [
      "Darksmoke attacks Aiden using Dagger with 20 damage",
      "Aiden attacks Darksmoke using Bolts with 21 damage",
      "Darksmoke attacks Aiden using Blizzard with 25 damage",
      "Aiden attacks Darksmoke using Bolts with 21 damage",
      "Darksmoke attacks Aiden using Dagger with 11 damage",
      "Aiden attacks Darksmoke using Bolts with 18 damage",
      "Darksmoke attacks Aiden using Charm with 32 damage",
      "Aiden attacks Darksmoke using Bolts with 16 damage",
      "Darksmoke attacks Aiden using Dagger with 19 damage",
      "Aiden has been defeated!",
      "Darksmoke won with 24HP left!"
    ]
  },
  "success": true,
  "message": ""
}
```

After 3 rounds of fight, we can look at the highscores of all the dragons that have participated in at least one fight.

```json
{
  "data": [
    {
      "id": 1,
      "name": "Darksmoke",
      "fights": 3,
      "victories": 3,
      "defeats": 0
    },
    {
      "id": 2,
      "name": "Aiden",
      "fights": 3,
      "victories": 0,
      "defeats": 3
    }
  ],
  "success": true,
  "message": ""
}
```

## Technologies and Tools Used

+ [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
  + C# programming language
  + Dependency Injection (DI)
  + Language-Integrated Query (LINQ)
+ [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet)
  + Model-View-Controller (MVC) pattern
  + asynchronous (async/await) HTTP GET/POST/PUT/DELETE
  + Data-Transfer-Objects (DTOs) and AutoMapper
  + Authentication
    + Registration and login
    + Token authentication with JSON Web Token (JWT)
    + Authorization - Claims and Roles
+ [Entity Framework](https://docs.microsoft.com/en-us/ef/)
  + Object-Relatinal-Mapping (ORM) using Model objects
  + Asynchronous CRUD persistance
  + Code-First migration
    + One-to-one relationships
    + One-to-many relationships
    + Many-to-many relationships
  + Data seeding
+ [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
+ [Swagger](https://swagger.io/)

## Running the game

#### 1. Install [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
#### 2. Install [Microsoft SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
#### 3. Initialize database

```shell
$ dotnet ef database update
```

#### 4. Start the game

```shell
$ dotnet run
```
