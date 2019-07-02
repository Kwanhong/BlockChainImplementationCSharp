using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Dotnetblockchainimplementation
{
    public static class BlockChainExtension
    {
        public static byte[] GenerateHash(this IBlock block)
        {
            using (SHA512 sha = new SHA512Managed())
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(block.Data);
                writer.Write(block.Nonce);
                writer.Write(block.TimeStamp.ToBinary());
                writer.Write(block.PreviousHash);
                var streamArray = stream.ToArray();
                return sha.ComputeHash(streamArray);
            }
        }
        public static byte[] MineHash(this IBlock block, byte[] difficulty)
        {
            if (difficulty == null) throw new ArgumentNullException(nameof(difficulty));

            byte[] hash = new byte[0];
            int length = difficulty.Length;
            while (!hash.Take(length).SequenceEqual(difficulty))
            {
                block.Nonce++;
                hash = block.GenerateHash();
            }

            return hash;
        }
        public static bool IsValid(this IBlock block)
        {
            var blockChain = block.GenerateHash();
            return block.Hash.SequenceEqual(blockChain);
        }
        public static bool IsValidPreviousBlock(this IBlock block, IBlock previousBlock)
        {
            if (previousBlock == null) throw new ArgumentNullException(nameof(previousBlock));

            var previous = previousBlock.GenerateHash();
            return previousBlock.IsValid() && block.PreviousHash.SequenceEqual(previous);
        }
        public static bool IsValid(this IEnumerable<IBlock> items)
        {
            var enumerable = items.ToList();
            return enumerable.Zip(enumerable.Skip(1), Tuple.Create).All(block => block.Item2.IsValid() && block.Item2.IsValidPreviousBlock(block.Item1));
        }
    }

    public interface IBlock
    {
        DateTime TimeStamp { get; }
        byte[] Data { get; set; }
        byte[] Hash { get; set; }
        byte[] PreviousHash { get; set; }
        int Nonce { get; set; }
    }

    public class Block : IBlock
    {
        public DateTime TimeStamp { get; }
        public byte[] Data { get; set; }
        public byte[] Hash { get; set; }
        public byte[] PreviousHash { get; set; }
        public int Nonce { get; set; }

        public Block(byte[] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Nonce = 0;
            PreviousHash = new byte[] { 0x00 };
            TimeStamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{BitConverter.ToString(Hash).Replace("-", "")}:\n{System.BitConverter.ToString(PreviousHash).Replace("-", "")}\n{Nonce} {TimeStamp}";
        }

    }

    public class BlockChain : IEnumerable<IBlock>
    {
        private List<IBlock> items = new List<IBlock>();

        public BlockChain(byte[] difficulty, IBlock genesis)
        {
            Difficulty = difficulty;
            genesis.Hash = genesis.MineHash(difficulty);
            Items.Add(genesis);
        }

        public void Add(IBlock item)
        {
            if (Items.LastOrDefault() != null)
            {
                item.PreviousHash = Items.LastOrDefault()?.Hash;
            }
            item.Hash = item.MineHash(Difficulty);
            Items.Add(item);
        }

        public int Count => items.Count;
        public IBlock this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }
        public List<IBlock> Items
        {
            get => items;
            set => items = value;
        }
        public byte[] Difficulty { get; }

        public IEnumerator<IBlock> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }

    class Program
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
                Console.WriteLine(chain.LastOrDefault()?.ToString());

                Console.WriteLine($"Block Number {i + 1} :\nChain Is Valid : {chain.IsValid()}\n");
            }

            Console.ReadLine();
        }
    }
}
