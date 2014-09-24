#version 400

uniform mat4 modelViewProjectionMatrix;

in vec3 inVertexPosition;
in vec4 inVertexColor;

out vec4 vertexColor

void main(void)
{
	gl_Position = modelViewProjectionMatrix * inVertexPosition;
	vertexColor = inVertexColor;
}