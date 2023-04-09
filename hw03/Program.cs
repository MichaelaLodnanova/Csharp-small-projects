using PV178.Homeworks.HW03;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

Console.WriteLine("Welcome to HW03, you can use this file as a playground for manually testing your solution.");
Queries queries = new Queries();
foreach (var pica in queries.TigerSharkAttackZipQuery())
{
    Console.WriteLine(pica);
};