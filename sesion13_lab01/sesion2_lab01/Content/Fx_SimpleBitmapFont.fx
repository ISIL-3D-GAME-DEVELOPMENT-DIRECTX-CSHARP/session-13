cbuffer ShaderInputParameters : register(b0) {
	uniform float4 textColor;
	uniform float4x4 worldViewProj;
};

struct VS_IN {
	float3 pos : POSITION0;
	float2 textCoord : TEXCOORD0;
};

struct PS_IN {
	float4 pos : SV_POSITION;
	float2 textCoord : TEXCOORD0;
	float4 textColor : COLOR0;
};

Texture2D picture : register(t0);
SamplerState pictureSampler : register(s0);

PS_IN VS( VS_IN input ) {
	PS_IN output = (PS_IN)0;
	
	float4 position = float4(input.pos.xyz, 1.0);

	output.pos = mul(position, worldViewProj);
	output.textColor = textColor;
	output.textCoord = input.textCoord;
	
	return output;
}

float4 PS( PS_IN input ) : SV_Target {
	float4 outputPixel = picture.Sample(pictureSampler, input.textCoord);
	return outputPixel * input.textColor;
}

technique10 Render {
	pass P0 {
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}