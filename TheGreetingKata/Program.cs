using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace TheGreetingKata
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var container = Container.CreateHostBuilder(args).Services;
            var greet = container.GetRequiredService<Greetings>();
            var multiGreet = container.GetRequiredService<MultiNames>();

            var names = container.GetRequiredService<Names>();
            names.Name = new List<string> { "Marco", "MArio" };
            names.previousNames = 0;
            multiGreet.Greets(names, greet);
            Console.WriteLine(greet.Greet("Marco"));
            Console.WriteLine(names.greet);


        }
    }

    public static class Container
    {
        public static IHost CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
               {
                   services.AddSingleton<Test>();
                   services.AddSingleton<Names>();
                   services.AddSingleton<Greetings>((x) =>
                   {
                       var nameNull = new NameNull();
                       var nameUpperCase = new NameUpperCase();
                       nameNull.SetSuccessor(nameUpperCase);
                       return nameNull;

                   });
                   services.AddSingleton<MultiNames>((x) =>
                   {
                       var checkMultiNames = new CheckMultiNames();
                       var twoLowerCaseMulti = new TwoLowerCaseMultiNames();
                       checkMultiNames.SetSuccessor(twoLowerCaseMulti);
                       return checkMultiNames;

                   });
               }).Build();
    }

    public class Test
    {
        public void Log()
        {
            Console.WriteLine("prova");
        }
    }

    public abstract class Greetings
    {
        protected Greetings? Next;
        public Greetings SetSuccessor(Greetings next)
        {
            //aproved = next.aproved;
            //errors = next.errors;
            Next = next;
            return Next;
        }
        public abstract string Greet(string name);
    }

    public abstract class MultiNames
    {
        protected MultiNames? Next;
        public MultiNames SetSuccessor(MultiNames next)
        {
            //aproved = next.aproved;
            //errors = next.errors;
            Next = next;
            return Next;
        }
        public abstract Names Greets(Names names, Greetings greetings);
    }

    public class CheckMultiNames : MultiNames
    {
        public override Names Greets(Names names, Greetings greetings)
        {
            if (names.Name.Count < 2)
            {
                names.greet += greetings.Greet(names.Name[1]);
                names.Name.RemoveAt(0);
            }
            else
            {
                names = Next?.Greets(names,greetings);
            }
            
            return names;
        }
    }

    public class TwoLowerCaseMultiNames : MultiNames
    {
        public override Names Greets(Names names, Greetings greetings)
        {
            if (names.Name[0].Any(char.IsLower) && names.Name[1].Any(char.IsLower))
            {
                names.greet += greetings.Greet(names.Name[0]) + $", {names.Name[1]}";
                names.Name.RemoveRange(0,2);
            }

            return names;
        }
    }

    public class Names
    {
        public List<string> Name;
        public string greet;
        public int previousNames;

        public void Order()
        {

            foreach(string name in Name)
            {

            }
        }
        /*Names(List<string> name)
        {
            Name = name;
            greet = "";
        }*/
    }



    public class NameNull : Greetings
    {
        public override string Greet(string name)
        {
            if (name == "")
            {
                name = "Hello, my friend";
            }
            else
            {
                name = Next?.Greet(name);
            }
            return name;
        }

    }

    public class NameUpperCase : Greetings
    {
        public override string Greet(string name)
        {
            if (name.All(char.IsUpper))
            {
                name = "HELLO " + name;
            }
            else
            {
                name = "Hello " + name;
            }
            return name;
        }
    }
}
