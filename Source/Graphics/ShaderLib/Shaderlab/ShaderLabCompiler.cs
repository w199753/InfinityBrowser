using System;
using System.Linq;
using System.Text;
using Infinity.Shaderlib;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Infinity.Shaderlib
{
    public class ShaderLabCompiler
    {
        private ShaderLabGenerator m_ShaderLabGenerator;

        public ShaderLabCompiler()
        {
            m_ShaderLabGenerator = new ShaderLabGenerator();
        }

        public void Compile(in Shaderlab shaderLab, string filePath)
        {

        }
    }
}
