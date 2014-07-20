struct VS_INPUT
{
	float4 Position : POSITION;
	float4 Color : COLOR;
};

struct PS_INPUT
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output = (PS_INPUT)0;

	output.Position = input.Position;
	output.Color = input.Position;
	
	return output;
}

float4 PS(PS_INPUT input) : SV_TARGET
{
	return input.Color;
}