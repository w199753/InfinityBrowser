using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Memory
{
    public class HeapBlock
    {
        public int BlockSize => m_BlockSize;
        public byte[] ElementState => m_ElementState;

        private int m_BlockSize;
        private byte[] m_ElementState;

        public HeapBlock(in int blockSize)
        {
            m_BlockSize = blockSize;
            m_ElementState = new byte[blockSize];
        }

        public bool PullFreeSpaceIndex(in int count, out int startIndex)
        {
            int freeSpaceCount = 0;
            int startSearchIndex = 0;

            for (int i = 0; i < m_ElementState.Length; ++i)
            {
                if (m_ElementState[i] == 0)
                {
                    ++freeSpaceCount;
                    if (freeSpaceCount >= count)
                    {
                        startSearchIndex = i;
                        break;
                    }
                }
                else
                {
                    freeSpaceCount = 0;
                }
            }

            bool bAvalibleSpace = freeSpaceCount >= count;
            startIndex = bAvalibleSpace ? startSearchIndex - (count - 1) : -1;

            for (int j = 0; j < count; ++j)
            {
                m_ElementState[startIndex + j] = 1;
            }

            return bAvalibleSpace;
        }

        public void PushFreeSpaceIndex(in int startIndex, in int count)
        {
            for (int i = 0; i < count; ++i)
            {
                m_ElementState[startIndex + i] = 0;
            }
        }
    }
}
