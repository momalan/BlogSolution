# BlogSolution
Simple .NET Web API project for job application.

This is a simple backend solution for blogging platform. 
General functionality of this solution:
- CRUD Blog posts
- Filter blog posts by tag
- List of all tags in system

System requirements: 
- import database `BlogSolutionDB.bacpac` to Microsoft SQL Server Management Studio.
- update connection strings inside `web.config` and `app.config` files - `data source="your sql server name"`.
- load the solution using `BlogSolution.sln` into Visual Studio and run.

Available endpoints:
- `GET /api/posts` => returns all blog posts, recent first.
- `GET /api/posts?tag=C#` => filter by tag. 
- `GET /api/posts/{slug}` => returns specific blog post
- `POST /api/posts` => create blog post 

{"title" : "sample title", "description" : "this is a description", "body" : "this is body", "tagList": ["Programmer", "IT", "C#"]}
- `PUT /api/posts/{slug}` => update specific blog post

{"title" : "sample title", "description" : "this is a description", "body" : "this is body" , "tagList": ["Programmer", "IT", "C#"]}
- `DELETE /api/posts/{slug}` => delete specific blog post
- `GET /api/tags` => returns a list of all tags in database.

All requests are in json type format.
