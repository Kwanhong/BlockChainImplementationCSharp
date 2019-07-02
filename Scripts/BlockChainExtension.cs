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
}