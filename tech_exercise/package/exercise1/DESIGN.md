## data model changes
- removed initial create migration(it had CareerStartDate as nullable which was later changed) and re-added it
- added unique constraint to person name
## query changes
- removed raw queries and used native ef core syntax to prevent sql injection

## controller changes
- Corrected GetAstronautDutiesByName sending the wrong request to mediatr