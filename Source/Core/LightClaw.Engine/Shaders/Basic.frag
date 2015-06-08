#version 400

in vec3 vertexColor;

out vec4 finalColor;

void main(void)
{
	finalColor = vec4(vertexColor, 1.0f);
}