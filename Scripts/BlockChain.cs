using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Dotnetblockchainimplementation
{
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
}