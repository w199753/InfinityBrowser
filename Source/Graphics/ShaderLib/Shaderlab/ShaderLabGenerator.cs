using System;
using Infinity.Container;
using Infinity.Mathmatics;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;

namespace Infinity.Shaderlib
{
    public struct ShaderLabVisitorResult
    {
        public string StringParam;
        public int IntParam;
        public float FloatParam;
        public float2 RangeParam;
        public float4 VectorParam;
        public Shaderlab ShaderlabParam;
        public ShaderlabKernel KernelParam;
        public ShaderlabCategory CategoryParam;
        public List<ShaderlabProperty> PropertyParam;
        public Dictionary<string, string> HashmapParam;
    };

    public class ShaderLabGenerator : ShaderLabBaseVisitor<ShaderLabVisitorResult>
    {
        public override ShaderLabVisitorResult VisitChildren(IRuleNode node)
        {
            ShaderLabVisitorResult result = DefaultResult;
            int n = node.ChildCount;
            for (int i = 0; i < n; ++i)
            {
                if (!ShouldVisitNextChild(node, result))
                    break;
                IParseTree c = node.GetChild(i);
                ShaderLabVisitorResult childResult = c.Accept(this);
                result = childResult;
            }
            return result;
        }

        public override ShaderLabVisitorResult VisitShader([NotNull] ShaderLabParser.ShaderContext context)
        {
            Shaderlab shaderLab = new Shaderlab();
            shaderLab.Name = VisitShader_name(context.shader_name()).StringParam;

            if (context.properties() != null)
            {
                shaderLab.Properties = VisitProperties(context.properties()).PropertyParam;
            }
            if (context.category() != null)
            {
                shaderLab.Category = VisitCategory(context.category()).CategoryParam;
            }

            ShaderLabVisitorResult result = new ShaderLabVisitorResult();
            result.ShaderlabParam = shaderLab;
            return result;
        }

        public override ShaderLabVisitorResult VisitProperties([NotNull] ShaderLabParser.PropertiesContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty([NotNull] ShaderLabParser.PropertyContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty_int([NotNull] ShaderLabParser.Property_intContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty_float([NotNull] ShaderLabParser.Property_floatContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty_range([NotNull] ShaderLabParser.Property_rangeContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty_color([NotNull] ShaderLabParser.Property_colorContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty_vector([NotNull] ShaderLabParser.Property_vectorContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty_2d([NotNull] ShaderLabParser.Property_2dContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty_cube([NotNull] ShaderLabParser.Property_cubeContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty_3d([NotNull] ShaderLabParser.Property_3dContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitCategory([NotNull] ShaderLabParser.CategoryContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitKernel([NotNull] ShaderLabParser.KernelContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitHlsl_block([NotNull] ShaderLabParser.Hlsl_blockContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitTags([NotNull] ShaderLabParser.TagsContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitTag([NotNull] ShaderLabParser.TagContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitCommon_state([NotNull] ShaderLabParser.Common_stateContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitCull([NotNull] ShaderLabParser.CullContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitZtest([NotNull] ShaderLabParser.ZtestContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitZwrite([NotNull] ShaderLabParser.ZwriteContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitColor_mask([NotNull] ShaderLabParser.Color_maskContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitMeta([NotNull] ShaderLabParser.MetaContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitRange([NotNull] ShaderLabParser.RangeContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitTag_key([NotNull] ShaderLabParser.Tag_keyContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitTag_val([NotNull] ShaderLabParser.Tag_valContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitShader_name([NotNull] ShaderLabParser.Shader_nameContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitDisplay_name([NotNull] ShaderLabParser.Display_nameContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitTexture_name([NotNull] ShaderLabParser.Texture_nameContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitProperty_identifier([NotNull] ShaderLabParser.Property_identifierContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitVal_int([NotNull] ShaderLabParser.Val_intContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitVal_float([NotNull] ShaderLabParser.Val_floatContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitVal_min([NotNull] ShaderLabParser.Val_minContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitVal_max([NotNull] ShaderLabParser.Val_maxContext context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitVal_vec4([NotNull] ShaderLabParser.Val_vec4Context context)
        {
            throw new NotImplementedException();
        }

        public override ShaderLabVisitorResult VisitChannel([NotNull] ShaderLabParser.ChannelContext context) 
        {
            throw new NotImplementedException();
        }
    }
}
