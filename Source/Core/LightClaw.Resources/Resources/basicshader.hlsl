float4 VS(float4 position : POSITION) : SV_POSITION
{
    return position;
}

float4 PS(float4 position : SV_POSITION) : SV_Target
{
    return float4(1.0f, 1.0f, 0.0f, 1.0f);
}