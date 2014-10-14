#version 400

uniform mat4 MVP;

in vec3 inVertexPosition;
in vec3 inVertexColor;

out vec4 vertexColor;

void main(void)
{
	gl_Position = MVP * vec4(inVertexPosition, 1.0f);
	vertexColor = vec4(inVertexColor, 1.0f);
}