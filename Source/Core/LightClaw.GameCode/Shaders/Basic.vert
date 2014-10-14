#version 400



in vec3 inVertexPosition;
in vec3 inVertexColor;

out vec4 vertexColor;

void main(void)
{
	gl_Position = vec4(inVertexPosition, 1.0f);
	vertexColor = vec4(inVertexColor, 1.0f);
}