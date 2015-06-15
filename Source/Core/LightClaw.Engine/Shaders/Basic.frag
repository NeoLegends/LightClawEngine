#version 400

uniform sampler2D diffuse;

in vec2 texCoords;

out vec4 finalColor;

void main(void)
{
	finalColor = vec4(texture(diffuse, texCoords).rgb, 1.0f);
}