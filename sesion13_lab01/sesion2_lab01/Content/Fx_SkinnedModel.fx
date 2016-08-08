cbuffer Fx_MiscInputBuffer {
	uniform float4x4 gWorld;
	uniform float4x4 gWorldInvTranspose;
	uniform float4x4 gWorldViewProj;
	uniform float4x4 gTexTransform;
};

struct VS_IN {
	float3 PosL    : POSITION;
    float3 NormalL : NORMAL; 
    float2 Tex    : TEXCOORD;
	float4 Tan		: TANGENT;
    float Weight0  : BLENDWEIGHT; 
    int4 BoneIndex : BLENDINDICES;
};

struct PS_IN {
	float4 PosH     : SV_POSITION;
    float3 PosW     : POSITION;
    float3 NormalW  : NORMAL;
	float3 TangentW : TANGENT;
	float2 Tex      : TEXCOORD;
};

Texture2D gDiffuseMap : register(t0);
SamplerState gDiffuseMapSampler : register(s0);

SamplerState samLinear {
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};

PS_IN VS( VS_IN input ) {
	PS_IN vout;
    
	float4 position = float4(input.PosL, 1.0f);

	// Transform to world space space.
	vout.PosW     = mul(position, gWorld).xyz;
	//vout.NormalW  = mul(n, gWorldInvTranspose).xyz;
	//vout.TangentW = float4(mul(t, (float3x3)gWorld), input.Tan.w);
	// Transform to homogeneous clip space.
	vout.PosH = mul(position, gWorldViewProj);
	
	// Output vertex attributes for interpolation across triangle.
	vout.Tex = mul(float4(input.Tex, 0.0f, 1.0f), gTexTransform).xy;

	return vout;
}

float4 PS(PS_IN input) : SV_Target {
	float4 texColor = float4(1, 1, 1, 1);

	texColor = gDiffuseMap.Sample( samLinear, input.Tex );

	return texColor;
}
