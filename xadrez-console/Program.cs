using System;
using tabuleiro;

namespace xadrez_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Tabuleiro P;
            P = new Tabuleiro(8, 8);

            Console.WriteLine("Posição: " + P);
        }
    }
}