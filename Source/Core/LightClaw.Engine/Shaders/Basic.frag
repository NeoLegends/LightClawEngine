#version 400

uniform sampler2D diffuse;

in vec2 texCoords;

out vec4 finalColor;

void main(void)
{
	finalColor = texture(diffuse, texCoords).rgba;
}