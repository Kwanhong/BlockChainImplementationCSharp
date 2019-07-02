using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Dotnetblockchainimplementation
{
    class MainApp
    {
        static void Main(string[] args)
        {
            Random rnd = new Random(DateTime.UtcNow.Millisecond);
            IBlock genesis = new Block(new byte[]{0x00, 0x00, 0x00 ,0x00,0x00});
            byte[] difficulty = new byte[]{0x00,0x00};

            BlockChain chain = new BlockChain(difficulty, genesis);
            for (int i = 0; i < 200; i++) {
                var data = Enumerable.Range(0, 2256).Select(p=>(byte)rnd.Next());
                chain.Add(new Block(data.ToArray()));

                Console.WriteLine($"Block Number {i + 1} :");
                Console.WriteLine(chain.LastOrDefault()?.ToString());
                Console.WriteLine($"Chain Is Valid : {chain.IsValid()}\n");
            }

            Console.ReadLine();
        }
    }
}
