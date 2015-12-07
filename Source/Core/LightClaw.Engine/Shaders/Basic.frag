#version 400

uniform sampler2D diffuse;

in vec2 texCoords;

out vec4 finalColor;

void main(void)
{
	finalColor = vec4(1.0f, 0.0f, 0.0f, 1.0f);
}