#version 400

uniform mat4 MVP;

layout(location = 0) in vec3 inVertexPosition;
layout(location = 1) in vec3 inVertexNormal;
layout(location = 4) in vec2 inVertexTexCoords;

out vec3 vertexColor;

void main(void)
{
	gl_Position = MVP * vec4(inVertexPosition, 1.0f);
	vertexColor = vec3(1.0f, 0.0, 0.0);
}