using System;
using System.Linq;
using System.Text;
using Infinity.Shaderlib;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Infinity.Shaderlib
{
    public class ShaderlabCompiler
    {
        private ShaderlabGenerator m_ShaderLabGenerator;

        public ShaderlabCompiler()
        {
            m_ShaderLabGenerator = new ShaderlabGenerator();
        }

        public void Compile(in Shaderlab shaderLab, string filePath)
        {

        }
    }
}
