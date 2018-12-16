# PostgreJsonExtensions.NetCore

This project implements a "rustic" library for parsing json/json in postgreSQL and not in runtime for ef core

## How to implement it in your project

Donwload the nuget from https://www.nuget.org/packages/PostgreJsonExtensions/ and then in your code use it as follow example

    _context.Test.JsonWhere<Test, Jason>("json", x => x.Num > 100 && x.Num < 500 && x.Fecha.Date >= DateTime.MinValue && x.Logico)
                 .JsonWhere<Test, Jason>("json2", x => x.Num > 100 && x.Num < 500 && x.Fecha.Date >= DateTime.MinValue && x.Logico)
                 .Where(x => x.Id != Guid.Empty);
    
### The signature of the method goes like this
  
    IQueryable<TEntity> JsonWhere<TEntity, TJsonObj>(this IQueryable<TEntity> source, string jsonColumnName, Expression<Func<TJsonObj, bool>> predicate)
    
Where 
- TEntity is the entity of your postgresql
- TJsonObj is the object you serialize and save in the table
- jsonColumnName is the column name that contains the Json/JsonB

Please have in mind that this library do not cover all the functions that postgreSQL has in his engine, if you want to add another funtions or improve the library don't hesitate and make a pull request or raise a issue with an example
