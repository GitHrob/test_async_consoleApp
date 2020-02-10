using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp_paralelni_zpracovani
{
    class Program
    {
        //delegát vyvolaný událostí
        public delegate string MyDelHandler(string str);
        //událost, která vyvolá delegáta MyDel
        event MyDelHandler MyEvent;

        public Program()
        {
            //registrace události
            this.MyEvent += new MyDelHandler(this.WelcomeUser);
        }


        //tělo události dané třídou, která událost akceptuje
        public string WelcomeUser(string username)
        {
            return "Aktivita: " + username;
        }

        static async Task Main()
        {
            Program obj1 = new Program();
            //vyvolání události
            string result = obj1.MyEvent("vyvolána nejjednodušší událost ");
            string resultInvoke = obj1.MyEvent.Invoke("znovu vyvolána prostřednictvím Invoke(string)");

            Console.WriteLine(result);
            Console.WriteLine(resultInvoke);

            //zrušení události
            obj1.MyEvent -= obj1.MyEvent;
            string resultEvent = (obj1.MyEvent!=null) ? obj1.MyEvent.Invoke("událost znovu vyvolána"):"událost odstraněna";
            Console.WriteLine(resultEvent);
            var sw = Stopwatch.StartNew();

            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(2000);
            var token = tokenSource.Token;
            token.ThrowIfCancellationRequested();

            Task<int> uloha1async = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(6000, token);
                    Console.WriteLine("uloha 1 splnena - start pred {0} ms", sw.ElapsedMilliseconds);
                    return 1;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("uloha 1 chyba zrušena - start pred {0} ms", sw.ElapsedMilliseconds);
                    return -1;
                }
            }, token);

            Task<int> uloha2async = Task.Run(() =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("uloha 2 splnena - start pred {0} ms", sw.ElapsedMilliseconds);
                return (2);
            });

            await Task.WhenAll(uloha1async, uloha2async);
            //Task.WaitAll(uloha1async, uloha2async);

            int uloha1Result = await uloha1async;
            Console.WriteLine("uloha 1 vysledek{1} - start pred {0} ms", sw.ElapsedMilliseconds, uloha1Result);
            int uloha2Result = await uloha2async;
            Console.WriteLine("uloha 2 vysledek{1} - start pred {0} ms", sw.ElapsedMilliseconds, uloha2Result);
            Console.WriteLine("Hello World! pred {0} ms", sw.ElapsedMilliseconds);

            Console.ReadLine();
        }
    }
}
