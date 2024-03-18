// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

IConfiguration config = new ConfigurationBuilder()
  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
  .Build();
