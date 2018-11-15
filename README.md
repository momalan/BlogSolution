# BlogSolution
Simple .NET web application for job application.

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
- `GET /api/posts` 
- `GET /api/posts?tag=C#`
- `GET /api/posts/{slug}`
- `POST /api/posts`
{"title" : "sample title", "description" : "this is a description", "body" : "this is body", "tagList": ["Programmer", "IT", "C#"]}
- `PUT /api/posts/{slug}`
{"title" : "sample title", "description" : "this is a description", "body" : "this is body" , "tagList": ["Programmer", "IT", "C#"]}
- `DELETE /api/posts/{slug}`
- `GET /api/tags`

All requests are in json type format.
