## data model changes
- removed initial create migration(it had CareerStartDate as nullable which was later changed) and re-added it
- added unique constraint to person name

## query changes
- removed raw queries and used native ef core syntax to prevent sql injection

## command changes
- removed sql injection queries
- refactored messy logic(eg moved AstronautDetail buisness logic into the class itself)
- Creating a person now returns the full dto 

## controller changes
- Corrected GetAstronautDutiesByName sending the wrong request to mediatr
- removed the redundant try/catch blocks scattered throughout controllers. GlobalExceptionHandler replaces them
- removed controllerbase extensions after removing BaseResponse
- propagated cancellation tokens down to the mediatr level

## dtos
- removed BaseReponse since that information would be handled by the GlobalExceptionHandler class I've made which would format and handle error
- removed PersonId from PersonDTO(which was renamed from PersonAstronaut) since letting the user know the integer primary key is security risk
- convert class dtos to records

## logging
- uses ef core for db logs
- serilog is in place for file logs in case ef core logging fails

## TestBed
- used sqllite in memory database as a stub for the database calls
- leverages nunit for the test library 
- user coverlet and reportgenerator to generate and view code coverage reports
    - code migrations are excluded from the report

## FrontEnd
- There are three pages
    - The Search Page, where you can search for people
    - The Person Page, where you can create, update person; add new astronaut duties; and view past duties
    - A "Page Not Found" Page for all invalid routes