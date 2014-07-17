#version 400

in vec3 passVertexColor;

out vec4 finalColor;

void main(void)
{
	finalColor = vec4(passVertexColor, 1.0);
}
