# Retrospective

## What went well
- Working with MediatR and the CQRS design pattern
- Stubbing the tests for DB calls. I thought it would be more of a hassle, but thankfully SQLite has an option for an in-memory database
- Identifying flaws such as the SQL injection queries made in the handler classes
- Setting up the UI. Time was tight, but I was able to make a functional UI, where it used all the
required endpoints

## What could be improved
- Logging to the database could be improved. I set up logging with the StargateContext instead of using a library like Serilog to funnel logs into the DB. I had to abandon the Serilog approach because I discovered SQLite only accepts one writer at a time and that causes issues
- Accounting for concurrency issues involving multiple requests
- Some of the operations were slower than I expected. Latency could definitely be improved
- the person page component has duplicate code that could be cleaned up

## What I would do with more time
- Adding a cache (something like IMemoryCache or Redis if it needs better scaling)
- Adding Testcontainers to run the tests against a more production-grade
DB provider like SQL Server
- Using pagination and the Intersection Observer for an infinite scroll on the UI's search page. Right now it loads in all the data at once, which isn't very scalable
- I could've added addition filters to the search beyond just the name
    - Elasticsearch could also be a good option if you wanted to implement, given that it has built it fuzzy search and the ability to rank searches
