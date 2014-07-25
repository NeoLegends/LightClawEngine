#version 400

uniform mat4 modelViewProjectionMatrix;
uniform sampler2D texture;

in vec2 passTextureCoordinates;

out vec3 finalColor;

void main(void)
{
	finalColor = texture(texture, passTextureCoordinates).rgb;
}