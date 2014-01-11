float4x4 World;
float4x4 View;
float4x4 Projection;

float3 EyePos;

float time = 0;

float textureLerp;

texture2D normalTex;
sampler2D NormalTextureSampler = sampler_state
{
    Texture = <normalTex>;
    MinFilter = anisotropic;
    MagFilter = anisotropic;
    MipFilter = Linear;
    AddressU = wrap;
    AddressV = wrap;
};

texture2D normalTex2;
sampler2D NormalTextureSampler2 = sampler_state
{
    Texture = <normalTex2>;
    MinFilter = anisotropic;
    MagFilter = anisotropic;
    MipFilter = Linear;
    AddressU = wrap;
    AddressV = wrap;
};

textureCUBE cubeTex;
samplerCUBE CubeTextureSampler = sampler_state
{
    Texture = <cubeTex>;
    MinFilter = anisotropic;
    MagFilter = anisotropic;
    MipFilter = Linear;
    AddressU = wrap;
    AddressV = wrap;
};

struct VertexShaderInput
{
    float3 Position            : POSITION0;
    float3 normal              : NORMAL0;
    float2 texCoord            : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position            : POSITION0;
    float2 texCoord            : TEXCOORD0;
    float3 worldPos			   : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(float4(input.Position,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    output.texCoord = input.texCoord*100;
    output.worldPos = worldPosition.xyz;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 diffuseColor = float4(0,0,1,1);
    
    float4 normalTexture1 = tex2D(NormalTextureSampler, input.texCoord*0.1+float2(time,time));
    float4 normalTexture2 = tex2D(NormalTextureSampler2, input.texCoord*0.1+float2(time,time));
    float4 normalTexture = (textureLerp*normalTexture1)+((1-textureLerp)*normalTexture2);
    float4 normalTexture3 = tex2D(NormalTextureSampler, input.texCoord*2+float2(-time,-time*2));
    float4 normalTexture4 = tex2D(NormalTextureSampler2, input.texCoord*2+float2(-time,-time*2));
    float4 normalTextureDetail = (textureLerp*normalTexture3)+((1-textureLerp)*normalTexture4);
    
    float3 normal = (((0.5*normalTexture)+(0.5*normalTextureDetail))*2)-1;
    normal.xyz = normal.xzy;
    normal = normalize(normal);
    
    float3 cubeTexCoords = reflect(input.worldPos-EyePos,normal);
    
    float3 cubeTex = texCUBE(CubeTextureSampler,cubeTexCoords).rgb;
    
    return float4((cubeTex*0.8)+(diffuseColor*0.2),1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
