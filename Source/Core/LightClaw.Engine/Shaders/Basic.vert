#version 400

uniform mat4 MVP;

in vec3 inVertexPosition;
in vec3 inVertexNormal;
in vec2 inVertexTexCoords;
in vec4 inVertexColor;

out vec4 vertexColor;

void main(void)
{
	gl_Position = vec4(inVertexPosition, 1.0f);
	vertexColor = inVertexColor;
}