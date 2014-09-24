#version 400

in vec4 vertexColor;

out vec3 finalColor;

void main(void)
{
	finalColor = vertexColor;
}