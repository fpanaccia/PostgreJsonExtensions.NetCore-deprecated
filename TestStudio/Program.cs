using Microsoft.EntityFrameworkCore;
using PostgreJsonExtensions;
using System;
using System.Linq;
using TestStudio.DTO;
using TestStudio.Entities;

namespace TestStudio
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var conStr = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=1234";
                var ctx = new TestContext(conStr);
                ctx.Database.Migrate();

                var list = ctx.Test.JsonWhere<Test, Jason>("json", x => x.Num > 100 && x.Num < 500 && x.Fecha.Date >= DateTime.MinValue && x.Logico)
                    .JsonWhere<Test, Jason>("json2", x => x.Num > 100 && x.Num < 500 && x.Fecha.Date >= DateTime.MinValue && x.Logico)
                    .Where(x => x.Id != Guid.Empty);

                foreach (var item in list)
                {
                    Console.WriteLine($"{item.Id} - {item.Json} - {item.Json2}");
                }

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
