using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Infinity.Mathmatics;
using System.Collections.Generic;

namespace Infinity.Shaderlib
{
    public struct ShaderlabVisitorResult
    {
        public string Name;
        public int IntParam;
        public float FloatParam;
        public float2 RangeParam;
        public float4 VectorParam;
        public Shaderlab Shaderlab;
        public ShaderlabPass Pass;
        public ShaderlabCategory Category;
        public List<ShaderlabProperties> Properties;
        public Dictionary<string, string> Tags;
    };

    public class ShaderlabGenerator : ShaderLabBaseVisitor<ShaderlabVisitorResult>
    {
        public override ShaderlabVisitorResult VisitChildren(IRuleNode node)
        {
            ShaderlabVisitorResult result = DefaultResult;
            int n = node.ChildCount;
            for (int i = 0; i < n; ++i)
            {
                if (!ShouldVisitNextChild(node, result))
                    break;
                IParseTree c = node.GetChild(i);
                ShaderlabVisitorResult childResult = c.Accept(this);
                result = childResult;
            }
            return result;
        }

        public override ShaderlabVisitorResult VisitShader([NotNull] ShaderLabParser.ShaderContext context)
        {
            Shaderlab shaderLab = new Shaderlab();
            shaderLab.Name = VisitShader_name(context.shader_name()).Name;

            if (context.properties() != null)
            {
                shaderLab.Properties = VisitProperties(context.properties()).Properties;
            }
            if (context.category() != null)
            {
                shaderLab.Category = VisitCategory(context.category()).Category;
            }

            ShaderlabVisitorResult result = new ShaderlabVisitorResult();
            result.Shaderlab = shaderLab;
            return result;
        }

        public override ShaderlabVisitorResult VisitProperties([NotNull] ShaderLabParser.PropertiesContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty([NotNull] ShaderLabParser.PropertyContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty_int([NotNull] ShaderLabParser.Property_intContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty_float([NotNull] ShaderLabParser.Property_floatContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty_range([NotNull] ShaderLabParser.Property_rangeContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty_color([NotNull] ShaderLabParser.Property_colorContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty_vector([NotNull] ShaderLabParser.Property_vectorContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty_2d([NotNull] ShaderLabParser.Property_2dContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty_cube([NotNull] ShaderLabParser.Property_cubeContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty_3d([NotNull] ShaderLabParser.Property_3dContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitCategory([NotNull] ShaderLabParser.CategoryContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitKernel([NotNull] ShaderLabParser.KernelContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitHlsl_block([NotNull] ShaderLabParser.Hlsl_blockContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitTags([NotNull] ShaderLabParser.TagsContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitTag([NotNull] ShaderLabParser.TagContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitCommon_state([NotNull] ShaderLabParser.Common_stateContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitCull([NotNull] ShaderLabParser.CullContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitZtest([NotNull] ShaderLabParser.ZtestContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitZwrite([NotNull] ShaderLabParser.ZwriteContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitColor_mask([NotNull] ShaderLabParser.Color_maskContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitMeta([NotNull] ShaderLabParser.MetaContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitRange([NotNull] ShaderLabParser.RangeContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitTag_key([NotNull] ShaderLabParser.Tag_keyContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitTag_val([NotNull] ShaderLabParser.Tag_valContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitShader_name([NotNull] ShaderLabParser.Shader_nameContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitDisplay_name([NotNull] ShaderLabParser.Display_nameContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitTexture_name([NotNull] ShaderLabParser.Texture_nameContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitProperty_identifier([NotNull] ShaderLabParser.Property_identifierContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitVal_int([NotNull] ShaderLabParser.Val_intContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitVal_float([NotNull] ShaderLabParser.Val_floatContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitVal_min([NotNull] ShaderLabParser.Val_minContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitVal_max([NotNull] ShaderLabParser.Val_maxContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitVal_vec4([NotNull] ShaderLabParser.Val_vec4Context context)
        {
            throw new NotImplementedException();
        }

        public override ShaderlabVisitorResult VisitChannel([NotNull] ShaderLabParser.ChannelContext context) 
        {
            throw new NotImplementedException();
        }
    }
}
